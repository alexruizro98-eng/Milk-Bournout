
using UnityEngine;
using UnityEngine.UI;

public class ControlVolumen : MonoBehaviour
{
    public AudioSource musicaFondo;
    public Slider sliderMusica;

    void Start()
    {
        sliderMusica.onValueChanged.AddListener(CambiarVolumen);
    }

    public void CambiarVolumen(float volumen)
    {
        if (musicaFondo != null)
        {
            musicaFondo.volume = volumen;
        }
    }
}
