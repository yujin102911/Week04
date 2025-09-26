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
    // PlayerMovement.cs

    private void Interact()
    {
        Vector2 rayOrigin = (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, lastDirection, 1f, interactableLayers);

        if (hit.collider != null && hit.collider.CompareTag("Box"))
        {
            Transform box = hit.transform;

            // ���� �� if ����� �����ϰų� �ּ� ó���մϴ�! ����
            /*
            // �ڵ鿡 �پ��ִ� �ڽ��� ���� ���� (������ ����)
            if (box.parent != null)
            {
                return;
            }
            */
            // ���� ������� ���� ����

            BoxCollider2D boxCollider = box.GetComponent<BoxCollider2D>();
            if (boxCollider == null) return;

            int originalLayer = box.gameObject.layer;
            box.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            RaycastHit2D castHit = Physics2D.BoxCast(
                (Vector2)box.position,
                boxCollider.size * 0.9f,
                0f,
                lastDirection,
                1f,
                interactableLayers
            );

            box.gameObject.layer = originalLayer;

            if (castHit.collider == null)
            {
                Vector2 targetPosition = (Vector2)box.position + lastDirection;
                Vector3 finalPosition = new Vector3(Mathf.Floor(targetPosition.x), Mathf.Floor(targetPosition.y), 0);
                box.GetComponent<Rigidbody2D>().MovePosition(finalPosition);

                // ���� �� ���� �߰��ϼ���! ����
                // �ڽ��� �� �Ŀ�, �ڵ���� �θ�-�ڽ� ���踦 ������ �����ݴϴ�.
                box.SetParent(null);
            }
        }
    }
}