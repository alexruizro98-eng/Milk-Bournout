using UnityEngine;

public class StrollerController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private StrollerStats stats;
    [SerializeField] private Transform modeloVisual;

    private Rigidbody rb;
    private float inputGiro;
    private float inputAcelerar;
    private bool estaDerrapando;

    private float giroSuavizado;
    private float giroMantenido;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
    }

    public void SetInput(float giro, float acelerar)
    {
        inputGiro = giro;
        inputAcelerar = acelerar;
    }

    public void SetDerrape(bool derrapar)
    {
        estaDerrapando = derrapar;
    }

    private void FixedUpdate()
    {
        Mover(inputAcelerar);
        Girar(inputGiro);
        InclinarModelo();
    }

    private void Mover(float acelerar)
    {
        if (acelerar > 0)
        {
            rb.AddForce(transform.forward * stats.aceleracion * acelerar, ForceMode.Acceleration);
        }
        else if (acelerar < 0)
        {
            rb.AddForce(transform.forward * stats.frenado * acelerar, ForceMode.Acceleration);
        }

        if (rb.linearVelocity.magnitude > stats.velocidadMaxima)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * stats.velocidadMaxima;
        }

        // Agarre: si derrapas patinas, si no, vas derecho
        Vector3 velocidadLocal = transform.InverseTransformDirection(rb.linearVelocity);
        float agarre = estaDerrapando ? 0.985f : 0.88f;
        velocidadLocal.x *= agarre;
        rb.linearVelocity = transform.TransformDirection(velocidadLocal);
    }

    private void Girar(float giro)
    {
        if (giro != 0)
        {
            giroMantenido = giro;
        }
        else if (inputAcelerar > 0.1f)
        {
            giroMantenido *= 0.98f;
        }
        else
        {
            giroMantenido = Mathf.MoveTowards(giroMantenido, 0, Time.fixedDeltaTime * 2f);
        }

        giroSuavizado = Mathf.MoveTowards(giroSuavizado, giroMantenido, Time.fixedDeltaTime / 0.12f);

        float velocidadActual = rb.linearVelocity.magnitude;
        float factorVelocidad = Mathf.Clamp01(velocidadActual / stats.velocidadMaxima);

        float multiplicadorGiro;
        if (estaDerrapando && velocidadActual > 5f)
        {
            multiplicadorGiro = stats.multiplicadorDerrape;
        }
        else
        {
            multiplicadorGiro = Mathf.Lerp(1.2f, 0.8f, factorVelocidad);
        }

        float giroFinal = giroSuavizado * stats.velocidadGiro * multiplicadorGiro;

        if (velocidadActual > 0.5f)
        {
            transform.Rotate(0, giroFinal * Time.fixedDeltaTime, 0);
        }
    }

    private void InclinarModelo()
    {
        if (modeloVisual == null) return;
        float inclinacion = -giroSuavizado * 12f;
        if (estaDerrapando) inclinacion *= 1.8f;
        modeloVisual.localRotation = Quaternion.Lerp(modeloVisual.localRotation, Quaternion.Euler(0, 0, inclinacion), Time.fixedDeltaTime * 8f);
    }
}