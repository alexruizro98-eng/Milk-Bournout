using UnityEngine;


// Devuelve la carriola a su posición inicial en tres casos:
// 1) El jugador pulsa R.
// 2) Se atora: acelera pero casi no se mueve durante varios segundos.
// 3) Se cae del mapa.

[RequireComponent(typeof(Rigidbody))]
public class StrollerRespawn : MonoBehaviour
{
    [Header("Atorado")]
    [Tooltip("Por debajo de esta velocidad (m/s) se considera detenida.")]
    [SerializeField] float stuckSpeed = 0.5f;
    [Tooltip("Segundos acelerando sin moverse antes de reaparecer.")]
    [SerializeField] float stuckTime = 3f;

    [Header("Caída")]
    [Tooltip("Metros por debajo de la altura inicial a los que se considera que cayó del mapa.")]
    [SerializeField] float fallDistance = 20f;

    Rigidbody rb;
    IStrollerInput input;
    StrollerController controller;

    Vector3 startPosition;
    Quaternion startRotation;
    bool respawnRequested;
    bool prevRespawnHeld;
    float stuckTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        input = GetComponent<IStrollerInput>();
        controller = GetComponent<StrollerController>();

        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    
    void Update()
    {
        bool held = input.Respawn;
        if (held && !prevRespawnHeld) respawnRequested = true;
        prevRespawnHeld = held;
    }

    void FixedUpdate()
    {
        if (respawnRequested || HasFallen() || IsStuck(Time.fixedDeltaTime))
            Respawn();
    }

    bool HasFallen()
    {
        return transform.position.y < startPosition.y - fallDistance;
    }

    bool IsStuck(float dt)
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0f;

        bool tryingToMove = Mathf.Abs(input.Throttle) > 0.1f;
        if (tryingToMove && velocity.magnitude < stuckSpeed)
            stuckTimer += dt;
        else
            stuckTimer = 0f;

        return stuckTimer >= stuckTime;
    }

    void Respawn()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.position = startPosition;
        rb.rotation = startRotation;

        controller.ResetMotion();

        respawnRequested = false;
        stuckTimer = 0f;
    }
}
