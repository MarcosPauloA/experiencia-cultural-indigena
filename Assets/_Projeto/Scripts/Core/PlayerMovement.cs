using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float velocidade = 5f;
    private Animator animador;

    private float ultimoMoveX = 0f;
    private float ultimoMoveY = -1f; 

    void Awake()
    {
        animador = GetComponent<Animator>();
    }

    void Update()
    {
        if (GameManager.Instancia == null)
        {
            Debug.LogWarning("[PlayerMovement] GameManager não encontrado.");
        }
        else if (GameManager.Instancia.estadoAtual != GameManager.EstadoDoJogo.Exploracao)
        {
            // Se entrar em diálogo, zera a velocidade para voltar ao falso Idle
            if (animador != null) animador.SetFloat("Velocidade", 0f); 
            return;
        }

        if (Keyboard.current == null) return;

        float moveX = 0f;
        float moveY = 0f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX = 1f;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveY = 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveY = -1f;

        Vector3 movimento = new Vector3(moveX, moveY, 0f).normalized;
        transform.Translate(movimento * velocidade * Time.deltaTime);

        if (animador != null)
        {
            // A magnitude varia de 0 (parado) a 1 (andando). A transição do Animator fará o resto.
            animador.SetFloat("Velocidade", movimento.magnitude);

            if (movimento.magnitude > 0)
            {
                ultimoMoveX = moveX;
                ultimoMoveY = moveY;
            }

            animador.SetFloat("MoveX", ultimoMoveX);
            animador.SetFloat("MoveY", ultimoMoveY);
        }
    }
}