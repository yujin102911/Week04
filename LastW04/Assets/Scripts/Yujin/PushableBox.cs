// PushableBox.cs
using UnityEngine;

public class PushableBox : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleLayer; // ��ֹ� ���̾� (�� ��)
    [SerializeField] private LayerMask lotusPadLayer; // �̵� ������ ������ ���̾�
    [SerializeField] private LayerMask waterLayer;    // �� ���̾�
    [SerializeField] private LayerMask boxLayer;      // ���� �ٸ� ���ڸ� �����ϱ� ���� �� ���� �߰��߾��! ����

    [Header("UI")]
    [SerializeField] private GameObject interactionPromptUI;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb;
    public bool IsOnLotus { get; private set; } = false;


    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        if (interactionPromptUI != null)
        {
            interactionPromptUI.SetActive(false);
        }
    }

    public void SetOnLotus(bool status)
    {
        IsOnLotus = status;
    }

    public void Push(Vector2 direction)
    {
        Vector2 targetPosition = (Vector2)transform.position + direction;

        if (CanMoveTo(targetPosition))
        {
            Vector3 finalPosition = new Vector3(
                Mathf.Floor(targetPosition.x) +.5f,
                Mathf.Floor(targetPosition.y) + .5f,
                transform.position.z
            );
            transform.position = finalPosition;
        }
    }

    /// <summary>
    /// ��ǥ ��ġ�� �̵��� �� �ִ��� ���� Ȯ���ϴ� �Լ�
    /// </summary>
    private bool CanMoveTo(Vector2 targetPos)
    {
        // �ڽ��� �ݶ��̴��� ��� ��Ȱ��ȭ�Ͽ� Raycast�� �ڽ��� �������� �ʵ��� ��
        boxCollider.enabled = false;

        // 1. ��ǥ ��ġ�� ��ֹ�(��)�� �ִ��� Ȯ��
        if (Physics2D.OverlapCircle(targetPos, 0.4f, obstacleLayer))
        {
            boxCollider.enabled = true; // �ݵ�� �ٽ� Ȱ��ȭ!
            return false; // ��ֹ��� ������ ������ �̵� �Ұ�
        }

        // ���� ���⿡ ���ο� �ڵ尡 �߰��Ǿ����! ����
        // 2. ��ǥ ��ġ�� �ٸ� ���ڰ� �ִ��� Ȯ��
        if (Physics2D.OverlapCircle(targetPos, 0.4f, boxLayer))
        {
            boxCollider.enabled = true; // �ݵ�� �ٽ� Ȱ��ȭ!
            return false; // �ٸ� ���ڰ� ������ �̵� �Ұ�
        }
        // ���� ��������� �߰��� �κ��Դϴ� ����

        // 3. ��ǥ ��ġ�� "��"�� �ִ��� Ȯ��
        Collider2D waterHit = Physics2D.OverlapCircle(targetPos, 0.4f, waterLayer);
        if (waterHit != null)
        {
            // ���� [������] ���� ���� �ִٸ� �� ���� �̵� �����ϵ��� ���� ����
            if (IsOnLotus)
            {
                boxCollider.enabled = true;
                return true;
            }

            Collider2D lotusHit = Physics2D.OverlapCircle(targetPos, .5f, lotusPadLayer);
            boxCollider.enabled = true;
            return lotusHit != null;
        }

        // 4. �� ��� ���ǿ� �ش����� ������(��ֹ���, �ٸ� ���ڵ�, ���� �ƴϸ�) �Ϲ� ���̹Ƿ� �̵� ����
        boxCollider.enabled = true; // �ݵ�� �ٽ� Ȱ��ȭ!
        return true;
    }
    public void ShowPrompt()
    {
        if (interactionPromptUI != null) interactionPromptUI.SetActive(true);
    }
    public void HidePrompt()
    {
        if (interactionPromptUI != null) interactionPromptUI.SetActive(false);
    }
    private void OnDrawGizmos()
    {
        // CanMoveTo �Լ����� ����ϴ� üũ �ݰ�� �����ϰ� �����մϴ�.
        float checkRadius = 0.2f;

        // �ӽ÷� �ݶ��̴��� �Ҵ�ޱ� ���� ������ �����մϴ�.
        // Awake�� ȣ����� ���� ������ ���¿����� �۵��ϰ� �ϱ� �����Դϴ�.
        BoxCollider2D tempCollider = GetComponent<BoxCollider2D>();
        if (tempCollider == null) return; // �ݶ��̴��� ������ �ߴ�

        // Ȯ���� �� ������ �����մϴ�.
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

        // �� ���⿡ ���� �̵� ���� ���θ� üũ�ϰ� ����� �׸��ϴ�.
        foreach (var dir in directions)
        {
            Vector2 targetPos = (Vector2)transform.position + dir;

            // CanMoveTo �Լ��� ���� ȣ���Ͽ� ����� Ȯ���մϴ�.
            // �� �� ���� �ݶ��̴��� ��� ��Ȱ��ȭ�ؾ� �ϹǷ� ���ǰ� �ʿ��մϴ�.
            // (Gizmo �Լ��� �� ������ ȣ��ǹǷ� ���� ���¸� ����ؾ� �մϴ�)
            bool originalState = tempCollider.enabled;
            bool isMovable = CanMoveTo(targetPos);
            tempCollider.enabled = originalState; // �ݵ�� ���� ���·� ����

            if (isMovable)
            {
                Gizmos.color = Color.green; // �̵� �����ϸ� �ʷϻ�
            }
            else
            {
                Gizmos.color = Color.red; // �̵� �Ұ��ϸ� ������
            }

            // ����� ���� �ش� ��ġ�� �� ����� ����� �׸��ϴ�.
            Gizmos.DrawWireSphere(targetPos, checkRadius);
        }
    }
}