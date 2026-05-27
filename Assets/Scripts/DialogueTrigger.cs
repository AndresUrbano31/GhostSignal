using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject dialoguePanel;      // El panel de diálogo (DialoguePanel)
    public TextMeshProUGUI dialogueText;  // El texto dentro del panel (Text TMP)

    [Header("Contenido")]
    [TextArea] public string mensaje = "ARIA: Eso es VOID. La memoria de todo lo que la gente eligió no recordar. No te odia... solo sepulta lo que se acerca.";
    public float duracion = 5f;           // Segundos que se muestra el diálogo

    private bool yaActivado = false;       // Para que solo aparezca una vez

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Algo entró en la zona de diálogo: " + other.name + " (Tag: " + other.tag + ")");

        if (yaActivado) { Debug.Log("Pero ya estaba activado, ignoro."); return; }
        if (!other.CompareTag("Player")) { Debug.Log("No es el Player, ignoro."); return; }

        Debug.Log("¡Es el Player! Mostrando diálogo.");
        yaActivado = true;
        StartCoroutine(MostrarDialogo());
    }

    IEnumerator MostrarDialogo()
    {
        Debug.Log("PASO 1: Entré a la corrutina. dialoguePanel es null? " + (dialoguePanel == null));

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
            Debug.Log("PASO 2: Activé el panel. Está activo ahora? " + dialoguePanel.activeSelf);
        }
        else
        {
            Debug.Log("PASO 2 FALLÓ: dialoguePanel es NULL, no puedo activarlo.");
        }

        if (dialogueText != null)
        {
            dialogueText.text = mensaje;
            Debug.Log("PASO 3: Texto asignado: " + mensaje);
        }
        else
        {
            Debug.Log("PASO 3 FALLÓ: dialogueText es NULL.");
        }

        yield return new WaitForSeconds(duracion);

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }
}