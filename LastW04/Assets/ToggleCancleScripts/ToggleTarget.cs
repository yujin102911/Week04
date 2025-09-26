using UnityEngine;
using System.Collections;

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


    public bool IsOn => isOn;

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    public void Toggle() => SetState(!isOn);

    public void SetState(bool on)
    {

        isOn = on;

        // 1) ��� ������Ʈ ��ü�� ���� �Ѵ� ���
        if (toggleGameObject)
        {
            if (target != gameObject)
            {
                if (target.activeSelf != isOn)
                    target.SetActive(isOn);

                Physics2D.SyncTransforms();
                return;
            }

            Physics2D.SyncTransforms();
            return;
        }


        if (solid && disableSolidOnOpen)
            solid.enabled = !isOn; // ������ �浹 off (Ŭ���� �ݶ��̴��� ���� �������� �� ��)

        if (sr)
            sr.sprite = isOn ? openSprite : closedSprite;

        Physics2D.SyncTransforms();
    }

    private void Start()
    {
        SetState(isOn); // �ʱ� ���� �ݿ�(armed=false��� ���� ����� arm �ĺ��� ����)
    }
}
