using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ConfirmQuitUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject panel;   // 팝업 루트 오브젝트
    [SerializeField] private Button yesButton;   // 종료 확인
    [SerializeField] private Button noButton;    // 취소

    [Header("Behavior")]
    [SerializeField] private bool pauseTimeScaleWhileOpen = true;

    [Header("Events")]
    public UnityEvent onConfirmed;  // 실제 종료 실행 (Application.Quit 연결 가능)
    public UnityEvent onCanceled;

    private bool isOpen;

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
        onConfirmed?.Invoke();

#if UNITY_EDITOR
        // 에디터에서만 플레이 모드 종료
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 빌드 환경에서는 게임 종료
#endif
    }

    public void Cancel()
    {
        Hide();
        onCanceled?.Invoke();
    }
}
