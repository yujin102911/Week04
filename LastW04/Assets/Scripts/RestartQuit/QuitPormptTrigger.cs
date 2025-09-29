using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class QuitPromptTrigger : MonoBehaviour
{
    [SerializeField] private ConfirmQuitUI confirmUI;
    [SerializeField] private Key key = Key.Escape; // ESC Ű �⺻

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current[key].wasPressedThisFrame)
        {
            if (confirmUI) confirmUI.Show();
        }
    }

    // UI ��ư OnClick�� �����ϸ� ��ư Ŭ�����ε� �˾��� ��� �� ����
    public void OpenPrompt()
    {
        if (confirmUI) confirmUI.Show();
    }
}
