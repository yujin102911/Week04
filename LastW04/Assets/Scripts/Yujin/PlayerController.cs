// PlayerController.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;               // 플레이어 이동 속도
    [SerializeField] private float interactionDistance = 1f;     // 상호작용 거리(보통 cellSize와 동일 또는 1.05배)
    [SerializeField] private LayerMask boxLayer;                 // 돌/박스가 속한 레이어 마스크

    private Rigidbody2D rb;
    private PlayerControls playerControls;
    private Vector2 moveInput;
    private Vector2 lastDirection = Vector2.down; // 마지막 바라보는 방향

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Player.Move.performed += OnMove;
        playerControls.Player.Move.canceled += OnMoveCanceled;
        playerControls.Player.Interact.performed += OnInteract;
        playerControls.Player.Enable();
    }

    private void OnDisable()
    {
        playerControls.Player.Move.performed -= OnMove;
        playerControls.Player.Move.canceled -= OnMoveCanceled;
        playerControls.Player.Interact.performed -= OnInteract;
        playerControls.Player.Disable();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed; // linearVelocity가 아닌 velocity 사용
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (moveInput.x != 0 || moveInput.y != 0)
        {
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
                lastDirection = new Vector2(Mathf.Sign(moveInput.x), 0);
            else
                lastDirection = new Vector2(0, Mathf.Sign(moveInput.y));
        }
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        // 4방향 스냅한 방향 벡터
        Vector2 dir = (Mathf.Abs(lastDirection.x) > Mathf.Abs(lastDirection.y))
            ? new Vector2(Mathf.Sign(lastDirection.x), 0)
            : new Vector2(0, Mathf.Sign(lastDirection.y));

        // 레이캐스트로 정면 한 칸 검사
        RaycastHit2D hit = Physics2D.Raycast(rb.position, dir, interactionDistance, boxLayer);
        if (!hit) return;

        // SlidingStone2D: 장애물에 닿기 전까지 미끄러지기
        if (hit.collider.TryGetComponent(out SlidingStone2D stone))
        {
            if (!stone.IsSliding) stone.Push(dir);
            return;
        }

        // (선택) PushableBox가 있다면 한 칸 밀기 유지
        if (hit.collider.TryGetComponent(out PushableBox box))
        {
            box.Push(dir);
        }
    }
}
