//using UnityEngine;
//using UnityEngine.InputSystem;

//public class PlayerMove : MonoBehaviour
//{
//    [SerializeField, Min(0f)] float moveSpeed = 5f;
//    [SerializeField] Animator anim;

//    [Header("Filters")]
//    [SerializeField, Range(0f, 0.3f)] float deadZone = 0.05f;
//    [SerializeField, Range(0f, 0.5f)] float snapHysteresis = 0.12f;

//    [Header("Interact / Block")]
//    [SerializeField] private LayerMask interactableLayers;         // 상호작용 대상
//    [SerializeField] private LayerMask blockLayers;              // 전진 차단 대상(벽/박스 등)
//    [SerializeField, Min(0.05f)] private float blockCastDist = 0.6f;   // 정면 차단 거리
//    [SerializeField, Min(0.05f)] private float blockCircleRadius = 0.25f; // 서클캐스트 반지름
//    [SerializeField, Min(0f)] private float interactCooldown = 0.1f;

//    Vector2 input;
//    Vector2 lastCardinal = Vector2.down;
//    float lastInteractTime = -999f;

//    // 플랫폼 위에 있는지 여부를 나타내는 public 변수
//    // AttachPlayerToPlatform 스크립트가 이 값을 변경합니다.
//    public bool IsOnPlatform { get; set; } = false;

//    public void OnMove(InputValue value)
//    {
//        input = value.Get<Vector2>();
//        if (input.sqrMagnitude > 1f) input = input.normalized;
//    }

//    void Update()
//    {
//        // 플랫폼 위에 있지 않을 때만 정면 차단 로직을 실행합니다.
//        if (!IsOnPlatform)
//        {
//            Vector2 forward = (lastCardinal == Vector2.zero) ? Vector2.down : lastCardinal;
//            RaycastHit2D blockHit = Physics2D.CircleCast(
//                origin: (Vector2)transform.position,
//                radius: blockCircleRadius,
//                direction: forward,
//                distance: blockCastDist,
//                layerMask: blockLayers
//            );

//            if (blockHit.collider != null)
//            {
//                // 입력 벡터에서 정면 성분만 제거(스트레이프는 유지)
//                float f = Vector2.Dot(input, forward);
//                if (f > 0f) input -= forward * f;
//            }
//        }

//        // --- 2) 이동/스냅 (이하 동일) ---
//        bool isMoving = input.magnitude > deadZone;

//        Vector2 cardinal = lastCardinal;
//        if (isMoving)
//        {
//            float ax = Mathf.Abs(input.x);
//            float ay = Mathf.Abs(input.y);
//            if (ax >= ay + snapHysteresis) cardinal = new Vector2(Mathf.Sign(input.x), 0f);
//            else if (ay >= ax + snapHysteresis) cardinal = new Vector2(0f, Mathf.Sign(input.y));
//            lastCardinal = cardinal;

//            Vector3 delta = new Vector3(input.x, input.y, 0f) * moveSpeed * Time.deltaTime;
//            transform.Translate(delta, Space.World);
//        }

//        // --- 3) 애니메이터 ---
//        anim.SetBool("isMoving", isMoving);
//        anim.SetFloat("dirX", isMoving ? cardinal.x : 0f);
//        anim.SetFloat("dirY", isMoving ? cardinal.y : 0f);
//        anim.SetFloat("lastX", lastCardinal.x);
//        anim.SetFloat("lastY", lastCardinal.y);

//        // (임시) E 키 상호작용 테스트
//        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
//        {
//            if (Time.time >= lastInteractTime + interactCooldown)
//            {
//                lastInteractTime = Time.time;
//                Interact();
//            }
//        }
//    }

//    /// <summary>바라보는 방향으로 상호작용(박스 밀기)</summary>
//    /// <summary>바라보는 방향으로 상호작용(돌 미끄러뜨리기/박스 밀기)</summary>
//    private void Interact()
//    {
//        Vector2 origin = transform.position;
//        Vector2 dir = (lastCardinal == Vector2.zero) ? Vector2.down : lastCardinal;

//        // 1) 먼저 '돌' 검사
//        RaycastHit2D hit = Physics2D.Raycast(origin, dir, 1f, interactableLayers);
//        if (hit.collider != null)
//        {
//            Debug.Log($"[PlayerMove] Interact hit: {hit.collider.name}");
//            // SlidingStone2D가 붙어있다면 '끝까지' 미끄러뜨림
//            if (hit.collider.TryGetComponent(out SlidingStone2D stone))
//            {
//                // 돌이 이미 미끄러지는 중이 아니고, 플레이어도 플랫폼 위가 아닌 경우만(원한다면 조건 제거 가능)
//                if (!stone.IsSliding)
//                {
//                    stone.Slide(Vector2Int.RoundToInt(dir));
//                }
//                return;
//            }

//            // 2) 돌이 아니라면 기존 '박스 한 칸 밀기' 로직 유지
//            if (hit.collider.CompareTag("Box"))
//            {
//                Transform box = hit.transform;
//                Vector2 target = (Vector2)box.position + dir;

//                // 상호작용 레이어에 뭔가 차지하고 있으면 못 밀게(단, 특정 태그는 허용 예시)
//                Collider2D blocked = Physics2D.OverlapCircle(target, 0.2f, interactableLayers);
//                if (blocked == null || blocked.CompareTag("Lotus"))
//                {
//                    Vector3 finalPosition = new Vector3(Mathf.Round(target.x), Mathf.Round(target.y), 0);
//                    box.position = finalPosition;
//                    // anim.SetTrigger("push");
//                }
//                return;
//            }
//        }
//    }


//    // 디버그: 서클캐스트 시각화
//    void OnDrawGizmosSelected()
//    {
//        if (!Application.isPlaying) return;
//        Vector3 o = transform.position;
//        Vector3 d = (Vector3)lastCardinal.normalized;
//        Gizmos.color = Color.cyan;
//        // 시작 원
//        DrawWireCircle(o, blockCircleRadius);
//        // 끝 원
//        DrawWireCircle(o + d * blockCastDist, blockCircleRadius);
//        // 연결선
//        Gizmos.DrawLine(o, o + d * blockCastDist);
//    }

//    // 간단한 원 그리기
//    void DrawWireCircle(Vector3 center, float radius, int seg = 24)
//    {
//        Vector3 prev = center + new Vector3(radius, 0f, 0f);
//        float step = Mathf.PI * 2f / seg;
//        for (int i = 1; i <= seg; i++)
//        {
//            float a = i * step;
//            Vector3 cur = center + new Vector3(Mathf.Cos(a) * radius, Mathf.Sin(a) * radius, 0f);
//            Gizmos.DrawLine(prev, cur);
//            prev = cur;
//        }
//    }
//}