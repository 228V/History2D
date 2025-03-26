using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Настройки патрулирования")]
    public Vector2[] patrolCoordinates;
    public float patrolSpeed = 2f;
    public float reachDistance = 0.1f;

    [Header("Настройки преследования")]
    public float chaseSpeed = 4f;
    public float chaseDistance = 5f;
    public float stopDistance = 1f;

    [Header("Настройки простоя")]
    public float idleTime = 2f;
    public float idleInterval = 10f;

    private Transform player;
    private int currentPointIndex = 0;
    private bool isChasing = false;
    private bool isIdle = false;
    private float idleTimer = 0f;
    private float nextIdleTime;
    private Vector3 originalScale;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        originalScale = transform.localScale;
        nextIdleTime = Time.time + idleInterval;
    }

    void Update()
    {
        if (isIdle)
            HandleIdleState();
        else
        {
            if (isChasing)
                ChasePlayer();
            else
                Patrol();
        }

        FlipSprite();
    }

    void Patrol()
    {
        if (patrolCoordinates.Length == 0) return;

        // Проверка на простой
        if (Time.time >= nextIdleTime && !isIdle)
        {
            StartIdle();
            nextIdleTime = Time.time + idleInterval;
        }

        // Движение к текущей координате
        Vector2 target = patrolCoordinates[currentPointIndex];
        Vector3 targetPosition = new Vector3(target.x, target.y, transform.position.z);

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            patrolSpeed * Time.deltaTime
        );

        // Проверка достижения точки
        if (Vector3.Distance(transform.position, targetPosition) < reachDistance)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolCoordinates.Length;
        }
    }

    void ChasePlayer()
    {
        // Проверка дистанции
        if (Vector3.Distance(transform.position, player.position) > stopDistance)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                chaseSpeed * Time.deltaTime
            );
        }
        else
        {
            isChasing = false; // Останавливаем преследование при достижении игрока
        }
    }

    void HandleIdleState()
    {
        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0)
        {
            isIdle = false;
        }
    }

    void StartIdle()
    {
        isIdle = true;
        idleTimer = idleTime;
    }

    void FlipSprite()
    {
        Vector3 direction;
        if (isChasing)
        {
            direction = player.position - transform.position;
        }
        else
        {
            Vector2 target = patrolCoordinates[currentPointIndex];
            direction = new Vector3(target.x, target.y, transform.position.z) - transform.position;
        }

        // Поворот спрайта
        if (direction.x > 0)
            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
    }

    // Триггеры для смены состояния
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isIdle)
        {
            isChasing = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isChasing = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }
}
