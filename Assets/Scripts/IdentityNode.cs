using UnityEngine;

public class IdentityNode : MonoBehaviour
{
    [Header("Node Settings")]
    public float floatSpeed = 1.5f;
    public float floatHeight = 0.3f;
    public float rotateSpeed = 90f;

    private Vector3 startPosition;
    private bool isCollected = false; // Seguro antibloqueo

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Flotación
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Rotación
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Si ya fue recolectado, ignoramos cualquier otro choque en el mismo frame
        if (!isCollected && other.CompareTag("Player"))
        {
            isCollected = true;
            GameManager.Instance.CollectNode();
            Destroy(gameObject);
        }
    }
}