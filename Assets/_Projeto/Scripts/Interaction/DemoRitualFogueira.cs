using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DemoRitualFogueira : MonoBehaviour
{
    [Header("Elementos Visuais")]
    public Text textoInstrucao;
    public Transform lua;
    public SpriteRenderer fogueiraSprite;
    public ParticleSystem faiscas;

    [Header("Áudio")]
    public AudioSource somTambor;
    public AudioSource somFlauta;

    [Header("Configurações de Ritmo")]
    public float tempoExibicaoTexto = 8f; 
    public float bpm = 60f; 
    public float tolerancia = 0.25f; 
    public int acertosNecessarios = 4;

    private float intervaloBatida;
    private float tempoProximaBatida;
    private int acertosAtuais = 0;
    private bool ritualAtivo = false;

    private void Start()
    {
        intervaloBatida = 60f / bpm;
        fogueiraSprite.color = Color.gray; // Fogueira começa apagada
        
        // Garante que as faíscas comecem zeradas, mas ativas
        var emission = faiscas.emission;
        emission.rateOverTime = 0f;
        faiscas.Play();
        
        StartCoroutine(IniciarSequencia());
    }

    private void OnEnable()
    {
        HardwareEmulator.OnInstrumentoPercussaoTocado += TentarAcerto;
    }

    private void OnDisable()
    {
        HardwareEmulator.OnInstrumentoPercussaoTocado -= TentarAcerto;
    }

    private IEnumerator IniciarSequencia()
    {
        yield return new WaitForSeconds(tempoExibicaoTexto);
        textoInstrucao.text = "";

        ritualAtivo = true;
        tempoProximaBatida = Time.time + intervaloBatida;
        StartCoroutine(LoopMetronomoVisual());
    }

    private IEnumerator LoopMetronomoVisual()
    {
        while (ritualAtivo)
        {
            somTambor.Play();
            
            float escalaBase = 1f + (acertosAtuais * 0.3f); 
            lua.localScale = Vector3.one * (escalaBase + 0.3f); 
            
            yield return new WaitForSeconds(0.15f);
            
            lua.localScale = Vector3.one * escalaBase; 
            
            yield return new WaitForSeconds(intervaloBatida - 0.15f);
        }
    }

    private void Update()
    {
        if (!ritualAtivo) return;

        if (Time.time >= tempoProximaBatida)
        {
            tempoProximaBatida += intervaloBatida;
        }
    }

    private void TentarAcerto()
    {
        if (!ritualAtivo) return;

        float diferencaAtual = Mathf.Abs(Time.time - tempoProximaBatida);
        float diferencaAnterior = Mathf.Abs(Time.time - (tempoProximaBatida - intervaloBatida));

        if (diferencaAtual <= tolerancia || diferencaAnterior <= tolerancia)
        {
            RegistrarAcerto();
        }
        else
        {
            Debug.Log("Fora do tempo! Diferença: " + Mathf.Min(diferencaAtual, diferencaAnterior));
        }
    }

    private void RegistrarAcerto()
    {
        acertosAtuais++;
        somFlauta.Play();

        float progresso = (float)acertosAtuais / acertosNecessarios;
        
        // --- A MÁGICA DAS PARTÍCULAS ACONTECE AQUI ---
        
        // 1. Aumenta a quantidade de faíscas emitidas por segundo (de 10 para 60)
        var emission = faiscas.emission;
        emission.rateOverTime = Mathf.Lerp(10f, 60f, progresso);

        // 2. Aumenta a velocidade inicial (faz as faíscas voarem mais alto)
        var main = faiscas.main;
        main.startSpeed = Mathf.Lerp(1.5f, 5f, progresso);

        // 3. Aumenta o raio de espalhamento da base da fogueira
        var shape = faiscas.shape;
        shape.radius = Mathf.Lerp(0.2f, 1.5f, progresso); 

        // ---------------------------------------------

        // Aquece a cor da lenha
        fogueiraSprite.color = Color.Lerp(Color.gray, Color.white, progresso);

        if (acertosAtuais >= acertosNecessarios)
        {
            FinalizarRitual();
        }
    }

    private void FinalizarRitual()
    {
        ritualAtivo = false;
        textoInstrucao.text = "A fogueira sagrada foi acesa!";
        
        // Brilho máximo no final
        var main = faiscas.main;
        main.startSpeed = 6f;
        var emission = faiscas.emission;
        emission.rateOverTime = 100f;
    }
}