using UnityEngine;
using UnityEngine.InputSystem;

public class StrollerInput : MonoBehaviour
{
    [SerializeField] private StrollerController controller;

    private void Awake()
    {
        if (controller == null)
            controller = GetComponent<StrollerController>();
    }

    private void Update()
    {
        // Nuevo Input System, lee teclado directo
        var teclado = Keyboard.current;
        if (teclado == null) return;

        float giro = 0f;
        float acelerar = 0f;

        if (teclado.aKey.isPressed || teclado.leftArrowKey.isPressed) giro = -1f;
        if (teclado.dKey.isPressed || teclado.rightArrowKey.isPressed) giro = 1f;
        if (teclado.sKey.isPressed || teclado.upArrowKey.isPressed) acelerar = 1f;
        if (teclado.wKey.isPressed || teclado.downArrowKey.isPressed) acelerar = -1f;

        controller.SetInput(giro, acelerar);
    }
}