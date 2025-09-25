using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private LayerMask interactableLayers; // ��ȣ�ۿ��� ���̾�� (�ڽ�, �� ��)

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastDirection = Vector2.down; // ���������� �ٶ� ���� (�ʱⰪ�� �Ʒ�)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // �̵� �Է�
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;

        // �������� ���� ���� ������ ������ ����
        if (moveInput != Vector2.zero)
        {
            lastDirection = moveInput;
        }

        // 'E' Ű�� ������ �� ��ȣ�ۿ� �õ�
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    /// <summary>
    /// �ٶ󺸴� �������� ��ȣ�ۿ��� �õ��ϴ� �Լ�
    /// </summary>
    private void Interact()
    {
        // 1. �ٷ� �տ� �ڽ��� �ִ��� Ȯ��
        Vector2 rayOrigin = (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, lastDirection, 1f, interactableLayers);

        // ���𰡿� �ε�����, �װ��� 'Box' �±׸� �����ٸ�
        if (hit.collider != null && hit.collider.CompareTag("Box"))
        {
            Transform box = hit.transform;

            // 2. �ڽ��� ������ ���� ����ִ��� Ȯ��
            Vector2 targetPosition = (Vector2)box.position + lastDirection;
            Collider2D targetOverlap = Physics2D.OverlapCircle(targetPosition, 0.2f, interactableLayers);

            // Ÿ�� ��ġ�� �ƹ��͵� ���ٸ� �ڽ��� �̵���Ŵ
            if (targetOverlap == null)
            {
                // �׸��忡 ���� ��ġ�� ����ϰ� ����
                Vector3 finalPosition = new Vector3(Mathf.Round(targetPosition.x), Mathf.Round(targetPosition.y), 0);
                box.position = finalPosition;
            }
        }
    }
}