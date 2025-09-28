using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ConfirmResetUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject panel;   // �˾� ��Ʈ ������Ʈ
    [SerializeField] private Button yesButton;   // Ȯ��
    [SerializeField] private Button noButton;    // ���

    [Header("Behavior")]
    [SerializeField] private bool pauseTimeScaleWhileOpen = true;

    [Header("Events")]
    public UnityEvent onConfirmed;  // ���� ���� ����(HardResetScene)�� �ν����Ϳ��� ����
    public UnityEvent onCanceled;

    bool isOpen;

    void Awake()
    {
        if (panel) panel.SetActive(false);
        if (yesButton) yesButton.onClick.AddListener(Confirm);
        if (noButton) noButton.onClick.AddListener(Cancel);
    }

    public void Show()
    {
        if (isOpen) return;
        isOpen = true;

        if (panel) panel.SetActive(true);
        if (pauseTimeScaleWhileOpen)
            Time.timeScale = 0f;
    }

    public void Hide()
    {
        if (!isOpen) return;
        isOpen = false;

        if (panel) panel.SetActive(false);
        if (pauseTimeScaleWhileOpen)
            Time.timeScale = 1f;
    }

    public void Confirm()
    {
        Hide();
        onConfirmed?.Invoke(); // �ν����Ϳ��� HardResetScene ����
    }

    public void Cancel()
    {
        Hide();
        onCanceled?.Invoke();
    }
}
