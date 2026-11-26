using UnityEngine;

// Carriola arcade: adelante, atrás, giro, aceleración con curva, derrape y salto.

[RequireComponent(typeof(Rigidbody))]
public class StrollerController : MonoBehaviour
{
    [SerializeField] StrollerStats stats;

    [Header("Suelo")]
    [SerializeField] LayerMask groundMask = ~0;
    [Tooltip("Distancia del pivote al suelo + un margen.")]
    [SerializeField] float groundCheckDistance = 0.6f;

    Rigidbody rb;
    IStrollerInput input;

    float forwardSpeed;
    bool grounded;
    bool jumpRequested;
    bool prevJumpHeld;
    bool drifting;
    float driftDirection;
    bool turboRequested;
    bool prevTurboHeld;
    float turboTimer;
    float turboCooldownTimer;
    float turboAccessTimer;

    //true mientras el jugador puede usar el turbo (recogió el item y no se acabó el tiempo).
    public bool HasTurboAccess => turboAccessTimer > 0f;

    /// Segundos restantes de permiso de turbo. Útil para el HUD.
    public float TurboAccessRemaining => Mathf.Max(0f, turboAccessTimer);

    //Lo llama el item: habilita la tecla de turbo durante turboAccessDuration segundos.
    public void GrantTurbo()
    {
        turboAccessTimer = stats.turboAccessDuration;
    }

    public void ResetMotion()
    {
        forwardSpeed = 0f;
        drifting = false;
        turboTimer = 0f;
        jumpRequested = false;
    }


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        input = GetComponent<IStrollerInput>();
    }

    // El salto se lee en Update para no perder pulsaciones cortas entre pasos de física.
    void Update()
    {
        bool jumpHeld = input.Jump;
        if (jumpHeld && !prevJumpHeld) jumpRequested = true;
        prevJumpHeld = jumpHeld;

        bool turboHeld = input.Turbo;
        if (turboHeld && !prevTurboHeld) turboRequested = true;
        prevTurboHeld = turboHeld;
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        grounded = Physics.Raycast(rb.position, Vector3.down, groundCheckDistance,
                                   groundMask, QueryTriggerInteraction.Ignore);

        UpdateDrift();
        UpdateTurbo(dt);

        Vector3 velocity = rb.linearVelocity;

        if (grounded)
            velocity = GroundMovement(velocity, dt);
        else
            rb.AddForce(Physics.gravity * (stats.gravityMultiplier - 1f), ForceMode.Acceleration);

        if (jumpRequested && grounded && velocity.y <= 0.1f)
            velocity.y = stats.jumpVelocity;
        jumpRequested = false;

        rb.linearVelocity = velocity;
        Steer();
    }

    // Aceleración con curva y agarre lateral.
    Vector3 GroundMovement(Vector3 velocity, float dt)
    {
        forwardSpeed = Vector3.Dot(velocity, transform.forward);
        float lateralSpeed = Vector3.Dot(velocity, transform.right);

        float throttle = input.Throttle;
        bool turbo = turboTimer > 0f && throttle >= 0f;
        float maxForward = stats.maxForwardSpeed * (turbo ? stats.turboMultiplier : 1f);

        float target = throttle >= 0f
            ? throttle * maxForward
            : throttle * stats.maxReverseSpeed;
        if (turbo) target = maxForward; // el turbo empuja aunque no acelere

        float rate;
        if (turbo)
            rate = stats.turboAcceleration;
        else if (Mathf.Approximately(throttle, 0f))
            rate = stats.coastDeceleration;
        else if (Mathf.Abs(forwardSpeed) > 0.1f && Mathf.Sign(throttle) != Mathf.Sign(forwardSpeed))
            rate = stats.brakeDeceleration;
        else
            rate = stats.acceleration * stats.accelerationCurve.Evaluate(
                Mathf.Clamp01(Mathf.Abs(forwardSpeed) / maxForward));

        forwardSpeed = Mathf.MoveTowards(forwardSpeed, target, rate * dt);

        float grip = drifting ? stats.driftGrip : stats.lateralGrip;
        lateralSpeed = Mathf.MoveTowards(lateralSpeed, 0f, grip * dt);

        return transform.forward * forwardSpeed
             + transform.right * lateralSpeed
             + Vector3.up * velocity.y;
    }

    void UpdateDrift()
    {
        bool wantsDrift = input.Drift;

        if (!drifting)
        {
            if (wantsDrift && grounded && forwardSpeed >= stats.minDriftSpeed && Mathf.Abs(input.Steer) > 0.3f)
            {
                drifting = true;
                driftDirection = Mathf.Sign(input.Steer);
            }
        }
        else if (!wantsDrift || forwardSpeed < stats.minDriftSpeed * 0.6f)
        {
            drifting = false;
        }
    }

    void UpdateTurbo(float dt)
    {
        turboTimer -= dt;
        turboCooldownTimer -= dt;
        turboAccessTimer -= dt;

        // Q solo funciona si el jugador tiene el permiso del item y ya pasó la espera entre turbos.
        if (turboRequested && HasTurboAccess && turboCooldownTimer <= 0f)
        {
            turboTimer = stats.turboDuration;
            turboCooldownTimer = stats.turboCooldown;
        }
        turboRequested = false;
    }

    void Steer()
    {
        float steer = input.Steer;

        // En derrape, girar a favor cierra la curva y girar en contra la abre.
        if (drifting)
        {
            float t = (steer * driftDirection + 1f) * 0.5f;
            steer = driftDirection * Mathf.Lerp(stats.driftMinSteer, stats.driftMaxSteer, t);
        }

        float speedFactor = Mathf.Clamp01(Mathf.Abs(forwardSpeed) / stats.fullTurnSpeed);
        float reverseSign = forwardSpeed >= 0f ? 1f : -1f;
        float yawRate = steer * stats.turnSpeed * speedFactor * reverseSign; // grados/seg

        rb.angularVelocity = Vector3.up * (yawRate * Mathf.Deg2Rad);
    }
}