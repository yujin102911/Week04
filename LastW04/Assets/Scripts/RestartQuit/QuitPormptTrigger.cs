using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class QuitPromptTrigger : MonoBehaviour
{
    [SerializeField] private ConfirmQuitUI confirmUI;
    [SerializeField] private Key key = Key.Escape; // ESC 키 기본

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current[key].wasPressedThisFrame)
        {
            if (confirmUI) confirmUI.Show();
        }
    }

    // UI 버튼 OnClick에 연결하면 버튼 클릭으로도 팝업을 띄울 수 있음
    public void OpenPrompt()
    {
        if (confirmUI) confirmUI.Show();
    }
}
