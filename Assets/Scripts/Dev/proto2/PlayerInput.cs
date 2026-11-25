using UnityEngine;
using UnityEngine.InputSystem;

///   Mover: W/S + A/D
///   Salto: Espacio
///   Derrape: Shift

public class PlayerInput : MonoBehaviour, IStrollerInput
{
    InputAction throttleAction;
    InputAction steerAction;
    InputAction jumpAction;
    InputAction driftAction;

    public float Steer { get; private set; }
    public float Throttle { get; private set; }
    public bool Jump => jumpAction != null && jumpAction.IsPressed();
    public bool Drift => driftAction != null && driftAction.IsPressed();

    void Awake()
    {
        throttleAction = new InputAction("Throttle", InputActionType.Value);
        throttleAction.AddCompositeBinding("1DAxis")
            .With("Negative", "<Keyboard>/s")
            .With("Positive", "<Keyboard>/w");
        throttleAction.AddCompositeBinding("1DAxis")
            .With("Negative", "<Keyboard>/downArrow")
            .With("Positive", "<Keyboard>/upArrow");
        

        steerAction = new InputAction("Steer", InputActionType.Value);
        steerAction.AddCompositeBinding("1DAxis")
            .With("Negative", "<Keyboard>/a")
            .With("Positive", "<Keyboard>/d");
        steerAction.AddCompositeBinding("1DAxis")
            .With("Negative", "<Keyboard>/leftArrow")
            .With("Positive", "<Keyboard>/rightArrow");
        

        jumpAction = new InputAction("Jump", InputActionType.Button);
        jumpAction.AddBinding("<Keyboard>/space");
       

        driftAction = new InputAction("Drift", InputActionType.Button);
        driftAction.AddBinding("<Keyboard>/leftShift");
        driftAction.AddBinding("<Keyboard>/rightShift");
       
    }

    void OnEnable()
    {
        throttleAction.Enable();
        steerAction.Enable();
        jumpAction.Enable();
        driftAction.Enable();
    }

    void OnDisable()
    {
        throttleAction.Disable();
        steerAction.Disable();
        jumpAction.Disable();
        driftAction.Disable();
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
        jumpAction.Dispose();
        driftAction.Dispose();
    }
}