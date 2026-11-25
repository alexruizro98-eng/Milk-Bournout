using UnityEngine;

[CreateAssetMenu(fileName = "StrollerStats", menuName = "Stroller/Stats")]
public class StrollerStats : ScriptableObject
{
    [Header("Velocidad (m/s)")]
    public float maxForwardSpeed = 18f;
    public float maxReverseSpeed = 6f;

    [Header("Aceleración (m/s²)")]
    [Tooltip("Aceleración base. Se multiplica por la curva de abajo.")]
    public float acceleration = 12f;
    [Tooltip("X = velocidad actual / velocidad máxima (0..1). Y = multiplicador de aceleración. " +
             "Arranque fuerte y cada vez más suave al acercarse al máximo.")]
    public AnimationCurve accelerationCurve = new AnimationCurve(
        new Keyframe(0f, 1.6f), new Keyframe(0.7f, 1f), new Keyframe(1f, 0.25f));
    [Tooltip("Frenado al pedir el sentido contrario al que va.")]
    public float brakeDeceleration = 28f;
    [Tooltip("Frenado natural al soltar el acelerador.")]
    public float coastDeceleration = 6f;

    [Header("Giro")]
    [Tooltip("Grados por segundo con el volante a tope.")]
    public float turnSpeed = 110f;
    [Tooltip("Velocidad a partir de la cual el giro es completo. Por debajo gira menos (no gira parada).")]
    public float fullTurnSpeed = 6f;

    [Header("Agarre")]
    [Tooltip("Qué tan rápido se anula el deslizamiento lateral. Bajo = derrapa, alto = sobre rieles.")]
    public float lateralGrip = 10f;

    [Header("Derrape")]
    [Tooltip("Velocidad mínima para poder iniciar un derrape.")]
    public float minDriftSpeed = 8f;
    [Tooltip("Agarre lateral durante el derrape (bajo = desliza mucho).")]
    public float driftGrip = 1.5f;
    [Tooltip("Giro mínimo durante el derrape, cuando el jugador gira CONTRA la curva (0..1).")]
    public float driftMinSteer = 0.5f;
    [Tooltip("Giro máximo durante el derrape, cuando el jugador gira A FAVOR de la curva.")]
    public float driftMaxSteer = 1.3f;

    [Header("Salto")]
    [Tooltip("Velocidad vertical inicial del salto (m/s).")]
    public float jumpVelocity = 9f;
    [Tooltip("Gravedad extra en el aire (1 = normal). Más alto = salto más seco y menos flotante.")]
    public float gravityMultiplier = 2f;
}