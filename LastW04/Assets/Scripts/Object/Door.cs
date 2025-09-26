using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class Door : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private bool isLocked = true;   // 잠금 상태
    [SerializeField] private bool isOpen = false;  // 열림 상태(열리면 비활성화)

    [Header("Command (외부 호출용)")]
    [Tooltip("외부에서 이 이벤트를 Invoke 하면 TryOpen()이 실행됩니다.")]
    public UnityEvent OpenCommand; // 호출하기 쉬운 이벤트 형식

    [Header("Callbacks (결과 알림용)")]
    public UnityEvent OnOpened;        // 실제로 열렸을 때(최초 1회)
    public UnityEvent OnOpenFailed;    // 열기 실패(잠겨있음 등)
    public UnityEvent OnLocked;        // 잠김 변경 알림
    public UnityEvent OnUnlocked;      // 잠금 해제 알림

    private void Awake()
    {
        // 이벤트형 호출과 실제 로직 연결
        if (OpenCommand == null) OpenCommand = new UnityEvent();
        OpenCommand.AddListener(TryOpen);

        // 시작 상태 동기화(열림 상태라면 비활성화)
        if (isOpen && gameObject.activeSelf)
            ApplyOpenVisual();

        Invoke("ForceOpen", 3f); // 테스트용: 3초 후 강제 오픈
    }

    // ===== 외부 API(다른 스크립트에서 호출) =====

    /// <summary>잠겨있지 않으면 문을 연다(권장 호출)</summary>
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

    /// <summary>강제로 문을 연다(잠금 무시, 퍼즐/컷신용)</summary>
    public void ForceOpen()
    {
        if (isOpen) return;
        isOpen = true;
        ApplyOpenVisual();
        OnOpened?.Invoke();
    }

    /// <summary>문을 잠근다</summary>
    public void Lock()
    {
        isLocked = true;
        OnLocked?.Invoke();
    }

    /// <summary>문 잠금을 해제한다</summary>
    public void Unlock()
    {
        isLocked = false;
        OnUnlocked?.Invoke();
    }

    /// <summary>현재 잠금 여부 반환</summary>
    public bool IsLocked() => isLocked;

    /// <summary>현재 열림 여부 반환</summary>
    public bool IsOpen() => isOpen;

    // ===== 내부 처리 =====
    private void ApplyOpenVisual()
    {
        // 요구사항: open 상태가 되면 문이 꺼짐(active=false)
        // Animator/Collider를 쓰는 경우엔 여기서 꺼주면 됩니다.
        // (필요 시 이 부분을 Animator 트리거나 Collider disable로 교체 가능)
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }
}
