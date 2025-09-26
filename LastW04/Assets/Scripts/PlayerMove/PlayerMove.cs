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
//    [SerializeField] private LayerMask interactableLayers;         // ��ȣ�ۿ� ���
//    [SerializeField] private LayerMask blockLayers;              // ���� ���� ���(��/�ڽ� ��)
//    [SerializeField, Min(0.05f)] private float blockCastDist = 0.6f;   // ���� ���� �Ÿ�
//    [SerializeField, Min(0.05f)] private float blockCircleRadius = 0.25f; // ��Ŭĳ��Ʈ ������
//    [SerializeField, Min(0f)] private float interactCooldown = 0.1f;

//    Vector2 input;
//    Vector2 lastCardinal = Vector2.down;
//    float lastInteractTime = -999f;

//    // �÷��� ���� �ִ��� ���θ� ��Ÿ���� public ����
//    // AttachPlayerToPlatform ��ũ��Ʈ�� �� ���� �����մϴ�.
//    public bool IsOnPlatform { get; set; } = false;

//    public void OnMove(InputValue value)
//    {
//        input = value.Get<Vector2>();
//        if (input.sqrMagnitude > 1f) input = input.normalized;
//    }

//    void Update()
//    {
//        // �÷��� ���� ���� ���� ���� ���� ���� ������ �����մϴ�.
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
//                // �Է� ���Ϳ��� ���� ���и� ����(��Ʈ�������� ����)
//                float f = Vector2.Dot(input, forward);
//                if (f > 0f) input -= forward * f;
//            }
//        }

//        // --- 2) �̵�/���� (���� ����) ---
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

//        // --- 3) �ִϸ����� ---
//        anim.SetBool("isMoving", isMoving);
//        anim.SetFloat("dirX", isMoving ? cardinal.x : 0f);
//        anim.SetFloat("dirY", isMoving ? cardinal.y : 0f);
//        anim.SetFloat("lastX", lastCardinal.x);
//        anim.SetFloat("lastY", lastCardinal.y);

//        // (�ӽ�) E Ű ��ȣ�ۿ� �׽�Ʈ
//        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
//        {
//            if (Time.time >= lastInteractTime + interactCooldown)
//            {
//                lastInteractTime = Time.time;
//                Interact();
//            }
//        }
//    }

//    /// <summary>�ٶ󺸴� �������� ��ȣ�ۿ�(�ڽ� �б�)</summary>
//    /// <summary>�ٶ󺸴� �������� ��ȣ�ۿ�(�� �̲����߸���/�ڽ� �б�)</summary>
//    private void Interact()
//    {
//        Vector2 origin = transform.position;
//        Vector2 dir = (lastCardinal == Vector2.zero) ? Vector2.down : lastCardinal;

//        // 1) ���� '��' �˻�
//        RaycastHit2D hit = Physics2D.Raycast(origin, dir, 1f, interactableLayers);
//        if (hit.collider != null)
//        {
//            Debug.Log($"[PlayerMove] Interact hit: {hit.collider.name}");
//            // SlidingStone2D�� �پ��ִٸ� '������' �̲����߸�
//            if (hit.collider.TryGetComponent(out SlidingStone2D stone))
//            {
//                // ���� �̹� �̲������� ���� �ƴϰ�, �÷��̾ �÷��� ���� �ƴ� ��츸(���Ѵٸ� ���� ���� ����)
//                if (!stone.IsSliding)
//                {
//                    stone.Slide(Vector2Int.RoundToInt(dir));
//                }
//                return;
//            }

//            // 2) ���� �ƴ϶�� ���� '�ڽ� �� ĭ �б�' ���� ����
//            if (hit.collider.CompareTag("Box"))
//            {
//                Transform box = hit.transform;
//                Vector2 target = (Vector2)box.position + dir;

//                // ��ȣ�ۿ� ���̾ ���� �����ϰ� ������ �� �а�(��, Ư�� �±״� ��� ����)
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


//    // �����: ��Ŭĳ��Ʈ �ð�ȭ
//    void OnDrawGizmosSelected()
//    {
//        if (!Application.isPlaying) return;
//        Vector3 o = transform.position;
//        Vector3 d = (Vector3)lastCardinal.normalized;
//        Gizmos.color = Color.cyan;
//        // ���� ��
//        DrawWireCircle(o, blockCircleRadius);
//        // �� ��
//        DrawWireCircle(o + d * blockCastDist, blockCircleRadius);
//        // ���ἱ
//        Gizmos.DrawLine(o, o + d * blockCastDist);
//    }

//    // ������ �� �׸���
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