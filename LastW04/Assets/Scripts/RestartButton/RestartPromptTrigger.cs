using UnityEngine;
using UnityEngine.InputSystem; // �� �Է� �ý��� ��� ��

[DisallowMultipleComponent]
public class RestartPromptTrigger : MonoBehaviour
{
    [SerializeField] private ConfirmResetUI confirmUI;
    [SerializeField] private Key key = Key.R; // R Ű �⺻

    void Update()
    {
        // Ű�ε� �˾� ����
        if (Keyboard.current != null && Keyboard.current[key].wasPressedThisFrame)
        {
            if (confirmUI) confirmUI.Show();
        }
    }

    // ĵ���� ��ư OnClick�� �� �޼��带 �����ϸ� ��ư���ε� �˾��� ��� �� ����
    public void OpenPrompt()
    {
        if (confirmUI) confirmUI.Show();
    }
}
