using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class StrollerController : MonoBehaviour
{
    [Header("Datos")]
    [SerializeField] private StrollerStats stats;

    [Header("Referencias")]
    [SerializeField] private Transform modeloVisual;

    private Rigidbody rb;
    private float inputAceleracion;
    private float inputGiro;
    private float bonusActual;
    private float tiempoBonus;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
    }

    private void FixedUpdate()
    {
        Mover();
        Girar();
        ActualizarBonus();
    }

    public void SetInput(float giro, float aceleracion)
    {
        inputGiro = giro;
        inputAceleracion = aceleracion;
    }

    private void Mover()
    {
        float velocidadObjetivo = stats.velocidadMax + bonusActual;

        if (inputAceleracion > 0)
        {
            rb.AddForce(transform.forward * stats.aceleracion * inputAceleracion, ForceMode.Acceleration);
        }
        else if (inputAceleracion < 0)
        {
            rb.AddForce(transform.forward * stats.brake * inputAceleracion, ForceMode.Acceleration);
        }

        if (rb.linearVelocity.magnitude > velocidadObjetivo)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * velocidadObjetivo;
        }

        Vector3 velocidadLateral = Vector3.Dot(rb.linearVelocity, transform.right) * transform.right;
        rb.linearVelocity -= velocidadLateral * stats.agarre * Time.fixedDeltaTime * 5f;
    }

    private void Girar()
    {
        float factorVelocidad = Mathf.Clamp01(rb.linearVelocity.magnitude / 5f);
        float giro = inputGiro * stats.velocidadGiro * factorVelocidad * Time.fixedDeltaTime;

        transform.Rotate(0, giro, 0);

        if (modeloVisual != null)
        {
            float inclinacion = -inputGiro * 10f * factorVelocidad;
            modeloVisual.localRotation = Quaternion.Lerp(
                modeloVisual.localRotation,
                Quaternion.Euler(0, 0, inclinacion),
                Time.fixedDeltaTime * 8f
            );
        }
    }

    public void AplicarBonusBaile(float duracion)
    {
        bonusActual = stats.bonusBaile;
        tiempoBonus = duracion;
    }

    private void ActualizarBonus()
    {
        if (tiempoBonus > 0)
        {
            tiempoBonus -= Time.fixedDeltaTime;
            if (tiempoBonus <= 0)
                bonusActual = 0;
        }
    }

    public float VelocidadActual => rb.linearVelocity.magnitude;
    public float VelocidadMaxima => stats.velocidadMax + bonusActual;
}