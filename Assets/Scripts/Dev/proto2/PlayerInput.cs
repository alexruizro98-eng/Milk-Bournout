using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Lee el teclado (WASD / flechas) y el mando, y lo expone como IStrollerInput.
/// Requiere el paquete "Input System" (Window > Package Manager).
/// Las acciones se crean por código: no necesitas un asset de Input Actions.
/// </summary>
public class PlayerInput : MonoBehaviour, IStrollerInput
{
    InputAction throttleAction;
    InputAction steerAction;

    public float Steer { get; private set; }
    public float Throttle { get; private set; }

    void Awake()
    {
        throttleAction = new InputAction("Throttle", InputActionType.Value);
        throttleAction.AddCompositeBinding("1DAxis")
            .With("Negative", "<Keyboard>/s")
            .With("Positive", "<Keyboard>/w");
        throttleAction.AddCompositeBinding("1DAxis")
            .With("Negative", "<Keyboard>/downArrow")
            .With("Positive", "<Keyboard>/upArrow");
        throttleAction.AddBinding("<Gamepad>/leftStick/y");

        steerAction = new InputAction("Steer", InputActionType.Value);
        steerAction.AddCompositeBinding("1DAxis")
            .With("Negative", "<Keyboard>/a")
            .With("Positive", "<Keyboard>/d");
        steerAction.AddCompositeBinding("1DAxis")
            .With("Negative", "<Keyboard>/leftArrow")
            .With("Positive", "<Keyboard>/rightArrow");
        steerAction.AddBinding("<Gamepad>/leftStick/x");
    }

    void OnEnable()
    {
        throttleAction.Enable();
        steerAction.Enable();
    }

    void OnDisable()
    {
        throttleAction.Disable();
        steerAction.Disable();
        Throttle = 0f;
        Steer = 0f;
    }

    void Update()
    {
        Throttle = Mathf.Clamp(throttleAction.ReadValue<float>(), -1f, 1f);
        Steer = Mathf.Clamp(steerAction.ReadValue<float>(), -1f, 1f);
    }

    void OnDestroy()
    {
        throttleAction.Dispose();
        steerAction.Dispose();
    }
}
