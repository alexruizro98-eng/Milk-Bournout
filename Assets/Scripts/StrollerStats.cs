using System.IO.Enumeration;
using UnityEngine;
[CreateAssetMenu(fileName = "NuevaCarriola", menuName = "juego/Carriola Stats")]


public class StrollerStats : ScriptableObject
{
    [Header("Motor")]
    public float velocidadMax = 20f;
    public float aceleracion = 10f;
    public float brake = 15f;


    [Header("Giro")]
    public float velocidadGiro = 80f;
    public float agarre = 0.9f; // 1= no derrapa, 0 = derrapa mucho

    [Header("Bonus por baile")]
    public float bonusBaile = 5f;
}
