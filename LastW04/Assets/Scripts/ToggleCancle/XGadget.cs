using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using static UnityEngine.UI.Image;

public class XGadget : MonoBehaviour
{
    [Header("Refs")]

    // 내부 상태
    public bool isHeld = true;
    [SerializeField]
    private AttachPoint attachedAP;        // 붙은 부착점
    [SerializeField]
    private DeletableTarget target;        // 삭제 대상
    [SerializeField]
    private UIPlacer UIPlacer;

    private void Start()
    {
        TryAttachAtMouse();
    }

    void Update()
    {
        if (isHeld)
        {
            attachedAP.occupied = false;
            TryAttachAtMouse();
        }
    }

    void OnMouseDown()
    {
        if (GameManager.mode != Mode.Editing)//일반 상태라면
                                          // 3) 이미 부착되어 있다 → 이 가젯을 클릭하면 대상 토글 작동
            if (attachedAP != null && target != null)
            {
                Activate();
            }
    }

    private void TryAttachAtMouse()
    {
        // 클릭 지점과 겹치는 모든 콜라이더 검사
        //Debug.Log(transform.position);
        var hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(1, 1), 0);
        AttachPoint apFound = null;
        DeletableTarget targetFound = null;

        foreach (var h in hits)
        {
            apFound = h.GetComponentInParent<AttachPoint>();
            targetFound = h.GetComponentInParent<DeletableTarget>();
        }

        if (apFound == null)
        {
            //if(targetFound != null)
            {
                Destroy(gameObject);
                return;
            }
        }
        else
        {
            attachedAP = apFound;
        }

        target = targetFound;

        /*transform.SetParent(attachedAP.snap.transform, false);
        transform.localPosition = Vector3.zero + Vector3.back;
        transform.localRotation = Quaternion.identity;*/
        Destroy(attachedAP.transform.parent.gameObject);
        UIPlacer.placed = true;//나 더 못옮겨욧
        //attachedAP.occupied = true;
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
        target.DeleteSelf();

        // 만약 재사용형으로 만들고 싶다면, 대상 삭제 전에 분리:
        // transform.SetParent(null);
        // attachedAP.occupied = false;
        // attachedAP = null;
        // target = null;
    }
}
