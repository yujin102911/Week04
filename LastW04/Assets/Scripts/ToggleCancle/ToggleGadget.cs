using UnityEngine;

public class ToggleGadget : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private SpriteRenderer sr; // 가젯 아이콘(필수)

    [Header("Visuals")]
    [SerializeField] private Sprite offSprite;           // 꺼짐 아이콘
    [SerializeField] private Sprite onSprite;            // 켜짐 아이콘
    [SerializeField] private bool startAsOn = false;     // 시작 시 아이콘 상태

    // 내부 상태
    public bool isHeld = false;       // 손에 들고 있었나
    public bool placed = false;       // 설치 완료 상태인가
    private AttachPoint attachedPoint;    // 붙은 부착점
    private ToggleTarget attachedTT;      // 기존 토글 대상(유지)
    private DoorToggle attachedDoor;      // 추가: 문 전용 토글 대상
    private SimpleButton attachedButton;

    private void Start()
    {
        if (sr)
            sr.sprite = startAsOn ? onSprite : offSprite;

        TryAttach();
    }

    void Update() 
    {
        if (isHeld) 
        {
            TryAttach();
        }
    }

    private void OnMouseDown()
    {
        if (GameManager.mode == Mode.None)
        {
            // 부착되어 있으면 작동
            if (attachedPoint != null && (attachedTT != null || attachedDoor != null || attachedButton != null))
            {
                Activate();
                Debug.Log("토글실행");
            }
        }
    }

    private void OnMouseUp()
    {
        if (GameManager.mode == Mode.Editing)
            TryAttach(); // 다시 부착 시도
    }

    public void TryAttach()
    {
        var hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(1, 1), 0f);

        AttachPoint apFound = null;
        ToggleTarget ttFound = null;
        DoorToggle doorFound = null;
        SimpleButton buttonFound = null;

        foreach (var h in hits)
        {
            // 같은 오브젝트/부모에 있을 수 있으니 InParent로 검색
            if (!apFound) apFound = h.GetComponentInParent<AttachPoint>();
            if (!ttFound) ttFound = h.GetComponentInParent<ToggleTarget>();
            if (!doorFound) doorFound = h.GetComponentInParent<DoorToggle>();
            if (!buttonFound) buttonFound = h.GetComponentInParent<SimpleButton>();

            // 다 찾았으면 조기 종료(선택)
            if (apFound && (ttFound || doorFound || buttonFound))
                break;
        }

        if (apFound == null)
        {
            // 부착 실패 시 자신 제거 (기존 로직 유지)
            Destroy(gameObject);
            return;
        }

        attachedPoint = apFound;
        attachedTT = ttFound;    // 기존 유지
        attachedDoor = doorFound;  // 추가
        attachedButton = buttonFound;

        // 스냅 부착
        if (attachedPoint && attachedPoint.snap)
        {
            transform.SetParent(attachedPoint.snap.transform, false);
            transform.localPosition = Vector3.zero + Vector3.back;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            // snap이 비어있다면 AP 위치로 스냅
            transform.SetParent(attachedPoint.transform, false);
            transform.localPosition = Vector3.back;
            transform.localRotation = Quaternion.identity;
        }

        attachedPoint.occupied = true;
        isHeld = false;
        placed = true;
    }

    private void Activate()
    {
        // 1) DoorToggle이 있으면 우선 실행 (문: 스프라이트+콜라이더 자동 처리)
        if (attachedDoor != null)
        {
            attachedDoor.Toggle();
        }
        // 1) DoorToggle이 있으면 우선 실행 (문: 스프라이트+콜라이더 자동 처리)
        if (attachedButton != null)
        {
            attachedButton.Toggle();
        }
        // 2) 그 외엔 기존 ToggleTarget으로 실행 (기존 동작 보존)
        else if (attachedTT != null)
        {
            attachedTT.Toggle();
        }

        // 3) 가젯 아이콘 토글(간단 버전 유지)
        if (sr && onSprite && offSprite)
        {
            sr.sprite = (sr.sprite == onSprite) ? offSprite : onSprite;
        }
    }

    private void OnDestroy()
    {
        if (attachedPoint)
            attachedPoint.occupied = false;
    }
}
