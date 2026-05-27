using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; 

public class MenuController : MonoBehaviour
{
    [Header("Configuración de Audio")]
    public AudioSource audioBoton;

    [Header("Interfaz de Usuario")]
    // Esta es la variable que creará la casilla en el Inspector
    public GameObject panelControles; 

    private void Update()
    {
        // Detecta si se presiona la tecla ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Verifica si el panel existe y si está visible en pantalla
            if (panelControles != null && panelControles.activeInHierarchy)
            {
                // Apaga el panel
                panelControles.SetActive(false);
                
                // Reproduce el sonido de clic al cerrar (opcional)
                if (audioBoton != null) 
                {
                    audioBoton.Play(); 
                }
            }
        }
    }

    public void IniciarConexion(string nombreNivel)
    {
        if (audioBoton != null) 
        {
            audioBoton.Play();
        }
        StartCoroutine(CargarNivelConRetraso(nombreNivel));
    }

    private IEnumerator CargarNivelConRetraso(string escena)
    {
        yield return new WaitForSeconds(0.4f);
        SceneManager.LoadScene(escena);
    }

    public void SalirDelJuego()
    {
        if (audioBoton != null) 
        {
            audioBoton.Play();
        }
        Application.Quit();
        Debug.Log("¡Conexión terminada! El juego se cerraría aquí.");
    }
}