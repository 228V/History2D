using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Настройки патрулирования")]
    public Transform[] patrolPoints; // Массив точек патрулирования
    public float patrolSpeed = 2f;    // Скорость при патрулировании
    public float reachDistance = 0.1f; // Дистанция для смены точки

    [Header("Настройки преследования")]
    public float chaseSpeed = 4f;     // Скорость при преследовании
    public float chaseDistance = 5f;  // Дистанция обнаружения игрока
    public float stopDistance = 1f;   // Дистанция остановки возле игрока

    private Transform player;
    private int currentPointIndex = 0;
    private bool isChasing = false;
    private Vector3 originalScale;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        originalScale = transform.localScale;

        // Проверка на наличие точек патрулирования
        if (patrolPoints.Length == 0)
            Debug.LogError("Не назначены точки патрулирования!");
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
        if (patrolPoints.Length == 0) return;

        // Движение к текущей точке
        Transform target = patrolPoints[currentPointIndex];
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            patrolSpeed * Time.deltaTime
        );

        // Проверка достижения точки
        if (Vector3.Distance(transform.position, target.position) < reachDistance)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }

    void ChasePlayer()
    {
        // Проверка дистанции для остановки
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
        // Определение направления движения
        Vector3 direction = isChasing ?
            (player.position - transform.position) :
            (patrolPoints[currentPointIndex].position - transform.position);

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

    // Визуализация зоны обнаружения в редакторе
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }
}
