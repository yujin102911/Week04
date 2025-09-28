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
        Debug.Log("게임 종료!");
        Application.Quit();

        // 에디터에서 테스트용 (선택)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
