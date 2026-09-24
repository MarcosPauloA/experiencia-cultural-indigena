using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System;

public class LeitorSerial : MonoBehaviour
{
    [Header("Configurações da Porta")]
    public string porta = "COM3"; // Mude para a porta do seu simulador
    public int baudRate = 9600;

    private SerialPort portaSerial;
    private Thread threadLeitura;
    private bool lendo = false;

    // Variáveis de trânsito (Ponte entre a Thread Secundária e a Main Thread)
    private bool percussaoDetectada = false;
    public int valorSopro { get; private set; } = 0;

    void Start()
    {
        ConectarHardware();
    }

    void ConectarHardware()
    {
        try
        {
            portaSerial = new SerialPort(porta, baudRate);
            portaSerial.ReadTimeout = 50; 
            portaSerial.DtrEnable = true; 
            portaSerial.RtsEnable = true;
            portaSerial.Open();

            lendo = true;
            threadLeitura = new Thread(EscutarPorta);
            threadLeitura.IsBackground = true;
            threadLeitura.Start();

            Debug.Log($"[LeitorSerial] Conectado na porta {porta}");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[LeitorSerial] Simulador/Arduino não encontrado na {porta}: {e.Message}");
        }
    }

    void EscutarPorta()
    {
        while (lendo && portaSerial != null && portaSerial.IsOpen)
        {
            try
            {
                string linha = portaSerial.ReadLine(); // Trava aqui até receber o \n
                ProcessarPacote(linha);
            }
            catch (TimeoutException) { /* Timeout é normal, mantém o loop vivo */ }
            catch (Exception e) { Debug.LogWarning("[LeitorSerial] Falha na leitura: " + e.Message); }
        }
    }

    void ProcessarPacote(string pacote)
    {
        // LOG OPACIONAL: Descomente a linha abaixo se quiser ver TUDO que chega na porta
        Debug.Log($"[LeitorSerial] Dado bruto recebido do Arduino: {pacote}");

        // Espera dados no formato "Percussao,Sopro" -> Ex: "1,512" ou "0,0"
        string[] dados = pacote.Trim().Split(',');
        
        if (dados.Length == 2)
        {
            if (int.TryParse(dados[0], out int toquePercussao))
            {
                // Se leu 1, o botão foi apertado!
                if (toquePercussao == 1) 
                {
                    Debug.Log("[LeitorSerial] BOTÃO APERTADO NO ARDUINO! Levantando a bandeira...");
                    percussaoDetectada = true;
                }
            }

            if (int.TryParse(dados[1], out int intensidadeSopro))
            {
                valorSopro = intensidadeSopro;
            }
        }
    }

    void Update()
    {
        // A MAIN THREAD verifica a bandeira e dispara o evento no jogo
        if (percussaoDetectada)
        {
            percussaoDetectada = false; // Abaixa a bandeira
            
            // Dispara o evento original do seu projeto!
            HardwareEmulator.DispararBatidaPorHardware(); 
        }
    }

    void OnDestroy()
    {
        lendo = false;
        if (threadLeitura != null && threadLeitura.IsAlive) threadLeitura.Join(100);
        if (portaSerial != null && portaSerial.IsOpen) portaSerial.Close();
    }
}