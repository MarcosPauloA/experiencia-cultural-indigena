# experiencia-cultural-indigena

### Sobre o Projeto
Um jogo 2D *top-down* em *pixel art* com temática de cultura indígena, focado em exploração narrativa e mecânicas rítmicas. O projeto atua na intersecção entre o entretenimento digital e o design de Instrumentos Musicais Digitais (DMI), utilizando periféricos físicos customizados para criar um engajamento imersivo e acessível através de desafios musicais integrados ao próprio ambiente (*World-Space*).

### Arquitetura Técnica
A fundação do projeto foi desenvolvida em Unity (C#) prezando pela modularidade, otimização de performance e boas práticas de engenharia de software:

* **Gerenciamento de Estado:** Arquitetura centralizada via `GameManager` (padrão *Singleton*, `DontDestroyOnLoad`) para controle de estados (Exploração, Diálogo, Desafio Musical).
* **Movimentação e Animação:** Controle via *New Input System* e motor de animação baseado em *Blend Trees* (2D Simple Directional) com implementação de "Falso Idle" para transições fluidas.
* **Sistemas Narrativos:** Diálogos modulares renderizados em *World Space* com efeito *Typewriter*, restritos por colisores de proximidade para evitar bloqueios indevidos do jogador.
* **Validação Rítmica:** Relógio DSP (`AudioSettings.dspTime`) implementado para garantir precisão matemática nos desafios, com ajuste de tolerância dinâmico (DDA) para minimizar frustrações.

### Hardware e Acessibilidade
O jogo foi projetado para se comunicar com sistemas embarcados e microcontroladores, promovendo interação física tátil:

* **Sensores de Percussão:** Resposta a impactos mecânicos (transientes) para comandos diretos, validação de ritmo estrito e ações diegéticas no mapa.
* **Sensores de Sopro:** Resposta a dados analógicos contínuos para sustentar rituais e modular parâmetros sonoros e visuais da cena.
* **Feedback Multissensorial:** Acessibilidade integrada através de expansões visuais (partículas *bloom*, pulso concêntrico de cenários) e pistas auditivas para engajar jogadores com diferentes perfis sensoriais.

### Próximos Passos (Roadmap)
* Implementar comunicação Serial multithread assíncrona para leitura de transdutores físicos sem bloquear a *main thread* do motor gráfico.
* Migrar a parametrização das trilhas musicais para `ScriptableObjects`, separando os dados rítmicos do código-fonte.
* Integrar protocolos de áudio em tempo real (OSC/MIDI) para síntese sonora procedural com motores externos.
