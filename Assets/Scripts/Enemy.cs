using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("��������� ��������������")]
    public Transform[] patrolPoints; // ������ ����� ��������������
    public float patrolSpeed = 2f;    // �������� ��� ��������������
    public float reachDistance = 0.1f; // ��������� ��� ����� �����

    [Header("��������� �������������")]
    public float chaseSpeed = 4f;     // �������� ��� �������������
    public float chaseDistance = 5f;  // ��������� ����������� ������
    public float stopDistance = 1f;   // ��������� ��������� ����� ������

    private Transform player;
    private int currentPointIndex = 0;
    private bool isChasing = false;
    private Vector3 originalScale;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        originalScale = transform.localScale;

        // �������� �� ������� ����� ��������������
        if (patrolPoints.Length == 0)
            Debug.LogError("�� ��������� ����� ��������������!");
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

        // �������� � ������� �����
        Transform target = patrolPoints[currentPointIndex];
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            patrolSpeed * Time.deltaTime
        );

        // �������� ���������� �����
        if (Vector3.Distance(transform.position, target.position) < reachDistance)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }

    void ChasePlayer()
    {
        // �������� ��������� ��� ���������
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
        // ����������� ����������� ��������
        Vector3 direction = isChasing ?
            (player.position - transform.position) :
            (patrolPoints[currentPointIndex].position - transform.position);

        // ������� �������
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

    // ������������ ���� ����������� � ���������
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }
}
