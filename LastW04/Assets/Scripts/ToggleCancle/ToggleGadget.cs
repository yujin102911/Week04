using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ToggleGadget : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private SpriteRenderer sr; // 가젯 아이콘(필수)

    [Header("Visuals")]
    [SerializeField] private Sprite offSprite;   // 꺼짐 아이콘
    [SerializeField] private Sprite onSprite;    // 켜짐 아이콘
    [SerializeField] private bool startAsOn = false; // 시작 시 아이콘 상태

    // 내부 상태
    public bool isHeld = false;       // 손에 들고 있나
    private AttachPoint attachedPoint;    // 붙은 부착점
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
            if (attachedPoint != null && attachedTT != null)
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
        // 클릭 지점과 겹치는 모든 콜라이더 검사
        //Debug.Log(transform.position);
        var hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(1, 1), 0);
        AttachPoint apFound = null;
        ToggleTarget targetFound = null;

        foreach (var h in hits)
        {
            apFound = h.GetComponentInParent<AttachPoint>();
            targetFound = h.GetComponentInParent<ToggleTarget>();
        }

        if (apFound == null)
        {
            if (targetFound != null)
            {
                Destroy(gameObject);
                return;
            }
        }
        else
        {
            attachedPoint = apFound;
        }

        attachedTT = targetFound;

        transform.SetParent(attachedPoint.snap.transform, false);
        transform.localPosition = Vector3.zero + Vector3.back;
        transform.localRotation = Quaternion.identity;

        attachedPoint.occupied = true;
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
