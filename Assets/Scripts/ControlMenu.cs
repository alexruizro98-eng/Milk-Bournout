using UnityEngine.SceneManagement;
using UnityEngine;

public class ControlMenu : MonoBehaviour
{
    public GameObject panelPrincipal;
    public GameObject panelOpciones;
    public UnityEngine.UI.Slider sliderVolumen;
    public AudioSource musicaFondo;
    public GameObject panelCreditos;

    public void AbrirOpciones()
    {
        panelPrincipal.SetActive(false);
        panelOpciones.SetActive(true);
    }

    public void RegresarMenu()
    {
        panelOpciones.SetActive(false);
        panelPrincipal.SetActive(true);
    }

    public void AbrirCreditos()
{
    panelPrincipal.SetActive(false);
    panelCreditos.SetActive(true);
}

public void CerrarCreditos()
{
    panelCreditos.SetActive(false);
    panelPrincipal.SetActive(true);
}
     // Cambiar volumen de música
    public void CambiarVolumen()
    {
        musicaFondo.volume = sliderVolumen.value;
    }
}

