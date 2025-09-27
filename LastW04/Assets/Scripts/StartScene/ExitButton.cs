using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        if (exitButton != null)
            exitButton.onClick.AddListener(QuitGame);
    }

    private void QuitGame()
    {
        Debug.Log("���� ����!");
        Application.Quit();

        // �����Ϳ��� �׽�Ʈ�� (����)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
