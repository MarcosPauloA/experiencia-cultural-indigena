using System;
using UnityEngine;
using UnityEngine.InputSystem; 

public class HardwareEmulator : MonoBehaviour
{
    public static event Action OnInstrumentoPercussaoTocado;
    public static event Action OnInstrumentoSoproTocado;
    public static event Action OnSensorAproximacaoAtivado;

    void Update()
    {
        if (Keyboard.current == null) return;

        // Tecla Q: Simula a Percussão
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            Debug.Log("[Hardware] Sinal físico recebido: Percussão");
            OnInstrumentoPercussaoTocado?.Invoke();
        }

        // CORREÇÃO: Tecla R (ao invés de W): Simula o Sopro, liberando o W para o movimento
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Debug.Log("[Hardware] Sinal físico recebido: Sopro");
            OnInstrumentoSoproTocado?.Invoke();
        }

        // Tecla E: Simula o Sensor de Aproximação (Interagir com NPC)
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("[Hardware] Sinal físico recebido: Sensor de ambiente");
            OnSensorAproximacaoAtivado?.Invoke();
        }
    }
}