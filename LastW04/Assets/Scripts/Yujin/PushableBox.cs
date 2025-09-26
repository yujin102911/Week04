// PushableBox.cs
using UnityEngine;

public class PushableBox : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleLayer; // ��ֹ� ���̾� (�� ��)
    [SerializeField] private LayerMask lotusPadLayer; // �̵� ������ ������ ���̾�
    [SerializeField] private LayerMask waterLayer;    // �� ���̾�
    [SerializeField] private LayerMask boxLayer;      // ���� �ٸ� ���ڸ� �����ϱ� ���� �� ���� �߰��߾��! ����

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb;
    public bool IsOnLotus { get; private set; } = false;


    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
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
                Mathf.Round(targetPosition.x),
                Mathf.Round(targetPosition.y),
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
        if (Physics2D.OverlapCircle(targetPos, 0.2f, obstacleLayer))
        {
            boxCollider.enabled = true; // �ݵ�� �ٽ� Ȱ��ȭ!
            return false; // ��ֹ��� ������ ������ �̵� �Ұ�
        }

        // ���� ���⿡ ���ο� �ڵ尡 �߰��Ǿ����! ����
        // 2. ��ǥ ��ġ�� �ٸ� ���ڰ� �ִ��� Ȯ��
        if (Physics2D.OverlapCircle(targetPos, 0.2f, boxLayer))
        {
            boxCollider.enabled = true; // �ݵ�� �ٽ� Ȱ��ȭ!
            return false; // �ٸ� ���ڰ� ������ �̵� �Ұ�
        }
        // ���� ��������� �߰��� �κ��Դϴ� ����

        // 3. ��ǥ ��ġ�� "��"�� �ִ��� Ȯ��
        Collider2D waterHit = Physics2D.OverlapCircle(targetPos, 0.2f, waterLayer);
        if (waterHit != null)
        {
            // ���� [������] ���� ���� �ִٸ� �� ���� �̵� �����ϵ��� ���� ����
            if (IsOnLotus)
            {
                boxCollider.enabled = true;
                return true;
            }

            Collider2D lotusHit = Physics2D.OverlapCircle(targetPos, 0.2f, lotusPadLayer);
            boxCollider.enabled = true;
            return lotusHit != null;
        }

        // 4. �� ��� ���ǿ� �ش����� ������(��ֹ���, �ٸ� ���ڵ�, ���� �ƴϸ�) �Ϲ� ���̹Ƿ� �̵� ����
        boxCollider.enabled = true; // �ݵ�� �ٽ� Ȱ��ȭ!
        return true;
    }
}