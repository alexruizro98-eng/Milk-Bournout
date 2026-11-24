using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Temporary keyboard-driven test movement for the placeholder stroller.</summary>
[RequireComponent(typeof(Rigidbody))]
public sealed class SimpleStrollerDrive : MonoBehaviour
{
    [SerializeField, Min(0f)] private float acceleration = 14f;
    [SerializeField, Min(0f)] private float maxSpeed = 9f;
    [SerializeField, Min(0f)] private float turnSpeed = 95f;
    [SerializeField, Min(0f)] private float coastDrag = 2f;

    private Rigidbody body;
    private float throttle;
    private float steering;
    private bool braking;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        body.centerOfMass = new Vector3(0f, -0.35f, 0f);
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) { throttle = 0f; steering = 0f; braking = false; return; }

        throttle = (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed ? 1f : 0f)
                 - (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed ? 1f : 0f);
        steering = (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed ? 1f : 0f)
                 - (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? 1f : 0f);
        braking = keyboard.spaceKey.isPressed;
    }

    private void FixedUpdate()
    {
        if (throttle != 0f)
            body.AddForce(transform.forward * (throttle * acceleration), ForceMode.Acceleration);
        else
            body.AddForce(-body.linearVelocity * (braking ? coastDrag * 5f : coastDrag), ForceMode.Acceleration);

        Vector3 planarVelocity = Vector3.ProjectOnPlane(body.linearVelocity, Vector3.up);
        if (planarVelocity.magnitude > maxSpeed)
            body.linearVelocity = planarVelocity.normalized * maxSpeed + Vector3.up * body.linearVelocity.y;

        float direction = Mathf.Sign(Vector3.Dot(body.linearVelocity, transform.forward));
        float speedFactor = Mathf.Clamp01(planarVelocity.magnitude / 2f);
        float yaw = steering * turnSpeed * speedFactor * (throttle < 0f ? -1f : direction);
        body.MoveRotation(body.rotation * Quaternion.Euler(0f, yaw * Time.fixedDeltaTime, 0f));
    }
}
