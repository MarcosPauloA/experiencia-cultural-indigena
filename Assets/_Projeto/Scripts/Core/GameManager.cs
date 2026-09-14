using UnityEngine;
using UnityEngine.SceneManagement; // Essencial para gerenciar as cenas
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia;

    public enum EstadoDoJogo { Exploracao, DesafioMusical, Dialogo }
    public EstadoDoJogo estadoAtual;

    public static event Action<EstadoDoJogo> AoMudarEstado;

    private void Awake()
    {
        // Padrão Singleton com Persistência
        if (Instancia == null) 
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject); // O segredo: protege os gerenciadores e a conexão de hardware
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Garante que o jogo inicie na exploração e avise todos os scripts
        MudarEstado(EstadoDoJogo.Exploracao);
    }

public void MudarEstado(EstadoDoJogo novoEstado)
    {
        estadoAtual = novoEstado;
        
        switch (estadoAtual)
        {
            case EstadoDoJogo.Exploracao:
                // SÓ carrega a cena se estivermos voltando do desafio musical. 
                // Se já estivermos na aldeia (só fechando um diálogo), ele não faz nada.
                if (SceneManager.GetActiveScene().name != "Cena_Exploracao")
                {
                    Debug.Log("Carregando cena da Aldeia...");
                    SceneManager.LoadScene("Cena_Exploracao");
                }
                else
                {
                    Debug.Log("Retomando a exploração (Cena já carregada).");
                }
                break;
                
            case EstadoDoJogo.DesafioMusical:
                if (SceneManager.GetActiveScene().name != "Cena_DesafioMusical")
                {
                    Debug.Log("Carregando cena do Desafio...");
                    SceneManager.LoadScene("Cena_DesafioMusical");
                }
                break;
                
            case EstadoDoJogo.Dialogo:
                Debug.Log("Modo de Diálogo ativado. Personagem paralisado.");
                // O diálogo não carrega cenas, apenas muda o estado lógico
                break;
        }

        AoMudarEstado?.Invoke(novoEstado);
    }
}