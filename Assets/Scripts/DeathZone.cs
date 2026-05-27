using UnityEngine;

public class DeathZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // Esto imprimirá un mensaje blanco en la consola si CUALQUIER cosa toca la red
        Debug.Log("ALGO TOCÓ LA RED: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("¡ES KAI! Ejecutando muerte...");
            GameManager.Instance.PlayerDied();
        }
    }
}