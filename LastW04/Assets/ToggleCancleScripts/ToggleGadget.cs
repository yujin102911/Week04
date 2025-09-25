using UnityEngine;

public class ToggleGadget : MonoBehaviour
{
    [Header("Refs")]
    public Camera cam; // �ν����Ϳ� Main Camera �巡��

    // ���� ����
    private bool isHeld = false;       // �տ� ��� �ֳ�
    private AttachPoint attachedAP;    // ���� ������
    private DoorToggle targetDoor;     // ��� ���

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
        // 1) ���� ��� ���� �ʰ�, ���� ��𿡵� ���� �ʾҴ� �� ����
        if (!isHeld && attachedAP == null)
        {
            isHeld = true;
            transform.SetParent(null);
            return;
        }

        // 2) �տ� ��� �ִ� �� ���� ���콺 ��ġ�� AttachPoint�� ���� �õ�
        if (isHeld)
        {
            TryAttachAtMouse();
            return;
        }

        // 3) �̹� �����Ǿ� �ִ� �� �� ������ Ŭ���ϸ� ��� ��� �۵�
        if (attachedAP != null && targetDoor != null)
        {
            Activate();
        }
    }

    private void TryAttachAtMouse()
    {
        if (cam == null) return;

        Vector3 p = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 point = new Vector2(p.x, p.y);

        // ���콺 ������ ��ġ�� �ݶ��̴� ��� Ȯ��
        var hits = Physics2D.OverlapPointAll(point);
        AttachPoint apFound = null;
        DoorToggle doorFound = null;

        foreach (var h in hits)
        {
            // ���� ������Ʈ�� �� ������Ʈ�� ���� ���� �� �־� GetComponentInParent ���
            var ap = h.GetComponentInParent<AttachPoint>();
            var door = h.GetComponentInParent<DoorToggle>();

            if (ap != null && !ap.occupied && door != null)
            {
                apFound = ap;
                doorFound = door;
                break;
            }
        }

        if (apFound == null || doorFound == null) return;

        // ���� ����
        attachedAP = apFound;
        targetDoor = doorFound;

        transform.SetParent(attachedAP.snap, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        attachedAP.occupied = true;
        isHeld = false; // �տ��� ��������
    }

    private void Activate()
    {
        targetDoor.Toggle();
        // �ʿ��ϸ� ����Ʈ/���� �߰�
    }
}
