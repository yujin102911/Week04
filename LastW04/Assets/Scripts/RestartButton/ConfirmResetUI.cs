using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ConfirmResetUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject panel;   // 팝업 루트 오브젝트
    [SerializeField] private Button yesButton;   // 확인
    [SerializeField] private Button noButton;    // 취소

    [Header("Behavior")]
    [SerializeField] private bool pauseTimeScaleWhileOpen = true;

    [Header("Events")]
    public UnityEvent onConfirmed;  // 실제 리셋 실행(HardResetScene)을 인스펙터에서 연결
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
        onConfirmed?.Invoke(); // 인스펙터에서 HardResetScene 연결
    }

    public void Cancel()
    {
        Hide();
        onCanceled?.Invoke();
    }
}
