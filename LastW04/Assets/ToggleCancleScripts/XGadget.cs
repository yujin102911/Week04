using UnityEngine;

public class XGadget : MonoBehaviour
{
    [Header("Refs")]
    public Camera cam; // Main Camera �巡��

    // ���� ����
    private bool isHeld = false;
    private AttachPoint attachedAP;        // ���� ������
    private DeletableTarget target;        // ���� ���

    void Update()
    {
        // ��� ���� ���� Ŀ�� ����ٴ�
        if (isHeld && cam != null)
        {
            Vector3 p = cam.ScreenToWorldPoint(Input.mousePosition);
            p.z = 0f;
            transform.position = p;
        }
    }

    void OnMouseDown()
    {
        // 1) ���� ��� ���� ������ ����
        if (!isHeld && attachedAP == null)
        {
            isHeld = true;
            transform.SetParent(null);
            return;
        }

        // 2) �տ� ��� ������ �� ���� �õ�
        if (isHeld)
        {
            TryAttachAtMouse();
            return;
        }

        // 3) �̹� �����Ǿ� ������ �� �۵�(����)
        if (attachedAP != null && target != null)
        {
            Activate();
        }
    }

    private void TryAttachAtMouse()
    {
        if (cam == null) return;

        Vector3 p = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 point = new Vector2(p.x, p.y);

        // Ŭ�� ������ ��ġ�� ��� �ݶ��̴� �˻�
        var hits = Physics2D.OverlapPointAll(point);
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

        // ���� Transform Ȯ��(������ AP �ڽ��� ���)
        Transform snap = apFound.snap != null ? apFound.snap : apFound.transform;
        if (apFound.snap == null)
            Debug.LogWarning($"[XGadget] '{apFound.name}'�� snap�� ��� �־� AP ��ġ�� ��ü�մϴ�.");

        attachedAP = apFound;
        target = targetFound;

        // ���� ��ǥ�� ���� ���� �� �θ� ���(��Ȯ�� ����)
        transform.position = snap.position;
        transform.rotation = snap.rotation;
        transform.SetParent(snap, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        attachedAP.occupied = true;
        isHeld = false; // �տ��� ��������
    }

    private void Activate()
    {
        if (target == null) return;

        if (!target.CanDelete)
        {
            // �ǵ�鸸 �ְ� ����(�ʿ� �� ��Ƽ���� ������, ���� ��)
            Debug.Log("���� �Ұ� ����Դϴ�(essential).");
            return;
        }

        // �⺻ ����: ���� �Բ� ������ �ı�(������ Snap�� �ڽ��̹Ƿ� �θ� �ı� �� ���� �����)
        target.DeleteSelf();

        // ���� ���������� ����� �ʹٸ�, ��� ���� ���� �и�:
        // transform.SetParent(null);
        // attachedAP.occupied = false;
        // attachedAP = null;
        // target = null;
    }
}
