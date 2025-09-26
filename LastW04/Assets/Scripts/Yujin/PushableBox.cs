// PushableBox.cs
using UnityEngine;
using System.Collections.Generic;

public class PushableBox : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask lotusPadLayer;
    [SerializeField] private LayerMask waterLayer;
    [SerializeField] private LayerMask boxLayer;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    // ���� ���ο� �Լ�: �ڵ��� ��ġ�� �޾� �������� �����մϴ�. ����
    public void MoveWithHandle(Vector3 handlePosition)
    {
        // �ڵ�� �ڽ� ������ �Ÿ� ���͸� ����մϴ�.
        Vector3 delta = handlePosition - transform.position;

        // �Ÿ��� 0.5ĭ(�׸����� ����)�� �Ѿ�� ���� �������� �õ��մϴ�.
        if (delta.magnitude > 0.5f)
        {
            // ��� �������� �������� ���� �����մϴ� (�����¿�).
            Vector2 moveDirection;
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                moveDirection = new Vector2(Mathf.Sign(delta.x), 0);
            }
            else
            {
                moveDirection = new Vector2(0, Mathf.Sign(delta.y));
            }

            // ������ ���� �б� ������ �����մϴ�.
            SliderPush(moveDirection);
        }
    }

    // --- ���� �Լ����� private���� �����ϰų� ���� ---

    public void PlayerPush(Vector2 direction)
    {
        Vector2 targetPos = (Vector2)transform.position + direction;
        LayerMask blockingLayers = obstacleLayer | boxLayer;
        if (Physics2D.OverlapCircle(targetPos, 0.2f, blockingLayers) ||
           (Physics2D.OverlapCircle(targetPos, 0.2f, waterLayer) && !Physics2D.OverlapCircle(targetPos, 0.2f, lotusPadLayer)))
        {
            return;
        }
        Move(direction);
    }

    // ���� �ܺΰ� �ƴ� MoveWithHandle ���ο����� ȣ��ǹǷ� private���� ����
    private void SliderPush(Vector2 direction)
    {
        List<PushableBox> pushChain = new List<PushableBox>();
        pushChain.Add(this);

        if (CanChainBePushed(direction, pushChain))
        {
            for (int i = pushChain.Count - 1; i >= 0; i--)
            {
                pushChain[i].Move(direction);
            }
        }
    }

    private bool CanChainBePushed(Vector2 direction, List<PushableBox> chain)
    {
        Vector2 targetPos = (Vector2)transform.position + direction;
        Collider2D hitCollider = Physics2D.OverlapCircle(targetPos, 0.2f, boxLayer);

        if (hitCollider != null)
        {
            if (hitCollider.TryGetComponent(out PushableBox nextBox))
            {
                if (chain.Contains(nextBox)) return false;
                chain.Add(nextBox);
                return nextBox.CanChainBePushed(direction, chain);
            }
        }

        if (Physics2D.OverlapCircle(targetPos, 0.2f, obstacleLayer) ||
           (Physics2D.OverlapCircle(targetPos, 0.2f, waterLayer) && !Physics2D.OverlapCircle(targetPos, 0.2f, lotusPadLayer)))
        {
            return false;
        }
        return true;
    }

    private void Move(Vector2 direction)
    {
        Vector2 newPos = (Vector2)transform.position + direction;
        transform.position = new Vector3(Mathf.Round(newPos.x), Mathf.Round(newPos.y), transform.position.z);
    }
}