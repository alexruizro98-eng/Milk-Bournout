using UnityEngine;

/// <summary>Race camera that follows the stroller from behind and looks down the track.</summary>
public sealed class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform stroller;

    [Header("Race camera framing")]
    [Tooltip("Horizontal distance behind the stroller, in world units.")]
    [SerializeField, Min(0f)] private float distanceBehind = 14f;
    [Tooltip("Camera height above the stroller, in world units.")]
    [SerializeField] private float heightAboveStroller = 4.2f;
    [Tooltip("Height above the stroller used as the camera look target.")]
    [SerializeField] private float targetHeight = 2.2f;
    [Tooltip("How far ahead of the stroller the camera looks, in world units.")]
    [SerializeField, Min(0f)] private float lookAhead = 30f;

    [Header("Follow smoothing")]
    [SerializeField, Min(0f)] private float positionSmooth = 4.5f;
    [SerializeField, Min(0f)] private float rotationSmooth = 4.5f;

    [Header("Camera lens")]
    [SerializeField, Range(1f, 179f)] private float fieldOfView = 62f;

    private Camera raceCamera;

    private void Awake()
    {
        raceCamera = GetComponent<Camera>();
        ApplyFieldOfView();
    }

    private void OnValidate()
    {
        ApplyFieldOfView();
    }

    private void ApplyFieldOfView()
    {
        if (raceCamera == null) raceCamera = GetComponent<Camera>();
        if (raceCamera != null && !raceCamera.orthographic)
            raceCamera.fieldOfView = fieldOfView;
    }

    private void LateUpdate()
    {
        if (stroller == null) return;

        Quaternion heading = Quaternion.Euler(0f, stroller.eulerAngles.y, 0f);
        Vector3 desiredPosition = stroller.position
                                - heading * Vector3.forward * distanceBehind
                                + Vector3.up * heightAboveStroller;

        float positionBlend = 1f - Mathf.Exp(-positionSmooth * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, positionBlend);

        Vector3 lookTarget = stroller.position
                           + heading * Vector3.forward * lookAhead
                           + Vector3.up * targetHeight;
        Vector3 lookDirection = lookTarget - transform.position;
        if (lookDirection.sqrMagnitude < 0.0001f) return;

        Quaternion desiredRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        float rotationBlend = 1f - Mathf.Exp(-rotationSmooth * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationBlend);
    }
}
