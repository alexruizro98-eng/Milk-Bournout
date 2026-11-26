using UnityEngine;
using UnityEngine.InputSystem;

/// Entrada del jugador
///   W / ↑ = adelante     S / ↓ = reversa
///   A / ← = izquierda    D / → = derecha
///   Espacio = salto      Shift = derrape      Q = turbo      R = reaparecer

public class PlayerInput : MonoBehaviour, IStrollerInput
{
    public float Steer { get; private set; }
    public float Throttle { get; private set; }
    public bool Jump { get; private set; }
    public bool Drift { get; private set; }
    public bool Turbo { get; private set; }
    public bool Respawn { get; private set; }

    void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null)
        {
            Clear();
            return;
        }

        // El avance (W/S) y el giro (A/D) son ejes totalmente independientes.

        Throttle = Axis(kb.sKey.isPressed || kb.downArrowKey.isPressed,
                        kb.wKey.isPressed || kb.upArrowKey.isPressed);
        Steer = Axis(kb.aKey.isPressed || kb.leftArrowKey.isPressed,
                     kb.dKey.isPressed || kb.rightArrowKey.isPressed);

        Jump = kb.spaceKey.isPressed;
        Drift = kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed;
        Turbo = kb.qKey.isPressed;
        Respawn = kb.rKey.isPressed;
    }

    void OnDisable()
    {
        Clear();
    }

    void Clear()
    {
        Steer = 0f;
        Throttle = 0f;
        Jump = false;
        Drift = false;
        Turbo = false;
        Respawn = false;
    }

    static float Axis(bool negative, bool positive)
    {
        return (positive ? 1f : 0f) - (negative ? 1f : 0f);
    }
}