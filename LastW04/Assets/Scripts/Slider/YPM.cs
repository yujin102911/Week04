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
    // PlayerMovement.cs

    private void Interact()
    {
        Vector2 rayOrigin = (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, lastDirection, 1f, interactableLayers);

        if (hit.collider != null && hit.collider.CompareTag("Box"))
        {
            Transform box = hit.transform;

            // ▼▼▼ 이 if 블록을 삭제하거나 주석 처리합니다! ▼▼▼
            /*
            // 핸들에 붙어있는 박스는 밀지 않음 (기존과 동일)
            if (box.parent != null)
            {
                return;
            }
            */
            // ▲▲▲ 여기까지 삭제 ▲▲▲

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

                // ▼▼▼ 이 줄을 추가하세요! ▼▼▼
                // 박스를 민 후에, 핸들과의 부모-자식 관계를 강제로 끊어줍니다.
                box.SetParent(null);
            }
        }
    }
}