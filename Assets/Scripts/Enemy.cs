using UnityEngine;


public class EnemyAI : MonoBehaviour
{
    [Header("Настройки патрулирования")]
    public Vector2[] patrolCoordinates; // Координаты X/Y из инспектора
    public float patrolSpeed = 2f;      // Скорость движения
    public float reachDistance = 0.1f; // Дистанция для смены точки

    [Header("Настройки преследования")]
    public float chaseSpeed = 4f;       // Скорость преследования
    public float chaseDistance = 5f;   // Дистанция обнаружения игрока
    public float stopDistance = 1f;     // Дистанция остановки

    private Transform player;
    private int currentPointIndex = 0;
    private bool isChasing = false;
    private Vector3 originalScale;
    private Vector3 targetPosition; // Хранение целевой позиции

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        originalScale = transform.localScale;

        if (patrolCoordinates.Length == 0)
        {
            Debug.LogError("Не назначены координаты патрулирования!");
            enabled = false;
        }
    }

    void Update()
    {
        if (isChasing)
            ChasePlayer();
        else
            Patrol();

        FlipSprite();
    }

    void Patrol()
    {
        if (patrolCoordinates.Length == 0) return;

        // Обновление целевой позиции только при необходимости
        if (targetPosition == Vector3.zero ||
            currentPointIndex >= patrolCoordinates.Length)
        {
            currentPointIndex = Mathf.Clamp(currentPointIndex, 0, patrolCoordinates.Length - 1);
            Vector2 target = patrolCoordinates[currentPointIndex];
            targetPosition = new Vector3(target.x, target.y, transform.position.z);
        }

        // Плавное перемещение
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            patrolSpeed * Time.deltaTime
        );

        // Проверка достижения точки
        if (Vector3.Distance(transform.position, targetPosition) < reachDistance)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolCoordinates.Length;
            targetPosition = Vector3.zero; // Сброс целевой позиции
        }
    }

    void ChasePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) > stopDistance)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                chaseSpeed * Time.deltaTime
            );
        }
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
            direction = targetPosition - transform.position;
        }

        // Поворот спрайта
        if (direction.x > 0)
            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
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




