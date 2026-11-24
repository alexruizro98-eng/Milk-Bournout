using UnityEngine;

[CreateAssetMenu(fileName = "Carriola_Nueva", menuName = "Juego / Carriola Stats")]
public class StrollerStats : ScriptableObject
{
    [Header("Movimiento")]
    public float velocidadMaxima = 22f;
    public float aceleracion = 12f;
    public float frenado = 18f;
    public float velocidadMaximaReversa = 8f;

    [Header("Giro / Derrape")]
    public float velocidadGiro = 170f;
    public float multiplicadorDerrape = 2.3f;
}