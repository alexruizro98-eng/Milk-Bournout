using UnityEngine;
using UnityEngine.InputSystem;

public class StrollerInput : MonoBehaviour
{
    [SerializeField] private StrollerController controller;

    private void Update()
    {
        if (Keyboard.current == null) return;

        var teclado = Keyboard.current;

        float giro = 0f;
        if (teclado.aKey.isPressed || teclado.leftArrowKey.isPressed) giro = -1f;
        if (teclado.dKey.isPressed || teclado.rightArrowKey.isPressed) giro = 1f;

        float acelerar = 0f;
        if (teclado.wKey.isPressed || teclado.upArrowKey.isPressed) acelerar = -1f;
        if (teclado.sKey.isPressed || teclado.downArrowKey.isPressed) acelerar = 1f;

        bool derrapar = teclado.spaceKey.isPressed;

        controller.SetInput(giro, acelerar);
        controller.SetDerrape(derrapar);
    }
}