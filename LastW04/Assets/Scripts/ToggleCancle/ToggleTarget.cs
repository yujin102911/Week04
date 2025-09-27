using UnityEngine;
using System.Collections;
using UnityEngine.Events; // �� �߰�

public class ToggleTarget : MonoBehaviour
{
    [Header("Target (����/�Ѱ�/���� �ٲ� ���)")]
    [SerializeField] private GameObject target;

    [Header("Optional Components (��������Ʈ/�ݶ��̴� ��ȯ��)")]
    [SerializeField] private Collider2D solid;      // �浹��(Ŭ���� �θ� �ݶ��̴��� ���⿡ ���� �� ��)
    [SerializeField] private SpriteRenderer sr;

    [Header("Sprites (�� �� ���� ��ȯ��)")]
    [SerializeField] private Sprite openSprite;
    [SerializeField] private Sprite closedSprite;

    [Header("State")]
    [SerializeField] private bool isOn = true;

    [Header("On/Off Mode")]
    [Tooltip("������Ʈ ��ü�� ���� �Ѱ� ������ true (�̶� target.SetActive �Ǵ� ��ü ���� ���)")]
    [SerializeField] private bool toggleGameObject = true; // ������ũ �뵵 �⺻ true ��õ

    [Header("Options")]
    [Tooltip("���� �� solid �ݶ��̴��� ���� �Ǵ��� (Ŭ������ ���� ���� �� ��)")]
    [SerializeField] private bool disableSolidOnOpen = false; // Ŭ�� ��� ������ ���� �⺻ false

    [Header("Events")]
    [Tooltip("��� ���°� �ٲ� �� bool(isOn)�� �Բ� ����")]
    public UnityEvent<bool> onStateChanged; // �� �߰�

    public bool IsOn => isOn;

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    public void Toggle() => SetState(!isOn);

    public void SetState(bool on)
    {
        if (isOn == on)
        {
            // �׷��� �ܺο��� ������ ������ ����� �� ������ �ּ��� ���־�/�ݶ��̴�/�̺�Ʈ�� �ѹ� ���ش�
            ApplyLocalVisuals(on);
            onStateChanged?.Invoke(isOn);
            return;
        }

        isOn = on;

        // 1) ��� ������Ʈ ��ü�� ���� �Ѵ� ���
        if (toggleGameObject)
        {
            if (target != null && target != gameObject)
            {
                if (target.activeSelf != isOn)
                    target.SetActive(isOn);

                ApplyLocalVisuals(isOn);    // �� ��������Ʈ/�ݶ��̴� �ݿ�
                onStateChanged?.Invoke(isOn); // �� �̺�Ʈ ����
                Physics2D.SyncTransforms();
                return;
            }

            ApplyLocalVisuals(isOn);
            onStateChanged?.Invoke(isOn);
            Physics2D.SyncTransforms();
            return;
        }

        // 2) ���ӿ�����Ʈ on/off ��� ���� ���־�/�ݶ��̴��� �ٲٴ� ���
        ApplyLocalVisuals(isOn);
        onStateChanged?.Invoke(isOn); // �� �̺�Ʈ ����
        Physics2D.SyncTransforms();
    }

    private void ApplyLocalVisuals(bool on)
    {
        if (solid && disableSolidOnOpen)
            solid.enabled = !on; // ������ �浹 off (Ŭ���� �ݶ��̴��� ���� �������� �� ��)

        if (sr)
            sr.sprite = on ? openSprite : closedSprite;
    }

    private void Start()
    {
        // ���� �� ���� ���� �ݿ� + �̺�Ʈ �� �� ����(�ʱ� ���¿� �����Ǵ� �ܺ� ������Ʈ�� ���� �� ����)
        ApplyLocalVisuals(isOn);
        onStateChanged?.Invoke(isOn);
        Physics2D.SyncTransforms();
    }
}
