// PushableBox.cs
using UnityEngine;

public class PushableBox : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleLayer; // 장애물 레이어 (벽 등)
    [SerializeField] private LayerMask lotusPadLayer; // 이동 가능한 연꽃잎 레이어
    [SerializeField] private LayerMask waterLayer;    // 물 레이어
    [SerializeField] private LayerMask boxLayer;      // ▼▼▼ 다른 상자를 감지하기 위해 이 줄을 추가했어요! ▼▼▼

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
    /// 목표 위치로 이동할 수 있는지 최종 확인하는 함수
    /// </summary>
    private bool CanMoveTo(Vector2 targetPos)
    {
        // 자신의 콜라이더를 잠시 비활성화하여 Raycast가 자신을 감지하지 않도록 함
        boxCollider.enabled = false;

        // 1. 목표 위치에 장애물(벽)이 있는지 확인
        if (Physics2D.OverlapCircle(targetPos, 0.4f, obstacleLayer))
        {
            boxCollider.enabled = true; // 반드시 다시 활성화!
            return false; // 장애물이 있으면 무조건 이동 불가
        }

        // ▼▼▼ 여기에 새로운 코드가 추가되었어요! ▼▼▼
        // 2. 목표 위치에 다른 상자가 있는지 확인
        if (Physics2D.OverlapCircle(targetPos, 0.4f, boxLayer))
        {
            boxCollider.enabled = true; // 반드시 다시 활성화!
            return false; // 다른 상자가 있으면 이동 불가
        }
        // ▲▲▲ 여기까지가 추가된 부분입니다 ▲▲▲

        // 3. 목표 위치에 "물"이 있는지 확인
        Collider2D waterHit = Physics2D.OverlapCircle(targetPos, 0.4f, waterLayer);
        if (waterHit != null)
        {
            // ▼▼▼ [수정됨] 연꽃 위에 있다면 물 위도 이동 가능하도록 변경 ▼▼▼
            if (IsOnLotus)
            {
                boxCollider.enabled = true;
                return true;
            }

            Collider2D lotusHit = Physics2D.OverlapCircle(targetPos, .5f, lotusPadLayer);
            boxCollider.enabled = true;
            return lotusHit != null;
        }

        // 4. 위 모든 조건에 해당하지 않으면(장애물도, 다른 상자도, 물도 아니면) 일반 땅이므로 이동 가능
        boxCollider.enabled = true; // 반드시 다시 활성화!
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
        // CanMoveTo 함수에서 사용하는 체크 반경과 동일하게 설정합니다.
        float checkRadius = 0.2f;

        // 임시로 콜라이더를 할당받기 위해 변수를 선언합니다.
        // Awake가 호출되지 않은 에디터 상태에서도 작동하게 하기 위함입니다.
        BoxCollider2D tempCollider = GetComponent<BoxCollider2D>();
        if (tempCollider == null) return; // 콜라이더가 없으면 중단

        // 확인할 네 방향을 정의합니다.
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

        // 각 방향에 대해 이동 가능 여부를 체크하고 기즈모를 그립니다.
        foreach (var dir in directions)
        {
            Vector2 targetPos = (Vector2)transform.position + dir;

            // CanMoveTo 함수를 직접 호출하여 결과를 확인합니다.
            // 이 때 실제 콜라이더를 잠시 비활성화해야 하므로 주의가 필요합니다.
            // (Gizmo 함수는 매 프레임 호출되므로 원래 상태를 기억해야 합니다)
            bool originalState = tempCollider.enabled;
            bool isMovable = CanMoveTo(targetPos);
            tempCollider.enabled = originalState; // 반드시 원래 상태로 복구

            if (isMovable)
            {
                Gizmos.color = Color.green; // 이동 가능하면 초록색
            }
            else
            {
                Gizmos.color = Color.red; // 이동 불가하면 빨간색
            }

            // 결과에 따라 해당 위치에 원 모양의 기즈모를 그립니다.
            Gizmos.DrawWireSphere(targetPos, checkRadius);
        }
    }
}