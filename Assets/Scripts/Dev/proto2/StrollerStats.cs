using UnityEngine;

/// <summary>
/// Valores de manejo de una carriola. Es un ScriptableObject: crea uno con
/// clic derecho > Create > Stroller > Stats, y puedes tener varios
/// (carriola normal, pesada, rápida...) sin tocar código.
/// </summary>
[CreateAssetMenu(fileName = "StrollerStats", menuName = "Stroller/Stats")]
public class StrollerStats : ScriptableObject
{
    [Header("Velocidad (m/s)")]
    public float maxForwardSpeed = 18f;
    public float maxReverseSpeed = 6f;

    [Header("Aceleración (m/s²)")]
    [Tooltip("Qué tan rápido gana velocidad al pisar el acelerador.")]
    public float acceleration = 14f;
    [Tooltip("Qué tan rápido frena al pedir el sentido contrario al que va.")]
    public float brakeDeceleration = 28f;
    [Tooltip("Qué tan rápido se detiene sola al soltar el acelerador.")]
    public float coastDeceleration = 6f;

    [Header("Giro")]
    [Tooltip("Grados por segundo con el volante a tope.")]
    public float turnSpeed = 110f;
    [Tooltip("Velocidad a partir de la cual el giro es completo. Por debajo gira menos (no gira parada).")]
    public float fullTurnSpeed = 6f;

    [Header("Agarre")]
    [Tooltip("Qué tan rápido se anula el deslizamiento lateral. Bajo = derrapa más, alto = sobre rieles.")]
    public float lateralGrip = 10f;
}
