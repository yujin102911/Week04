// PlayerController.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float interactionDistance = 1f;
    [SerializeField] private LayerMask boxLayer;
    [SerializeField] private LayerMask waterLayer;
    [SerializeField] private LayerMask lotusLayer;

    private Rigidbody2D rb;
    private PlayerControls playerControls;
    private Vector2 moveInput;
    private Vector2 lastDirection = Vector2.down;

    // ▼▼▼ [추가됨] 마지막 안전 위치를 저장할 변수 ▼▼▼
    private Vector2 lastSafePosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerControls = new PlayerControls();
        // ▼▼▼ [추가됨] 게임 시작 시 초기 위치를 안전 위치로 저장 ▼▼▼
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

    // ▼▼▼ [추가됨] 플레이어가 물에 끼었는지 확인하고 복구하는 로직 ▼▼▼
    private void Update()
    {
        // 현재 위치에 물이 있는지, 연꽃이 있는지 확인
        bool isInWater = Physics2D.OverlapCircle(rb.position, 0.4f, waterLayer) != null;
        bool isOnLotus = Physics2D.OverlapCircle(rb.position, 0.4f, lotusLayer) != null;

        // 만약 연꽃 위도 아닌데 물 속에 있다면 (끼인 상태)
        if (isInWater && !isOnLotus)
        {
            // 마지막으로 저장된 안전한 위치로 즉시 이동!
            transform.position = lastSafePosition;
            Debug.Log("플레이어가 물에 끼어 안전한 위치로 복귀합니다.");
        }
    }


    private void FixedUpdate()
    {
        Vector2 newPosition = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;

        Collider2D waterCollider = Physics2D.OverlapCircle(newPosition, 0.4f, waterLayer);
        Collider2D lotusCollider = Physics2D.OverlapCircle(newPosition, 0.4f, lotusLayer);

        if (waterCollider == null || lotusCollider != null)
        {
            rb.MovePosition(newPosition);
            // ▼▼▼ [추가됨] 성공적으로 이동했을 때만 안전 위치를 갱신 ▼▼▼
            lastSafePosition = rb.position;
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (moveInput.x != 0 || moveInput.y != 0)
        {
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

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        RaycastHit2D hit = Physics2D.Raycast(rb.position, lastDirection, interactionDistance, boxLayer);

        if (hit.collider != null)
        {
            if (hit.collider.TryGetComponent(out PushableBox box))
            {
                box.Push(lastDirection);
            }
        }
    }
}