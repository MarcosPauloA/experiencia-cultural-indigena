using System;
using UnityEngine;
using UnityEngine.InputSystem; 

public class HardwareEmulator : MonoBehaviour
{
    public static event Action OnInstrumentoPercussaoTocado;
    public static event Action OnInstrumentoSoproTocado;
    public static event Action OnSensorAproximacaoAtivado;

    // =========================================================
    // PONTES PARA A COMUNICAÇÃO SERIAL (Chamados pelo LeitorSerial.cs)
    // =========================================================
    
    public static void DispararBatidaPorHardware()
    {
        Debug.Log("[Hardware] Sinal SERIAL recebido do Arduino virtual: Percussão");
        OnInstrumentoPercussaoTocado?.Invoke();
    }

    // Já deixamos o método preparado para receber o valor analógico do sopro no futuro
    public static void DispararSoproPorHardware(int intensidade)
    {
        Debug.Log($"[Hardware] Sinal SERIAL recebido do Arduino virtual: Sopro (Valor: {intensidade})");
        OnInstrumentoSoproTocado?.Invoke();
    }

    // =========================================================

    void Update()
    {
        if (Keyboard.current == null) return;

        // Tecla Q: Simula a Percussão pelo Teclado
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            Debug.Log("[Hardware] Sinal físico (Teclado) recebido: Percussão");
            OnInstrumentoPercussaoTocado?.Invoke();
        }

        // Tecla R (ao invés de W): Simula o Sopro pelo Teclado
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Debug.Log("[Hardware] Sinal físico (Teclado) recebido: Sopro");
            OnInstrumentoSoproTocado?.Invoke();
        }

        // Tecla E: Simula o Sensor de Aproximação (Interagir com NPC)
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("[Hardware] Sinal físico (Teclado) recebido: Sensor de ambiente");
            OnSensorAproximacaoAtivado?.Invoke();
        }
    }
}