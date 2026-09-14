using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Necessário para usar Coroutines
using System.Collections.Generic;

public class NPCInteractable : MonoBehaviour
{
    [Header("Interface do Balão")]
    public GameObject balaoDeDialogo; 
    public Text textoBalao;
    [Tooltip("Tempo em segundos entre cada letra")]
    public float velocidadeDigitacao = 0.04f; 
    
    [Header("Narrativa")]
    public string[] falasDoNPC;

    private bool jogadorProximo = false;
    private bool conversando = false;
    private bool digitando = false; // Controla se a animação está rodando
    private string fraseAtual = "";
    private Coroutine rotinaDigitacao;
    private Queue<string> filaDeFrases;

    private void Awake()
    {
        filaDeFrases = new Queue<string>();
        balaoDeDialogo.SetActive(false); 
    }

    private void OnEnable()
    {
        HardwareEmulator.OnSensorAproximacaoAtivado += Interagir;
    }

    private void OnDisable()
    {
        HardwareEmulator.OnSensorAproximacaoAtivado -= Interagir;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) jogadorProximo = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jogadorProximo = false;
            if (conversando) EncerrarDialogo(); // Fecha o balão se o jogador se afastar muito
        }
    }

    private void Interagir()
    {
        if (!jogadorProximo) return; 

        if (!conversando)
        {
            IniciarDialogo();
        }
        else if (digitando)
        {
            // O jogador apertou E durante a animação: auto-completa o texto na hora
            CompletarFraseImediatamente();
        }
        else
        {
            AvancarFrase();
        }
    }

    private void IniciarDialogo()
    {
        if (GameManager.Instancia != null)
            GameManager.Instancia.MudarEstado(GameManager.EstadoDoJogo.Dialogo);

        conversando = true;
        balaoDeDialogo.SetActive(true);
        filaDeFrases.Clear();

        foreach (string fala in falasDoNPC)
        {
            filaDeFrases.Enqueue(fala);
        }
        
        AvancarFrase();
    }

    private void AvancarFrase()
    {
        if (filaDeFrases.Count == 0)
        {
            EncerrarDialogo();
            return;
        }
        
        fraseAtual = filaDeFrases.Dequeue();
        
        // Garante que não teremos duas corrotinas de texto rodando ao mesmo tempo
        if (rotinaDigitacao != null) StopCoroutine(rotinaDigitacao);
        
        // Inicia o efeito de máquina de escrever
        rotinaDigitacao = StartCoroutine(DigitarFrase(fraseAtual));
    }

    // A Coroutine que faz a mágica de desenhar letra por letra
    private IEnumerator DigitarFrase(string frase)
    {
        digitando = true;
        textoBalao.text = ""; // Limpa a caixa de texto

        foreach (char letra in frase.ToCharArray())
        {
            textoBalao.text += letra; // Adiciona uma letra
            yield return new WaitForSeconds(velocidadeDigitacao); // Pausa o código aqui
        }

        digitando = false;
    }

    private void CompletarFraseImediatamente()
    {
        if (rotinaDigitacao != null) StopCoroutine(rotinaDigitacao);
        textoBalao.text = fraseAtual; // Imprime a frase inteira de uma vez
        digitando = false;
    }

    private void EncerrarDialogo()
    {
        conversando = false;
        balaoDeDialogo.SetActive(false);
        
        if (GameManager.Instancia != null && GameManager.Instancia.estadoAtual == GameManager.EstadoDoJogo.Dialogo)
        {
            GameManager.Instancia.MudarEstado(GameManager.EstadoDoJogo.Exploracao);
        }
    }
}