using UnityEngine;

public class SeguirStroller : MonoBehaviour
{
    [SerializeField] private Transform stroller;
    [SerializeField] private float distanciaAtras = 8f;
    [SerializeField] private float altura = 5f;
    [SerializeField] private float suavidad = 5f;

    private float yawSuavizado;
    private Vector3 velocidad;

    private void Start()
    {
        if (stroller != null)
        {
            yawSuavizado = stroller.eulerAngles.y;
        }
    }

    private void LateUpdate()
    {
        if (stroller == null) return;

        
        yawSuavizado = Mathf.LerpAngle(yawSuavizado, stroller.eulerAngles.y, suavidad * Time.deltaTime);

        Quaternion giroSoloY = Quaternion.Euler(0, yawSuavizado, 0);

        Vector3 posicionDeseada = stroller.position - (giroSoloY * Vector3.forward * distanciaAtras) + Vector3.up * altura;

        transform.position = Vector3.SmoothDamp(transform.position, posicionDeseada, ref velocidad, 1f / suavidad);

       
        transform.LookAt(stroller.position + Vector3.up * 1f, Vector3.up);

    }
}