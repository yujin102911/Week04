// PlayerController.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;      // 플레이어 이동 속도
    [SerializeField] private float interactionDistance = 1f; // 상호작용 거리
    [SerializeField] private LayerMask boxLayer;        // 박스 레이어를 특정하기 위함

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
        // Move 액션에 대한 이벤트 구독
        playerControls.Player.Move.performed += OnMove;
        playerControls.Player.Move.canceled += OnMoveCanceled;
        // Interact 액션에 대한 이벤트 구독
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

    // FixedUpdate는 물리 기반 이동에 더 적합합니다.
    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    // Move 입력이 들어왔을 때 호출될 함수
    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        // 움직임이 있을 때만 마지막 방향을 갱신 (대각선 입력 방지)
        if (moveInput.x != 0 || moveInput.y != 0)
        {
            // 수평, 수직 중 더 큰 값으로 방향을 결정
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            {
                lastDirection = new Vector2(Mathf.Sign(moveInput.x), 0);
            }
            else
            {
                lastDirection = new Vector2(0, Mathf.Sign(moveInput.y));
            }
        }
    }

    // Move 입력이 끝났을 때 호출될 함수
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    // Interact(E키) 입력이 들어왔을 때 호출될 함수
    private void OnInteract(InputAction.CallbackContext context)
    {
        // 플레이어가 바라보는 방향으로 Raycast 발사
        RaycastHit2D hit = Physics2D.Raycast(rb.position, lastDirection, interactionDistance, boxLayer);

        // Raycast에 무언가 감지되었다면
        if (hit.collider != null)
        {
            // 감지된 오브젝트가 PushableBox 컴포넌트를 가지고 있는지 확인
            if (hit.collider.TryGetComponent(out PushableBox box))
            {
                // 박스의 Push 함수 호출
                box.PlayerPush(lastDirection);
            }
        }
    }
}