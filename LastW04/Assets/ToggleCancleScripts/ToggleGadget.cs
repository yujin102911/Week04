using UnityEngine;

public class ToggleGadget : MonoBehaviour
{
    [Header("Refs")]

    // 내부 상태
    private bool isHeld = false;       // 손에 들고 있나
    private AttachPoint attachedAP;    // 붙은 부착점
    private ToggleTarget attachedTT;     // 토글 대상
    private void Start()
    {
        TryAttach();
    }
    void Update()
    {

    }

    void OnMouseDown()
    {
        // 3) 이미 부착되어 있다 → 이 가젯을 클릭하면 대상 토글 작동
        if (attachedAP != null && attachedTT != null)
        {
            Activate();
        }
    }

    private void TryAttach()
    {
        // 마우스 지점과 겹치는 콜라이더 모두 확인
        var hits = Physics2D.OverlapBoxAll(transform.position,new Vector2(1,1),0);
        AttachPoint apFound = null;
        ToggleTarget ttFound = null;

        foreach (var h in hits)
        {
            // 같은 오브젝트에 두 컴포넌트가 따로 있을 수 있어 GetComponentInParent 사용
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
        transform.localPosition = Vector3.zero+Vector3.back;
        transform.localRotation = Quaternion.identity;

        attachedAP.occupied = true;
        isHeld = false; // 손에서 내려놓음
    }

    private void Activate()
    {
        attachedTT.Toggle();
        // 필요하면 이펙트/사운드 추가
    }
}
