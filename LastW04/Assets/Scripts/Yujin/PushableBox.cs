// PushableBox.cs
using UnityEngine;

public class PushableBox : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleLayer; // 장애물 레이어 (벽 등)
    [SerializeField] private LayerMask lotusPadLayer; // 이동 가능한 연꽃잎 레이어
    [SerializeField] private LayerMask waterLayer;    // ▼▼▼ "물" 레이어를 감지하기 위해 이 줄을 추가하세요! ▼▼▼

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
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
    /// 목표 위치로 이동할 수 있는지 최종 확인하는 함수
    /// </summary>
    private bool CanMoveTo(Vector2 targetPos)
    {
        // 자신의 콜라이더를 잠시 비활성화하여 Raycast가 자신을 감지하지 않도록 함
        boxCollider.enabled = false;

        // 1. 목표 위치에 장애물(벽)이 있는지 확인
        if (Physics2D.OverlapCircle(targetPos, 0.2f, obstacleLayer))
        {
            boxCollider.enabled = true; // 반드시 다시 활성화!
            return false; // 장애물이 있으면 무조건 이동 불가
        }

        // 2. 목표 위치에 "물"이 있는지 확인
        Collider2D waterHit = Physics2D.OverlapCircle(targetPos, 0.2f, waterLayer);
        if (waterHit != null)
        {
            // 물 위라면, 연꽃잎이 있는지 추가로 확인해야 함
            Collider2D lotusHit = Physics2D.OverlapCircle(targetPos, 0.2f, lotusPadLayer);
            boxCollider.enabled = true; // 반드시 다시 활성화!
            // lotusHit가 null이 아니면(연꽃잎이 있으면) true, 없으면 false 반환
            return lotusHit != null;
        }

        // 3. 위 모든 조건에 해당하지 않으면(장애물도 없고 물도 아니면) 일반 땅이므로 이동 가능
        boxCollider.enabled = true; // 반드시 다시 활성화!
        return true;
    }
}