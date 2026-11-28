using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ControlPausa : MonoBehaviour
{
    public GameObject menuPausa;
    public GameObject menuMusica;

   void Update()
{
    if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
    {
        if (menuMusica.activeSelf)
            return;

        if (menuPausa.activeSelf)
            Reanudar();
        else
            Pausar();
    }
}

    public void Pausar()
    {
        menuPausa.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Reanudar()
    {
        menuPausa.SetActive(false);
        Time.timeScale = 1f;
    }

    public void MenuPrincipal()
{
    Time.timeScale = 1f;
    SceneManager.LoadScene("UI");
}

    public void Reiniciar()
{
    Time.timeScale = 1f;
    UnityEngine.SceneManagement.SceneManager.LoadScene(
        UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
    );
}

public void AbrirMusica()
{
    menuPausa.SetActive(false);
    menuMusica.SetActive(true);
}

public void CerrarMusica()
{
    menuMusica.SetActive(false);
    menuPausa.SetActive(true);
}
}