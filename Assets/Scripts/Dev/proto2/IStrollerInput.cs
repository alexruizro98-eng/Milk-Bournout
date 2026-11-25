
public interface IStrollerInput
{
    /// Giro: -1 = izquierda, 0 = recto, 1 = derecha.
    float Steer { get; }

    /// Acelerador: 1 = adelante, 0 = suelto, -1 = reversa / freno.
    float Throttle { get; }

    /// mientras el botón de salto está apretado (mantener = salto más alto).
    bool Jump { get; }

    ///  mientras el botón de derrape está apretado.
    bool Drift { get; }
}