using UnityEngine;

/// <summary>
/// Mueve la carriola con física arcade: adelante, atrás y giro izquierda/derecha.
/// Lee un IStrollerInput (jugador o IA) y los valores de un StrollerStats.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class StrollerController : MonoBehaviour
{
    [SerializeField] StrollerStats stats;

    Rigidbody rb;
    IStrollerInput input;

    /// <summary>Velocidad con signo en el eje de avance (negativa = reversa).</summary>
    public float ForwardSpeed { get; private set; }

    /// <summary>0..1 respecto a la velocidad máxima de avance. Útil para cámara, audio y HUD.</summary>
    public float NormalizedSpeed => stats != null
        ? Mathf.Clamp01(Mathf.Abs(ForwardSpeed) / stats.maxForwardSpeed)
        : 0f;

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

    void FixedUpdate()
    {
        if (input == null || stats == null) return;

        float dt = Time.fixedDeltaTime;
        Vector3 velocity = rb.linearVelocity; // en Unity 2022 o anterior usa rb.velocity

        float forwardSpeed = Vector3.Dot(velocity, transform.forward);
        float lateralSpeed = Vector3.Dot(velocity, transform.right);

        // --- Avance / reversa ---
        float throttle = input.Throttle;
        float targetSpeed = throttle >= 0f
            ? throttle * stats.maxForwardSpeed
            : throttle * stats.maxReverseSpeed;

        float rate;
        if (Mathf.Approximately(throttle, 0f))
            rate = stats.coastDeceleration;
        else if (Mathf.Abs(forwardSpeed) > 0.1f && Mathf.Sign(throttle) != Mathf.Sign(forwardSpeed))
            rate = stats.brakeDeceleration;
        else
            rate = stats.acceleration;

        forwardSpeed = Mathf.MoveTowards(forwardSpeed, targetSpeed, rate * dt);

        // --- Agarre: reduce el deslizamiento lateral ---
        lateralSpeed = Mathf.MoveTowards(lateralSpeed, 0f, stats.lateralGrip * dt);

        rb.linearVelocity = transform.forward * forwardSpeed
                          + transform.right * lateralSpeed
                          + Vector3.up * velocity.y; // conserva la caída/gravedad

        // --- Giro: proporcional a la velocidad, y se invierte al ir en reversa ---
        float speedFactor = Mathf.Clamp01(Mathf.Abs(forwardSpeed) / stats.fullTurnSpeed);
        float direction = forwardSpeed >= 0f ? 1f : -1f;
        float yaw = input.Steer * stats.turnSpeed * speedFactor * direction * dt;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, yaw, 0f));

        ForwardSpeed = forwardSpeed;
    }
}
