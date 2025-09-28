using UnityEngine;
using UnityEngine.InputSystem; // 새 입력 시스템 사용 시

[DisallowMultipleComponent]
public class RestartPromptTrigger : MonoBehaviour
{
    [SerializeField] private ConfirmResetUI confirmUI;
    [SerializeField] private Key key = Key.R; // R 키 기본

    void Update()
    {
        // 키로도 팝업 열기
        if (Keyboard.current != null && Keyboard.current[key].wasPressedThisFrame)
        {
            if (confirmUI) confirmUI.Show();
        }
    }

    // 캔버스 버튼 OnClick에 이 메서드를 연결하면 버튼으로도 팝업을 띄울 수 있음
    public void OpenPrompt()
    {
        if (confirmUI) confirmUI.Show();
    }
}
