using UnityEngine;

public class ToggleTarget : MonoBehaviour
{
    [Header("Target (����/�Ѱ�/���� �ٲ� ���)")]
    [SerializeField] private GameObject target;   // ����: ������ ���

    [Header("Optional Components (��������Ʈ/�ݶ��̴� ��ȯ��)")]
    [SerializeField] private Collider2D solid;         // ����� Collider2D (���Ҵ� �� �ڵ� Ž��)
    [SerializeField] private SpriteRenderer sr;        // ����� SpriteRenderer (���Ҵ� �� �ڵ� Ž��)

    [Header("Sprites (�� �� ���� ��ȯ��)")]
    [SerializeField] private Sprite openSprite;        // ������ ��
    [SerializeField] private Sprite closedSprite;      // ������ ��

    [Header("State")]
    [SerializeField] private bool isOn = true;         // true = ����(�Ǵ� ����)

    [Header("On/Off Mode")]
    [Tooltip("������Ʈ ��ü�� ���� �Ѱ� ������ true (�̶� target.SetActive �Ǵ� ��ü ���� ���)")]
    [SerializeField] private bool toggleGameObject = false;

    [Header("Options")]
    [Tooltip("���� �� solid �ݶ��̴��� ���� ����(���� ���� ������, Ŭ���� ��� �������� false)")]
    [SerializeField] private bool disableSolidOnOpen = true; // �� �߰�

    public bool IsOn => isOn;

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
        AutoCacheComponents();
    }

    public void SetState(bool on)
    {
        isOn = on;

        // ��� ������ �ڱ� �ڽ� �������
        if (target == null) target = gameObject;

        // 1) ��� ������Ʈ ��ü�� ���� �Ѵ� ���
        if (toggleGameObject)
        {
            // target�� �ڱ� �ڽ��� �ƴϸ�: �״�� SetActive
            if (target != gameObject)
            {
                if (target.activeSelf != isOn)
                    target.SetActive(isOn);
                return; // �� �ݵ�� ��ȯ
            }

        }

        // 2) �ݶ��̴�/��������Ʈ�� ��ȯ�ϴ� ��� (����)
        if (solid == null || sr == null)
        {
            AutoCacheComponents();
        }

        // �� Ŭ���� ��� �޾ƾ� �Ѵٸ� disableSolidOnOpen=false�� �θ� ��
        if (solid && disableSolidOnOpen)
            solid.enabled = !isOn; // ������ �浹 off

        if (sr)
            sr.sprite = isOn ? openSprite : closedSprite; // ��������Ʈ ��ü
    }

    public void Toggle() => SetState(!isOn);

    void Start()
    {
        AutoCacheComponents();
        SetState(isOn);
    }

    private void AutoCacheComponents()
    {
        if (target == null) target = gameObject;

        if (solid == null)
        {
            solid = target.GetComponent<Collider2D>();
            if (solid == null) solid = target.GetComponentInChildren<Collider2D>(true);
        }

        if (sr == null)
        {
            sr = target.GetComponent<SpriteRenderer>();
            if (sr == null) sr = target.GetComponentInChildren<SpriteRenderer>(true);
        }
    }

}
