using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class Door : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private bool isLocked = true;   // ��� ����
    [SerializeField] private bool isOpen = false;  // ���� ����(������ ��Ȱ��ȭ)

    [Header("Command (�ܺ� ȣ���)")]
    [Tooltip("�ܺο��� �� �̺�Ʈ�� Invoke �ϸ� TryOpen()�� ����˴ϴ�.")]
    public UnityEvent OpenCommand; // ȣ���ϱ� ���� �̺�Ʈ ����

    [Header("Callbacks (��� �˸���)")]
    public UnityEvent OnOpened;        // ������ ������ ��(���� 1ȸ)
    public UnityEvent OnOpenFailed;    // ���� ����(������� ��)
    public UnityEvent OnLocked;        // ��� ���� �˸�
    public UnityEvent OnUnlocked;      // ��� ���� �˸�

    private void Awake()
    {
        // �̺�Ʈ�� ȣ��� ���� ���� ����
        if (OpenCommand == null) OpenCommand = new UnityEvent();
        OpenCommand.AddListener(TryOpen);

        // ���� ���� ����ȭ(���� ���¶�� ��Ȱ��ȭ)
        if (isOpen && gameObject.activeSelf)
            ApplyOpenVisual();

        Invoke("ForceOpen", 3f); // �׽�Ʈ��: 3�� �� ���� ����
    }

    // ===== �ܺ� API(�ٸ� ��ũ��Ʈ���� ȣ��) =====

    /// <summary>������� ������ ���� ����(���� ȣ��)</summary>
    public void TryOpen()
    {
        if (isOpen) return;

        if (isLocked)
        {
            OnOpenFailed?.Invoke();
            return;
        }

        isOpen = true;
        ApplyOpenVisual();
        OnOpened?.Invoke();
    }

    /// <summary>������ ���� ����(��� ����, ����/�ƽſ�)</summary>
    public void ForceOpen()
    {
        if (isOpen) return;
        isOpen = true;
        ApplyOpenVisual();
        OnOpened?.Invoke();
    }

    /// <summary>���� ��ٴ�</summary>
    public void Lock()
    {
        isLocked = true;
        OnLocked?.Invoke();
    }

    /// <summary>�� ����� �����Ѵ�</summary>
    public void Unlock()
    {
        isLocked = false;
        OnUnlocked?.Invoke();
    }

    /// <summary>���� ��� ���� ��ȯ</summary>
    public bool IsLocked() => isLocked;

    /// <summary>���� ���� ���� ��ȯ</summary>
    public bool IsOpen() => isOpen;

    // ===== ���� ó�� =====
    private void ApplyOpenVisual()
    {
        // �䱸����: open ���°� �Ǹ� ���� ����(active=false)
        // Animator/Collider�� ���� ��쿣 ���⼭ ���ָ� �˴ϴ�.
        // (�ʿ� �� �� �κ��� Animator Ʈ���ų� Collider disable�� ��ü ����)
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }
}
