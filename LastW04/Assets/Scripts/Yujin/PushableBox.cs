// PushableBox.cs
using UnityEngine;

public class PushableBox : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleLayer; // 장애물 레이어 (벽 등)
    [SerializeField] private LayerMask lotusPadLayer; // 이동 가능한 연꽃잎 레이어
    [SerializeField] private LayerMask waterLayer;    // 물 레이어
    [SerializeField] private LayerMask boxLayer;      // ▼▼▼ 다른 상자를 감지하기 위해 이 줄을 추가했어요! ▼▼▼

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

        // ▼▼▼ 여기에 새로운 코드가 추가되었어요! ▼▼▼
        // 2. 목표 위치에 다른 상자가 있는지 확인
        if (Physics2D.OverlapCircle(targetPos, 0.2f, boxLayer))
        {
            boxCollider.enabled = true; // 반드시 다시 활성화!
            return false; // 다른 상자가 있으면 이동 불가
        }
        // ▲▲▲ 여기까지가 추가된 부분입니다 ▲▲▲

        // 3. 목표 위치에 "물"이 있는지 확인
        Collider2D waterHit = Physics2D.OverlapCircle(targetPos, 0.2f, waterLayer);
        if (waterHit != null)
        {
            // ▼▼▼ [수정됨] 연꽃 위에 있다면 물 위도 이동 가능하도록 변경 ▼▼▼
            if (IsOnLotus)
            {
                boxCollider.enabled = true;
                return true;
            }

            Collider2D lotusHit = Physics2D.OverlapCircle(targetPos, 0.2f, lotusPadLayer);
            boxCollider.enabled = true;
            return lotusHit != null;
        }

        // 4. 위 모든 조건에 해당하지 않으면(장애물도, 다른 상자도, 물도 아니면) 일반 땅이므로 이동 가능
        boxCollider.enabled = true; // 반드시 다시 활성화!
        return true;
    }
}