using UnityEngine;

[DisallowMultipleComponent]
public class ObjectHighlighter : MonoBehaviour
{
    [Header("Trigger Detection")]
    [SerializeField] private float radius = 1.6f;         // 감지 반경(트리거 콜라이더 크기)
    [SerializeField] private string playerTag = "Player"; // 플레이어 태그

    [Header("Outline Visual")]
    [SerializeField] private Color outlineColor = new Color(1f, 0.92f, 0.16f, 1f); // 노란 하이라이트
    [SerializeField, Range(1f, 1.3f)] private float outlineScale = 1.06f;          // 외곽선 두께(스케일)
    [SerializeField] private int orderOffset = 2;          // 원본보다 앞에 보이도록 정렬순서 오프셋
    [SerializeField] private bool pulse = false;           // 숨쉬기(펄스) 효과
    [SerializeField] private float pulseAmount = 0.02f;
    [SerializeField] private float pulseSpeed = 4f;

    private SpriteRenderer mainSR;     // 원본 스프라이트
    private SpriteRenderer outlineSR;  // 외곽선용 스프라이트(자식)
    private CircleCollider2D trigger;  // 감지용 트리거
    private bool visible = false;
    private Vector3 baseOutlineLocalScale;

    void Awake()
    {
        // 원본 스프라이트 찾기(자식 포함)
        mainSR = GetComponent<SpriteRenderer>();
        if (!mainSR) mainSR = GetComponentInChildren<SpriteRenderer>(includeInactive: true);

        SetupOutline();
        SetupTrigger();
    }

    void Start()
    {
        Show(false, force: true); // 시작 시 비활성
    }

    void LateUpdate()
    {
        // 애니메이션/플립/정렬 동기화
        if (outlineSR && mainSR)
        {
            if (outlineSR.sprite != mainSR.sprite) outlineSR.sprite = mainSR.sprite;
            if (outlineSR.flipX != mainSR.flipX) outlineSR.flipX = mainSR.flipX;
            if (outlineSR.flipY != mainSR.flipY) outlineSR.flipY = mainSR.flipY;

            if (outlineSR.sortingLayerID != mainSR.sortingLayerID)
                outlineSR.sortingLayerID = mainSR.sortingLayerID;

            int desiredOrder = mainSR.sortingOrder + orderOffset;
            if (outlineSR.sortingOrder != desiredOrder)
                outlineSR.sortingOrder = desiredOrder;
        }

        // 펄스(숨쉬기) 효과
        if (pulse && outlineSR && visible)
        {
            float s = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            outlineSR.transform.localScale = baseOutlineLocalScale * s;
        }
    }

    // 트리거 진입: 하이라이트 ON
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
            Show(true);
    }

    // 트리거 이탈: 하이라이트 OFF
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
            Show(false);
    }

    // 외부에서 강제로 켜고/끄고 싶을 때 호출 가능
    public void SetHighlight(bool on) => Show(on);

    private void Show(bool on, bool force = false)
    {
        if (!outlineSR) return;
        if (!force && visible == on) return;
        visible = on;
        outlineSR.enabled = on;
    }

    private void SetupOutline()
    {
        // 자식 오브젝트 "_Outline" 생성/준비
        var holder = transform.Find("_Outline");
        if (!holder)
        {
            var go = new GameObject("_Outline");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = Vector3.zero;
            holder = go.transform;
        }

        outlineSR = holder.GetComponent<SpriteRenderer>();
        if (!outlineSR) outlineSR = holder.gameObject.AddComponent<SpriteRenderer>();

        if (mainSR)
        {
            outlineSR.sprite = mainSR.sprite;
            outlineSR.sortingLayerID = mainSR.sortingLayerID;
            outlineSR.sortingOrder = mainSR.sortingOrder + orderOffset;
            outlineSR.flipX = mainSR.flipX;
            outlineSR.flipY = mainSR.flipY;
        }

        outlineSR.color = outlineColor;
        holder.localScale = Vector3.one * outlineScale;
        baseOutlineLocalScale = holder.localScale;

        outlineSR.enabled = false; // 기본 꺼두기
    }

    private void SetupTrigger()
    {
        // 원형 트리거 준비
        trigger = GetComponent<CircleCollider2D>();
        if (!trigger) trigger = gameObject.AddComponent<CircleCollider2D>();
        trigger.isTrigger = true;
        trigger.radius = radius;

        // 트리거 동작을 위해 한쪽에는 Rigidbody2D 필요 → 대상에 경량 리지드바디 부착
        var rb = GetComponent<Rigidbody2D>();
        if (!rb)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.isKinematic = true; // 물리 영향 없음
            rb.simulated = true;
            rb.gravityScale = 0f;
        }
    }

    void OnValidate()
    {
        // 인스펙터 값 변경 시 즉시 반영
        if (!outlineSR) return;

        outlineSR.color = outlineColor;
        outlineSR.sortingOrder = (mainSR ? mainSR.sortingOrder + orderOffset : orderOffset);
        outlineSR.transform.localScale = Vector3.one * outlineScale;
        baseOutlineLocalScale = outlineSR.transform.localScale;

        if (!trigger) trigger = GetComponent<CircleCollider2D>();
        if (trigger)
        {
            trigger.isTrigger = true;
            trigger.radius = radius;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.9f, 0.1f, 0.6f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
