using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class RhythmManager : MonoBehaviour
{
    [Header("Configurações da Música")]
    public float bpm = 120f; 
    [Tooltip("Quantas batidas o teste vai durar. 64 batidas a 120 BPM = 32 segundos.")]
    public int quantidadeDeBatidas = 64; 
    public float janelaDeTolerancia = 0.25f; // Margem de erro em segundos

    [Header("Interface Adaptativa")]
    public Text textoFeedbackVisual; 
    public Image indicadorVisualRitmo; 
    public Color corBase = Color.white;
    public Color corBatida = Color.green;

    private AudioSource geradorDeAudio;
    private AudioClip somMetronomo;
    
    private List<float> batidasEsperadas = new List<float>();
    private int indiceBatidaAtual = 0;
    private int indiceMetronomo = 0;
    private double tempoInicioMusicaDSP; 
    private bool musicaTocando = false;

    private void Awake()
    {
        geradorDeAudio = GetComponent<AudioSource>();
        CriarMetronomoSintetico();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += IniciarDesafioSincronizado;
        HardwareEmulator.OnInstrumentoPercussaoTocado += ValidarInput;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= IniciarDesafioSincronizado;
        HardwareEmulator.OnInstrumentoPercussaoTocado -= ValidarInput;
    }

private void CriarMetronomoSintetico()
    {
        somMetronomo = AudioClip.Create("Bipe", 44100 / 10, 1, 44100, false);
        float[] samples = new float[somMetronomo.samples];

        for (int i = 0; i < samples.Length; i++) 
        {
            // Decaimento linear simples para evitar estalos (clicks) de áudio no corte da onda
            float envelope = 1f - ((float)i / samples.Length);
            samples[i] = Mathf.Sin(2 * Mathf.PI * 440 * i / 44100f) * envelope; 
        }

        somMetronomo.SetData(samples, 0);

        // Atribui o clipe procedural ao componente para permitir o agendamento via PlayScheduled
        geradorDeAudio.clip = somMetronomo;
    }
    
    // Calcula os tempos em segundos com base no BPM
    private void GerarPadraoRitmico()
    {
        batidasEsperadas.Clear();
        float intervaloEntreBatidas = 60f / bpm;

        for (int i = 0; i < quantidadeDeBatidas; i++)
        {
            // A primeira batida ocorre no segundo 0, a segunda em 0.5 (se 120 BPM), etc.
            batidasEsperadas.Add(i * intervaloEntreBatidas);
        }
    }

    private void IniciarDesafioSincronizado(Scene cena, LoadSceneMode modo)
    {
        if (cena.name == "Cena_DesafioMusical")
        {
            // Procura a interface na nova cena
            GameObject objTexto = GameObject.Find("TextoFeedback");
            if (objTexto != null) textoFeedbackVisual = objTexto.GetComponent<Text>();

            GameObject objImagem = GameObject.Find("IndicadorRitmo");
            if (objImagem != null) indicadorVisualRitmo = objImagem.GetComponent<Image>();

            // Gera a sequência longa baseada no BPM
            GerarPadraoRitmico();

            indiceBatidaAtual = 0;
            indiceMetronomo = 0;
            
            // Adiciona 2 segundos de "delay" para o jogador se preparar
            tempoInicioMusicaDSP = AudioSettings.dspTime + 2.0; 
            musicaTocando = true;
            
            MostrarFeedback("PREPARE-SE!");
        }
        else
        {
            musicaTocando = false;
        }
    }

    private void Update()
    {
        if (!musicaTocando) return;

        double tempoAtualDSP = AudioSettings.dspTime - tempoInicioMusicaDSP;

        // Se o tempoAtual for negativo, estamos nos 2 segundos de preparação (não faz nada ainda)
        if (tempoAtualDSP < 0) return; 

        // 1. ANIMAÇÃO VISUAL ADAPTATIVA
        if (indicadorVisualRitmo != null && indiceMetronomo < batidasEsperadas.Count)
        {
            float tempoRestante = batidasEsperadas[indiceMetronomo] - (float)tempoAtualDSP;
            float tempoDePiscar = Mathf.Min(0.2f, (60f / bpm) / 2f); // Adapta a velocidade do piscar ao BPM

            if (tempoRestante > 0 && tempoRestante <= tempoDePiscar)
            {
                indicadorVisualRitmo.color = Color.Lerp(corBatida, corBase, tempoRestante / tempoDePiscar);
            }
            else
            {
                indicadorVisualRitmo.color = corBase;
            }
        }

        // 2. REPRODUÇÃO DE ÁUDIO AGENDADA
        if (indiceMetronomo < batidasEsperadas.Count)
        {
            double tempoExatoDaProximaBatida = tempoInicioMusicaDSP + batidasEsperadas[indiceMetronomo];
            
            if (AudioSettings.dspTime >= tempoExatoDaProximaBatida - 0.1)
            {
                geradorDeAudio.PlayScheduled(tempoExatoDaProximaBatida);
                indiceMetronomo++;
            }
        }

        // 3. VALIDAÇÃO DE PERDA DE TEMPO (Jogador deixou passar)
        if (indiceBatidaAtual < batidasEsperadas.Count)
        {
            if (tempoAtualDSP > batidasEsperadas[indiceBatidaAtual] + janelaDeTolerancia)
            {
                MostrarFeedback("ERROU (Passou)!");
                indiceBatidaAtual++; 
            }
        }
        else if (indiceBatidaAtual >= batidasEsperadas.Count && indiceMetronomo >= batidasEsperadas.Count)
        {
            musicaTocando = false;
            MostrarFeedback("FIM DO DESAFIO!");
            Invoke("VoltarParaAldeia", 3f);
        }
    }

    private void ValidarInput()
    {
        if (!musicaTocando || indiceBatidaAtual >= batidasEsperadas.Count) return;

        double tempoAtualDSP = AudioSettings.dspTime - tempoInicioMusicaDSP;
        
        // Evita que o jogador "acidentalmente" jogue durante os 2 segundos de preparação
        if (tempoAtualDSP < -janelaDeTolerancia) return; 

        float tempoEsperado = batidasEsperadas[indiceBatidaAtual];
        float precisao = Mathf.Abs((float)tempoAtualDSP - tempoEsperado);

        if (precisao <= janelaDeTolerancia)
        {
            if (precisao <= 0.08f) MostrarFeedback("PERFEITO!");
            else MostrarFeedback("BOM!");
            indiceBatidaAtual++; 
        }
        else if (tempoAtualDSP < tempoEsperado - janelaDeTolerancia)
        {
            MostrarFeedback("ERROU (Muito cedo)!");
        }
    }

    private void MostrarFeedback(string mensagem)
    {
        if (textoFeedbackVisual != null) textoFeedbackVisual.text = mensagem;
    }

    private void VoltarParaAldeia()
    {
        GameManager.Instancia.MudarEstado(GameManager.EstadoDoJogo.Exploracao);
    }
}