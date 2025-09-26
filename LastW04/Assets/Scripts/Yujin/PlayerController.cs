// PlayerController.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 5f;               // 플레이어 이동 속도
    [SerializeField] private float interactionDistance = 1f;     // 상호작용 거리(보통 cellSize와 동일 또는 1.05배)
    [SerializeField] private LayerMask boxLayer;                 // 돌/박스가 속한 레이어 마스크

    [Header("Animation")]
    [SerializeField] private Animator anim;                      // 애니메이터
    [SerializeField, Range(0f, 0.3f)] private float deadZone = 0.05f;
    [SerializeField, Range(0f, 0.5f)] private float snapHysteresis = 0.12f;

    private Rigidbody2D rb;
    private PlayerControls playerControls;
    private Vector2 moveInput;
    private Vector2 lastDirection = Vector2.down;   // 마지막 바라보는 방향(입력 기준)
    private Vector2 lastCardinal = Vector2.down;    // 애니메이션용 스냅 방향

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
        // 이동은 물리 프레임에서 처리
        rb.linearVelocity = moveInput * moveSpeed;
    }

    private void Update()
    {
        // === 애니메이션 로직 (PlayerMove와 동일한 스냅 규칙) ===
        bool isMoving = moveInput.magnitude > deadZone;

        Vector2 cardinal = lastCardinal;
        if (isMoving)
        {
            float ax = Mathf.Abs(moveInput.x);
            float ay = Mathf.Abs(moveInput.y);

            if (ax >= ay + snapHysteresis)
                cardinal = new Vector2(Mathf.Sign(moveInput.x), 0f);
            else if (ay >= ax + snapHysteresis)
                cardinal = new Vector2(0f, Mathf.Sign(moveInput.y));

            lastCardinal = cardinal;         // 애니용 스냅 방향 저장
            lastDirection = cardinal;        // 상호작용도 스냅된 방향을 사용하고 싶으면 유지
        }

        if (anim != null)
        {
            anim.SetBool("isMoving", isMoving);
            anim.SetFloat("dirX", isMoving ? cardinal.x : 0f);
            anim.SetFloat("dirY", isMoving ? cardinal.y : 0f);
            anim.SetFloat("lastX", lastCardinal.x);
            anim.SetFloat("lastY", lastCardinal.y);
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        // 입력이 있을 때만 lastDirection 갱신 (대각 입력은 Update에서 스냅)
        if (moveInput.x != 0 || moveInput.y != 0)
            lastDirection = moveInput;
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        // 4방향 스냅한 방향(애니메이션 스냅 결과와 일치시키려면 lastCardinal 사용)
        Vector2 dir = (Mathf.Abs(lastDirection.x) > Mathf.Abs(lastDirection.y))
            ? new Vector2(Mathf.Sign(lastDirection.x), 0)
            : new Vector2(0, Mathf.Sign(lastDirection.y));

        RaycastHit2D hit = Physics2D.Raycast(rb.position, dir, interactionDistance, boxLayer);
        if (!hit) return;

        if (hit.collider.TryGetComponent(out SlidingStone2D stone))
        {
            if (!stone.IsSliding) stone.Push(dir);
            return;
        }

        if (hit.collider.TryGetComponent(out PushableBox box))
        {
            box.Push(dir);
        }
    }
}
