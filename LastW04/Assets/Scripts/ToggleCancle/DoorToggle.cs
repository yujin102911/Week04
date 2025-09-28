using UnityEngine;

[DisallowMultipleComponent]
public class DoorToggle : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openSprite;

    [Header("Blockers (���� �ݶ��̴���)")]
    [Tooltip("�� ��ü�� ���� �ݶ��̴� 1��(������ ����� ��)")]
    [SerializeField] private Collider2D doorCollider;
    [Tooltip("�ڽ��� AttachPoint ��, �Բ� ���� �ϴ� �߰� �ݶ��̴���")]
    [SerializeField] private Collider2D[] extraColliders;

    [Header("State")]
    [SerializeField] private bool isOpen = false;

    [Header("Options")]
    [Tooltip("������ �� ���� ��� �ݶ��̴����� ��Ȱ��ȭ���� ����")]
    [SerializeField] private bool disableCollidersWhenOpen = true;

    public void Toggle() => SetOpen(!isOpen);

    public void SetOpen(bool open)
    {
        isOpen = open;

        // ��������Ʈ
        if (sr) sr.sprite = isOpen ? openSprite : closedSprite;

        // �ݶ��̴� on/off (�� + �߰��� ����)
        bool enable = !(disableCollidersWhenOpen && isOpen);
        if (doorCollider) doorCollider.enabled = enable;

        if (extraColliders != null)
        {
            for (int i = 0; i < extraColliders.Length; i++)
            {
                if (extraColliders[i]) extraColliders[i].enabled = enable;
            }
        }

        Physics2D.SyncTransforms();
    }

    private void Start()
    {
        // ���� �� ���� �ݿ�
        SetOpen(isOpen);
    }
}
