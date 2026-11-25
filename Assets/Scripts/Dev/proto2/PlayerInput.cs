using UnityEngine;
using UnityEngine.InputSystem;

/// Entrada del jugador solo con teclado:
///   W / ? = adelante     S / ? = reversa
///   A / ? = izquierda    D / ? = derecha
///   Espacio = salto      Shift = derrape

public class PlayerInput : MonoBehaviour, IStrollerInput
{
    public float Steer { get; private set; }
    public float Throttle { get; private set; }
    public bool Jump { get; private set; }
    public bool Drift { get; private set; }
    public bool Turbo { get; private set; }

    void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null)
        {
            Steer = 0f;
            Throttle = 0f;
            Jump = false;
            Drift = false;
            return;
        }

        // El avance (W/S) y el giro (A/D) 
        Throttle = Axis(kb.sKey.isPressed || kb.downArrowKey.isPressed,
                        kb.wKey.isPressed || kb.upArrowKey.isPressed);
        Steer = Axis(kb.aKey.isPressed || kb.leftArrowKey.isPressed,
                     kb.dKey.isPressed || kb.rightArrowKey.isPressed);

        Jump = kb.spaceKey.isPressed;
        Drift = kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed;
        Turbo = kb.qKey.isPressed;
    }

    void OnDisable()
    {
        Steer = 0f;
        Throttle = 0f;
        Jump = false;
        Drift = false;
        Turbo = false;
    }

    static float Axis(bool negative, bool positive)
    {
        return (positive ? 1f : 0f) - (negative ? 1f : 0f);
    }
}