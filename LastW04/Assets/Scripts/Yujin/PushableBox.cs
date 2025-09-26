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

    // ▼▼▼ 새로운 함수: 핸들의 위치를 받아 움직임을 결정합니다. ▼▼▼
    public void MoveWithHandle(Vector3 handlePosition)
    {
        // 핸들과 박스 사이의 거리 벡터를 계산합니다.
        Vector3 delta = handlePosition - transform.position;

        // 거리가 0.5칸(그리드의 절반)을 넘어섰을 때만 움직임을 시도합니다.
        if (delta.magnitude > 0.5f)
        {
            // 어느 방향으로 움직여야 할지 결정합니다 (상하좌우).
            Vector2 moveDirection;
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                moveDirection = new Vector2(Mathf.Sign(delta.x), 0);
            }
            else
            {
                moveDirection = new Vector2(0, Mathf.Sign(delta.y));
            }

            // 기존의 연쇄 밀기 로직을 실행합니다.
            SliderPush(moveDirection);
        }
    }

    // --- 기존 함수들은 private으로 변경하거나 유지 ---

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

    // 이제 외부가 아닌 MoveWithHandle 내부에서만 호출되므로 private으로 변경
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