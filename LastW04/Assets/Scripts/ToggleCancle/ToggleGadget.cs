using UnityEngine;

public class ToggleGadget : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private SpriteRenderer sr; // 가젯 아이콘(필수)

    [Header("Visuals")]
    [SerializeField] private Sprite offSprite;   // 꺼짐 아이콘
    [SerializeField] private Sprite onSprite;    // 켜짐 아이콘
    [SerializeField] private bool startAsOn = false; // 시작 시 아이콘 상태

    // 내부 상태
    private bool isHeld = false;       // 손에 들고 있나
    private AttachPoint attachedAP;    // 붙은 부착점
    private ToggleTarget attachedTT;   // 토글 대상

    private void Start()
    {
        // 아이콘 초기화(인스펙터에서 offSprite/onSprite를 할당해두세요)
        if (sr)
            sr.sprite = startAsOn ? onSprite : offSprite;

        TryAttach();
    }

    void Update() { }

    void OnMouseDown()
    {
        if (GameManager.mode == Mode.None)
        {
            // 3) 이미 부착되어 있다 → 이 가젯을 클릭하면 대상 토글 작동
            if (attachedAP != null && attachedTT != null)
            {
                Activate();
            }
        }
    }

    void OnMouseUp() // 클릭 뗐을 때
    {
        if (GameManager.mode == Mode.Editing) // 에딧 상태라면
            TryAttach(); // 다시 붙기 시도
    }

    private void TryAttach()
    {
        var hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(1, 1), 0);
        AttachPoint apFound = null;
        ToggleTarget ttFound = null;

        foreach (var h in hits)
        {
            var ap = h.GetComponentInParent<AttachPoint>();
            var tt = h.GetComponentInParent<ToggleTarget>();

            if (ap != null && !ap.occupied && tt != null)
            {
                apFound = ap;
                ttFound = tt;
                break;
            }
        }

        if (apFound == null || ttFound == null) return;

        // 스냅 부착
        attachedAP = apFound;
        attachedTT = ttFound;

        transform.SetParent(attachedAP.snap, false);
        transform.localPosition = Vector3.zero + Vector3.back;
        transform.localRotation = Quaternion.identity;

        attachedAP.occupied = true;
        isHeld = false; // 손에서 내려놓음
    }

    private void Activate()
    {
        // 1) 토글 실행
        attachedTT.Toggle();

        // 2) 아이콘 스프라이트 토글(간단 버전)
        if (sr && onSprite && offSprite)
        {
            // 현재 무엇이 표시 중이든 반대로 전환
            sr.sprite = (sr.sprite == onSprite) ? offSprite : onSprite;
        }

        // 필요하면 이펙트/사운드 추가
    }
}
