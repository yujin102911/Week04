// PlayerController.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;               // 플레이어 이동 속도
    [SerializeField] private float interactionDistance = 1f;     // 상호작용 거리
    [SerializeField] private LayerMask boxLayer;                 // 박스/돌 레이어

    private Rigidbody2D rb;
    private PlayerControls playerControls;
    private Vector2 moveInput;
    private Vector2 lastDirection = Vector2.down; // 마지막으로 바라본 방향 (기본값: 아래)

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
        // 최소 변경: linearVelocity 대신 velocity 사용
        rb.linearVelocity = moveInput * moveSpeed;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        // 움직임이 있을 때만 마지막 방향 갱신(대각선 입력 방지)
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
        // 최소 변경: lastDirection을 바로 dir로 사용(4방향 스냅)
        Vector2 dir = (Mathf.Abs(lastDirection.x) > Mathf.Abs(lastDirection.y))
            ? new Vector2(Mathf.Sign(lastDirection.x), 0)
            : new Vector2(0, Mathf.Sign(lastDirection.y));

        // 레이캐스트
        RaycastHit2D hit = Physics2D.Raycast(rb.position, dir, interactionDistance, boxLayer);
        if (!hit) return;

        if (hit.collider.TryGetComponent(out SlidingStone2D stone))
        {
            if (!stone.IsSliding) stone.Push(dir);
            return;
        }

        // (선택) 박스: 한 칸 밀기 — 사용 중일 때만 필요
        if (hit.collider.TryGetComponent(out PushableBox box))
        {
            box.Push(dir);
        }
    }
}
