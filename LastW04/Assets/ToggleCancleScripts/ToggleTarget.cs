using UnityEngine;

public class ToggleTarget : MonoBehaviour
{
    [Header("Target (끄고/켜고/상태 바꿀 대상)")]
    [SerializeField] private GameObject target;   // 기존: 제어할 대상

    [Header("Optional Components (스프라이트/콜라이더 전환용)")]
    [SerializeField] private Collider2D solid;         // 대상의 Collider2D (미할당 시 자동 탐색)
    [SerializeField] private SpriteRenderer sr;        // 대상의 SpriteRenderer (미할당 시 자동 탐색)

    [Header("Sprites (문 등 상태 전환용)")]
    [SerializeField] private Sprite openSprite;        // 열렸을 때
    [SerializeField] private Sprite closedSprite;      // 닫혔을 때

    [Header("State")]
    [SerializeField] private bool isOn = true;         // true = 열림(또는 보임)

    [Header("On/Off Mode")]
    [Tooltip("오브젝트 자체를 껐다 켜고 싶으면 true (이때 target.SetActive 또는 대체 로직 사용)")]
    [SerializeField] private bool toggleGameObject = false;

    [Header("Options")]
    [Tooltip("열릴 때 solid 콜라이더를 끌지 여부(문은 보통 끄지만, 클릭을 계속 받으려면 false)")]
    [SerializeField] private bool disableSolidOnOpen = true; // ★ 추가

    public bool IsOn => isOn;

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
        AutoCacheComponents();
    }

    public void SetState(bool on)
    {
        isOn = on;

        // 대상 없으면 자기 자신 대상으로
        if (target == null) target = gameObject;

        // 1) 대상 오브젝트 자체를 껐다 켜는 모드
        if (toggleGameObject)
        {
            // target이 자기 자신이 아니면: 그대로 SetActive
            if (target != gameObject)
            {
                if (target.activeSelf != isOn)
                    target.SetActive(isOn);
                return; // ★ 반드시 반환
            }

        }

        // 2) 콜라이더/스프라이트만 전환하는 모드 (기존)
        if (solid == null || sr == null)
        {
            AutoCacheComponents();
        }

        // ★ 클릭을 계속 받아야 한다면 disableSolidOnOpen=false로 두면 됨
        if (solid && disableSolidOnOpen)
            solid.enabled = !isOn; // 열리면 충돌 off

        if (sr)
            sr.sprite = isOn ? openSprite : closedSprite; // 스프라이트 교체
    }

    public void Toggle() => SetState(!isOn);

    void Start()
    {
        AutoCacheComponents();
        SetState(isOn);
    }

    private void AutoCacheComponents()
    {
        if (target == null) target = gameObject;

        if (solid == null)
        {
            solid = target.GetComponent<Collider2D>();
            if (solid == null) solid = target.GetComponentInChildren<Collider2D>(true);
        }

        if (sr == null)
        {
            sr = target.GetComponent<SpriteRenderer>();
            if (sr == null) sr = target.GetComponentInChildren<SpriteRenderer>(true);
        }
    }

}
