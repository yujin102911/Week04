// PlayerController.cs (with Editing guard)
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float interactionDistance = 1f;
    [SerializeField] private LayerMask boxLayer;
    [SerializeField] private LayerMask waterLayer;
    [SerializeField] private LayerMask lotusLayer;

    [Header("Animation")]
    [SerializeField] private Animator anim;                      // Animator (isMoving, dirX, dirY, lastX, lastY)
    [SerializeField, Range(0f, 0.3f)] private float deadZone = 0.05f;
    [SerializeField, Range(0f, 0.5f)] private float snapHysteresis = 0.12f;

    [Header("Stuck Recovery")]
    [SerializeField, Tooltip("물에 빠졌다고 판단하기까지의 시간(초)")]
    private float unstuckThreshold = 0.1f;
    private float timeStuckInWater = 0f;

    private Rigidbody2D rb;
    private PlayerControls playerControls;
    private Vector2 moveInput;
    private Vector2 lastDirection = Vector2.down;   // 입력 기준 마지막 방향
    private Vector2 lastCardinal = Vector2.down;    // 애니/상호작용용 스냅 방향

    // 마지막 안전 위치 저장
    private Vector2 lastSafePosition;

    // --- Editing 모드 헬퍼 ---
    private bool IsEditing => GameManager.mode == Mode.Editing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerControls = new PlayerControls();
        lastSafePosition = transform.position;
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

    private void Update()
    {
        // --- EDITING: 모든 동작 정지(애니메이션도 Idle로 고정) ---
        if (IsEditing)
        {
            moveInput = Vector2.zero;
            if (anim != null)
            {
                anim.SetBool("isMoving", false);
                anim.SetFloat("dirX", 0f);
                anim.SetFloat("dirY", 0f);
                anim.SetFloat("lastX", lastCardinal.x);
                anim.SetFloat("lastY", lastCardinal.y);
            }
            return; // 물/연꽃 체크 및 나머지 로직 스킵
        }

        // --- 물에 끼임 복구 ---
        bool isInWater = Physics2D.OverlapCircle(rb.position, 0.4f, waterLayer) != null;
        bool isOnLotus = Physics2D.OverlapCircle(rb.position, 0.4f, lotusLayer) != null;

        bool isCurrentlySafe = !isInWater || isOnLotus;

        if (isCurrentlySafe)
        {
            // 1. 현재 위치가 안전하다면, 매 프레임 lastSafePosition을 갱신합니다.
            lastSafePosition = rb.position;
            timeStuckInWater = 0f; // 안전하므로 물에 빠진 시간 초기화
        }
        else
        {
            // 2. 현재 위치가 안전하지 않다면 (물 속이라면), 타이머를 작동시킵니다.
            timeStuckInWater += Time.deltaTime;
            if (timeStuckInWater > unstuckThreshold)
            {
                // 3. 유예 시간이 지나면, 마지막으로 안전했던 위치로 복귀시킵니다.
                transform.position = lastSafePosition;
                timeStuckInWater = 0f; // 타이머 리셋
            }
        }

        // --- 애니메이션/스냅 ---
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

            lastCardinal = cardinal;
            lastDirection = cardinal; // 상호작용 방향도 스냅과 일치
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

    private void FixedUpdate()
    {
        // --- EDITING: 물리 이동 정지 ---
        if (IsEditing)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // 충돌/물 체크 후 이동(성공 시 안전 위치 갱신)
        Vector2 newPosition = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;

        Collider2D waterCollider = Physics2D.OverlapCircle(newPosition, 0.4f, waterLayer);
        Collider2D lotusCollider = Physics2D.OverlapCircle(newPosition, 0.4f, lotusLayer);

        if (waterCollider == null || lotusCollider != null)
        {
            rb.MovePosition(newPosition);
            lastSafePosition = rb.position; // 이동 성공했을 때만 갱신
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (IsEditing) { moveInput = Vector2.zero; return; } // 입력 무시
        moveInput = context.ReadValue<Vector2>();
        // 입력이 있으면 우선 원시 입력으로 갱신(최종 스냅은 Update에서 처리)
        if (moveInput.x != 0 || moveInput.y != 0)
            lastDirection = moveInput;
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (IsEditing) return; // 상호작용 무시

        // 스냅된 4방향 사용
        Vector2 dir = (Mathf.Abs(lastDirection.x) > Mathf.Abs(lastDirection.y))
            ? new Vector2(Mathf.Sign(lastDirection.x), 0)
            : new Vector2(0, Mathf.Sign(lastDirection.y));

        RaycastHit2D hit = Physics2D.Raycast(rb.position, dir, interactionDistance, boxLayer);
        if (!hit) return;

        if (hit.collider.TryGetComponent(out SlidingStone2D stone))
        {
            if (!stone.IsSliding) stone.Push(dir); // 막힐 때까지 미끄럼
            return;
        }

        if (hit.collider.TryGetComponent(out PushableBox box))
        {
            box.Push(dir); // 한 칸 밀기(기존 동작 유지)
        }
    }
}
