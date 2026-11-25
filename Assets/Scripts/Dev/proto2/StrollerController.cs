using UnityEngine;

/// <summary>
/// Carriola con física arcade: adelante, atrás, giro, y tres mecánicas extra:
/// aceleración con curva, derrape y salto.
/// Lee un IStrollerInput (jugador o IA) y los valores de un StrollerStats.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class StrollerController : MonoBehaviour
{
    [SerializeField] StrollerStats stats;

    [Header("Detección de suelo")]
    [SerializeField] LayerMask groundMask = ~0;
    [Tooltip("Distancia desde el pivote hasta el suelo + un margen. Con un cubo de 1 m de alto y pivote al centro: ~0.6.")]
    [SerializeField] float groundCheckDistance = 0.6f;

    Rigidbody rb;
    IStrollerInput input;

    bool grounded;
    bool jumpRequested;
    bool prevJumpHeld;
    bool drifting;
    float driftDirection;

    /// <summary>Velocidad con signo en el eje de avance (negativa = reversa).</summary>
    public float ForwardSpeed { get; private set; }
    /// <summary>0..1 respecto a la velocidad máxima. Útil para cámara, audio y HUD.</summary>
    public float NormalizedSpeed => stats != null ? Mathf.Clamp01(Mathf.Abs(ForwardSpeed) / stats.maxForwardSpeed) : 0f;
    public bool IsGrounded => grounded;
    public bool IsDrifting => drifting;
    /// <summary>-1 = derrapa a la izquierda, 1 = a la derecha, 0 = no derrapa.</summary>
    public float DriftDirection => drifting ? driftDirection : 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // la rotación la controlamos nosotros (solo en Y)

        input = GetComponent<IStrollerInput>();
        if (input == null)
            Debug.LogError($"{name}: falta un componente que implemente IStrollerInput (ej. PlayerInput).", this);
        if (stats == null)
            Debug.LogError($"{name}: asigna un StrollerStats en el inspector.", this);
    }

    // La pulsación de salto se lee en Update para no perderla entre pasos de física.
    void Update()
    {
        if (input == null) return;

        bool jumpHeld = input.Jump;
        if (jumpHeld && !prevJumpHeld) jumpRequested = true;
        prevJumpHeld = jumpHeld;
    }

    void FixedUpdate()
    {
        if (input == null || stats == null) return;
        float dt = Time.fixedDeltaTime;

        grounded = Physics.Raycast(rb.position, Vector3.down, groundCheckDistance,
                                   groundMask, QueryTriggerInteraction.Ignore);

        UpdateDrift();

        Vector3 velocity = rb.linearVelocity; // en Unity 2022 o anterior usa rb.velocity
        float forwardSpeed = Vector3.Dot(velocity, transform.forward);

        if (grounded)
            velocity = GroundMovement(velocity, dt, out forwardSpeed);
        else
            rb.AddForce(Physics.gravity * (stats.gravityMultiplier - 1f), ForceMode.Acceleration);

        velocity = TryJump(velocity);

        rb.linearVelocity = velocity;
        ForwardSpeed = forwardSpeed;

        Steer(forwardSpeed, dt);
    }

    // ------------------------------------------------------- Aceleración y agarre

    Vector3 GroundMovement(Vector3 velocity, float dt, out float forwardSpeed)
    {
        forwardSpeed = Vector3.Dot(velocity, transform.forward);
        float lateralSpeed = Vector3.Dot(velocity, transform.right);

        float throttle = input.Throttle;
        float target = throttle >= 0f
            ? throttle * stats.maxForwardSpeed
            : throttle * stats.maxReverseSpeed;

        float rate;
        if (Mathf.Approximately(throttle, 0f))
        {
            rate = stats.coastDeceleration;
        }
        else if (Mathf.Abs(forwardSpeed) > 0.1f && Mathf.Sign(throttle) != Mathf.Sign(forwardSpeed))
        {
            rate = stats.brakeDeceleration;
        }
        else
        {
            // Curva de aceleración: fuerte al arrancar, suave cerca del máximo.
            float t = Mathf.Clamp01(Mathf.Abs(forwardSpeed) / stats.maxForwardSpeed);
            rate = stats.acceleration * stats.accelerationCurve.Evaluate(t);
        }

        forwardSpeed = Mathf.MoveTowards(forwardSpeed, target, rate * dt);

        // Menos agarre lateral = la carriola desliza (derrape).
        float grip = drifting ? stats.driftGrip : stats.lateralGrip;
        lateralSpeed = Mathf.MoveTowards(lateralSpeed, 0f, grip * dt);

        return transform.forward * forwardSpeed
             + transform.right * lateralSpeed
             + Vector3.up * velocity.y; // conserva caída/gravedad
    }

    // ---------------------------------------------------------------- Derrape

    void UpdateDrift()
    {
        bool wantsDrift = input.Drift;

        if (!drifting)
        {
            if (wantsDrift && grounded && ForwardSpeed >= stats.minDriftSpeed && Mathf.Abs(input.Steer) > 0.3f)
            {
                drifting = true;
                driftDirection = Mathf.Sign(input.Steer);
            }
        }
        else if (!wantsDrift || ForwardSpeed < stats.minDriftSpeed * 0.6f)
        {
            drifting = false;
        }
    }

    // ------------------------------------------------------------------ Salto

    Vector3 TryJump(Vector3 velocity)
    {
        if (jumpRequested && grounded && velocity.y <= 0.1f)
            velocity.y = stats.jumpVelocity;

        jumpRequested = false;
        return velocity;
    }

    // ------------------------------------------------------------------- Giro

    void Steer(float forwardSpeed, float dt)
    {
        float steer = input.Steer;

        // En derrape el giro queda "enganchado" a la dirección del derrape:
        // girar a favor cierra más la curva y girar en contra la abre.
        if (drifting)
        {
            float t = (steer * driftDirection + 1f) * 0.5f;
            steer = driftDirection * Mathf.Lerp(stats.driftMinSteer, stats.driftMaxSteer, t);
        }

        float speedFactor = Mathf.Clamp01(Mathf.Abs(forwardSpeed) / stats.fullTurnSpeed);
        float reverseSign = forwardSpeed >= 0f ? 1f : -1f;
        float yaw = steer * stats.turnSpeed * speedFactor * reverseSign * dt;

        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, yaw, 0f));
    }
}