using UnityEngine;


/// Carriola arcade: adelante, atrás, giro, aceleración con curva, derrape y salto.
/// Lee un IStrollerInput y los valores de un StrollerStats.
/// 
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
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        grounded = Physics.Raycast(rb.position, Vector3.down, groundCheckDistance,
                                   groundMask, QueryTriggerInteraction.Ignore);

        UpdateDrift();

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
        float target = throttle >= 0f
            ? throttle * stats.maxForwardSpeed
            : throttle * stats.maxReverseSpeed;

        float rate;
        if (Mathf.Approximately(throttle, 0f))
            rate = stats.coastDeceleration;
        else if (Mathf.Abs(forwardSpeed) > 0.1f && Mathf.Sign(throttle) != Mathf.Sign(forwardSpeed))
            rate = stats.brakeDeceleration;
        else
            rate = stats.acceleration * stats.accelerationCurve.Evaluate(
                Mathf.Clamp01(Mathf.Abs(forwardSpeed) / stats.maxForwardSpeed));

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