using UnityEngine;

public class MemoryObstacle : MonoBehaviour
{
    public enum ObstacleState { Patrolling, Chasing, Stunned }
    public ObstacleState currentState = ObstacleState.Patrolling;

    [Header("Patrol Settings")]
    public float patrolSpeed = 2f;
    public float patrolDistance = 3f;

    [Header("Chase Settings")]
    public float chaseSpeed = 4f;
    public float chaseRange = 5f;

    [Header("Stun Settings")]
    public float stunDuration = 1f;

    private Vector3 startPosition;
    private float patrolDirection = 1f;
    private float stunTimer = 0f;
    private Transform player;

    void Start()
    {
        startPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        switch (currentState)
        {
            case ObstacleState.Patrolling:
                Patrol();
                DetectPlayer();
                break;
            case ObstacleState.Chasing:
                Chase();
                break;
            case ObstacleState.Stunned:
                stunTimer -= Time.deltaTime;
                if (stunTimer <= 0f)
                    currentState = ObstacleState.Patrolling;
                break;
        }
    }

    void Patrol()
    {
        transform.Translate(Vector2.right * patrolDirection * patrolSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, startPosition) >= patrolDistance)
            patrolDirection *= -1f;
    }

    void DetectPlayer()
    {
        if (player == null) return;
        if (Vector3.Distance(transform.position, player.position) < chaseRange)
            currentState = ObstacleState.Chasing;
    }

    void Chase()
    {
        if (player == null) return;
        transform.position = Vector2.MoveTowards(
            transform.position, player.position, chaseSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, player.position) > chaseRange * 1.5f)
            currentState = ObstacleState.Patrolling;
    }

    public void GetStunned()
    {
        currentState = ObstacleState.Stunned;
        stunTimer = stunDuration;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // El "chivato" para ver en consola qué está tocando
        Debug.Log("El cubo tocó a: " + other.gameObject.name + " que tiene el Tag: " + other.tag);

        if (!other.CompareTag("Player")) return;

        // Verificar invencibilidad antes de aplicar daño
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null && !health.TryTakeDamage()) return;

        GameManager.Instance.PlayerDied();
        GetStunned();
    }

}