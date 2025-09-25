using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private LayerMask interactableLayers; // 상호작용할 레이어들 (박스, 벽 등)

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastDirection = Vector2.down; // 마지막으로 바라본 방향 (초기값은 아래)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 이동 입력
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;

        // 움직임이 있을 때만 마지막 방향을 저장
        if (moveInput != Vector2.zero)
        {
            lastDirection = moveInput;
        }

        // 'E' 키를 눌렀을 때 상호작용 시도
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
    /// 바라보는 방향으로 상호작용을 시도하는 함수
    /// </summary>
    private void Interact()
    {
        // 1. 바로 앞에 박스가 있는지 확인
        Vector2 rayOrigin = (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, lastDirection, 1f, interactableLayers);

        // 무언가에 부딪혔고, 그것이 'Box' 태그를 가졌다면
        if (hit.collider != null && hit.collider.CompareTag("Box"))
        {
            Transform box = hit.transform;

            // 2. 박스가 움직일 곳이 비어있는지 확인
            Vector2 targetPosition = (Vector2)box.position + lastDirection;
            Collider2D targetOverlap = Physics2D.OverlapCircle(targetPosition, 0.2f, interactableLayers);

            // 타겟 위치에 아무것도 없다면 박스를 이동시킴
            if (targetOverlap == null)
            {
                // 그리드에 맞춰 위치를 깔끔하게 보정
                Vector3 finalPosition = new Vector3(Mathf.Round(targetPosition.x), Mathf.Round(targetPosition.y), 0);
                box.position = finalPosition;
            }
        }
    }
}