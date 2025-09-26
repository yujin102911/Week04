using UnityEngine;

public class ToggleGadget : MonoBehaviour
{
    [Header("Refs")]
    public Camera cam; // 인스펙터에 Main Camera 드래그

    // 내부 상태
    private bool isHeld = false;       // 손에 들고 있나
    private AttachPoint attachedAP;    // 붙은 부착점
    private ToggleTarget targetDoor;     // 토글 대상
    private void Start()
    {
        cam = Camera.main;
    }
    void Update()
    {
        // 들고 있을 때는 커서 따라다님
        if (isHeld && cam != null)
        {
            Vector3 p = cam.ScreenToWorldPoint(Input.mousePosition);
            p.z = 0f;
            transform.position = p;
        }
    }

    void OnMouseDown()
    {
        // 1) 아직 들고 있지 않고, 아직 어디에도 붙지 않았다 → 집기
        if (!isHeld && attachedAP == null)
        {
            isHeld = true;
            transform.SetParent(null);
            return;
        }

        // 2) 손에 들고 있다 → 현재 마우스 위치의 AttachPoint에 부착 시도
        if (isHeld)
        {
            TryAttach();
            return;
        }

        // 3) 이미 부착되어 있다 → 이 가젯을 클릭하면 대상 토글 작동
        if (attachedAP != null && targetDoor != null)
        {
            Activate();
        }
    }

    private void TryAttach()
    {
        if (cam == null) return;

        // 마우스 지점과 겹치는 콜라이더 모두 확인
        var hits = Physics2D.OverlapBox(transform.position,new Vector2(1,1),0);
        AttachPoint apFound = null;
        ToggleTarget doorFound = null;

        foreach (var h in hits)
        {
            // 같은 오브젝트에 두 컴포넌트가 따로 있을 수 있어 GetComponentInParent 사용
            var ap = h.GetComponentInParent<AttachPoint>();
            var door = h.GetComponentInParent<ToggleTarget>();

            if (ap != null && !ap.occupied && door != null)
            {
                apFound = ap;
                doorFound = door;
                break;
            }
        }

        if (apFound == null || doorFound == null) return;

        // 스냅 부착
        attachedAP = apFound;
        targetDoor = doorFound;

        transform.SetParent(attachedAP.snap, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        attachedAP.occupied = true;
        isHeld = false; // 손에서 내려놓음
    }

    private void Activate()
    {
        targetDoor.Toggle();
        // 필요하면 이펙트/사운드 추가
    }
}
