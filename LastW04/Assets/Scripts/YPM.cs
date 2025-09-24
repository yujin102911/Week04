using UnityEngine;

// �� ��ũ��Ʈ�� Rigidbody 2D ������Ʈ�� �ݵ�� �ʿ��մϴ�.
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    // [SerializeField]�� ����ϸ� private �������� �ν����� â���� ���� ������ �� �ֽ��ϴ�.
    [SerializeField]
    private float moveSpeed = 5f; // �÷��̾��� �̵� �ӵ�

    private Rigidbody2D rb; // Rigidbody 2D ������Ʈ�� ���� ����
    private Vector2 moveInput; // ������� �Է�(WASD) ���� ������ ����

    // ������ ���۵� �� �� �� ȣ��˴ϴ�.
    void Start()
    {
        // �� ��ũ��Ʈ�� �پ��ִ� ���� ������Ʈ�� Rigidbody 2D ������Ʈ�� �����ͼ� rb ������ �Ҵ��մϴ�.
        rb = GetComponent<Rigidbody2D>();
    }

    // �� �����Ӹ��� ȣ��˴ϴ�. �Է� ���� �ޱ⿡ �����մϴ�.
    void Update()
    {
        // ����(A, D, ��, ��) �� ����(W, S, ��, ��) �Է� ���� �޽��ϴ�.
        // �Է� ���� -1.0f���� 1.0f ������ ���� �����ϴ�.
        float moveX = Input.GetAxisRaw("Horizontal"); // Horizontal ��
        float moveY = Input.GetAxisRaw("Vertical"); // Vertical ��

        // �Է� ���� Vector2 ���·� �����ϰ�, ����ȭ(Normalize)�Ͽ� �밢�� �̵� �� �ӵ��� �������� ���� �����մϴ�.
        moveInput = new Vector2(moveX, moveY).normalized;
    }

    // ������ �ð� �������� ȣ��˴ϴ�. ���� ����� �ϱ⿡ �����մϴ�.
    void FixedUpdate()
    {
        // Rigidbody�� �ӵ�(velocity)�� �����Ͽ� �÷��̾ �����Դϴ�.
        // moveInput ���� �̵� �ӵ�(moveSpeed)�� ���� �ð�(Time.fixedDeltaTime)�� �����ݴϴ�.
        // Time.fixedDeltaTime�� �����ָ� ������ �ӵ��� ������� ������ �ӵ��� �����Դϴ�.
        rb.linearVelocity = moveInput * moveSpeed;
    }
}