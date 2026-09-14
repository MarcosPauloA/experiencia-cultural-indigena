using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    private void OnEnable()
    {
        // O gerenciador global agora escuta apenas o sopro para trocar de cena
        HardwareEmulator.OnInstrumentoSoproTocado += ProcessarSopro;
    }

    private void OnDisable()
    {
        HardwareEmulator.OnInstrumentoSoproTocado -= ProcessarSopro;
    }

    private void ProcessarSopro()
    {
        // Trava de segurança para testes isolados da cena
        if (GameManager.Instancia == null)
        {
            Debug.LogWarning("[InteractionManager] GameManager não encontrado. Ignorando transição.");
            return;
        }

        // O instrumento de sopro invoca o desafio musical a partir da aldeia
        if (GameManager.Instancia.estadoAtual == GameManager.EstadoDoJogo.Exploracao)
        {
            Debug.Log("[Interação] Transição para o ritual musical...");
            GameManager.Instancia.MudarEstado(GameManager.EstadoDoJogo.DesafioMusical);
        }
    }
}