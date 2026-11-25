/// <summary>
/// Contrato de entrada de una carriola. Lo implementan el jugador (PlayerInput)
/// y, más adelante, la IA, para que StrollerController no dependa de quién la maneja.
/// </summary>
public interface IStrollerInput
{
    /// <summary>Giro: -1 = izquierda, 0 = recto, 1 = derecha.</summary>
    float Steer { get; }

    /// <summary>Acelerador: 1 = adelante, 0 = suelto, -1 = reversa / freno.</summary>
    float Throttle { get; }
}
