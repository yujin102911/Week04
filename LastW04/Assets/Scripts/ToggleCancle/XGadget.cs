using UnityEngine;

public class XGadget : MonoBehaviour
{
    [Header("Refs")]

    // 내부 상태
    private bool isHeld = false;
    private AttachPoint attachedAP;        // 붙은 부착점
    private DeletableTarget target;        // 삭제 대상

    void Start()
    {
        TryAttachAtMouse();
    }

    void OnMouseDown()
    {
        if (GameManager.mode == Mode.None)//일반 상태라면
                                          // 3) 이미 부착되어 있다 → 이 가젯을 클릭하면 대상 토글 작동
            if (attachedAP != null && target != null)
            {
                Activate();
            }
    }

    private void TryAttachAtMouse()
    {
        // 클릭 지점과 겹치는 모든 콜라이더 검사
        var hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(1, 1), 0);
        AttachPoint apFound = null;
        DeletableTarget targetFound = null;

        foreach (var h in hits)
        {
            var ap = h.GetComponentInParent<AttachPoint>();
            var dt = h.GetComponentInParent<DeletableTarget>();

            if (ap != null && !ap.occupied && dt != null)
            {
                apFound = ap;
                targetFound = dt;
                break;
            }
        }

        if (apFound == null || targetFound == null) return;

        // 스냅 Transform 확보(없으면 AP 자신을 사용)
        Transform snap = apFound.snap != null ? apFound.snap : apFound.transform;
        if (apFound.snap == null)
            Debug.LogWarning($"[XGadget] '{apFound.name}'의 snap이 비어 있어 AP 위치로 대체합니다.");

        attachedAP = apFound;
        target = targetFound;

        // 월드 좌표를 먼저 맞춘 뒤 부모 등록(정확한 스냅)
        transform.position = snap.position;
        transform.rotation = snap.rotation;
        transform.SetParent(snap, false);
        transform.localPosition = Vector3.zero + Vector3.back;
        transform.localRotation = Quaternion.identity;

        attachedAP.occupied = true;
        isHeld = false; // 손에서 내려놓음
    }

    private void Activate()
    {
        if (target == null) return;

        if (!target.CanDelete)
        {
            // 피드백만 주고 종료(필요 시 머티리얼 깜빡임, 사운드 등)
            Debug.Log("삭제 불가 대상입니다(essential).");
            return;
        }

        // =대상과 함께 가젯도 파괴(가젯이 Snap의 자식이므로 부모 파괴 시 같이 사라짐)
        //target.DeleteSelf();

        // 만약 재사용형으로 만들고 싶다면, 대상 삭제 전에 분리:
        // transform.SetParent(null);
        // attachedAP.occupied = false;
        // attachedAP = null;
        // target = null;
    }
}
