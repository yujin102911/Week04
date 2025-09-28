using UnityEngine;

[DisallowMultipleComponent]
public sealed class DoorSwitch : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private SpriteRenderer doorRenderer; // ���� ������ SpriteRenderer
    [SerializeField] private Sprite closedSprite;         // ���� ���� ��������Ʈ
    [SerializeField] private Sprite openSprite;           // ���� ���� ��������Ʈ

    [Header("Collider")]
    [SerializeField] private Collider2D doorCollider;     // ���� ���� �ݶ��̴�
    [SerializeField] private Collider2D[] attachPointColliders; // �� ��/�ֺ� ������ �ݶ��̴���

    [Header("Options")]
    [SerializeField] private bool openDisablesCollider = true; // ������ �ݶ��̴� ��Ȱ��
    [SerializeField] private bool openDisablesAttachPoints = true; // ������ �������� ��Ȱ��

    private bool isOpen;

    public void ApplyState(bool isPressed)
    {
        SetOpen(isPressed);
    }

    public void SetOpen(bool open)
    {
        if (isOpen == open) return; // �̹� ���� ���¸� �н�
        isOpen = open;

        // ���� �� �ݶ��̴� ó��
        if (doorCollider && openDisablesCollider)
            doorCollider.enabled = !open;

        // ������ �ݶ��̴� ó��
        if (attachPointColliders != null && openDisablesAttachPoints)
        {
            foreach (var col in attachPointColliders)
            {
                if (col) col.enabled = !open;
            }
        }

        // ��������Ʈ ��ü
        if (doorRenderer)
        {
            doorRenderer.sprite = open ? openSprite : closedSprite;
        }
    }
}
