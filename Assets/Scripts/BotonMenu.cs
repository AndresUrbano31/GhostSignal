using UnityEngine;

public class BotonMenu : MonoBehaviour
{
    // Llama al GameManager que esté activo (el que sobrevive entre escenas)
    public void IrAlMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.VolverAlMenu();
        }
    }
}