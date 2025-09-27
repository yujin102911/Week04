using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartSlider : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Slider startSlider;   // UI �����̴�
    [SerializeField] private string mapSceneName;  // �ҷ��� �� �� �̸�

    [Header("Options")]
    [SerializeField, Min(0f)] private float threshold = 0.99f; // ������ �зȴٰ� �ν��� ����

    private bool started = false;

    private void Awake()
    {
        if (startSlider != null)
            startSlider.onValueChanged.AddListener(OnSliderChanged);

        // ���� �� �׻� 0���� ����
        if (startSlider != null)
            startSlider.value = 0f;
    }

    private void OnSliderChanged(float value)
    {
        if (started) return; // �ߺ� ���� ����

        if (value >= threshold)
        {
            started = true;
            Debug.Log("���� ����!");
            SceneManager.LoadScene(mapSceneName);
        }
    }
}
