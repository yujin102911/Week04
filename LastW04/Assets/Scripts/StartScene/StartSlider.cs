using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartSlider : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Slider startSlider;   // UI 슬라이더
    [SerializeField] private string mapSceneName;  // 불러올 맵 씬 이름

    [Header("Options")]
    [SerializeField, Min(0f)] private float threshold = 0.99f; // 끝까지 밀렸다고 인식할 기준

    private bool started = false;

    private void Awake()
    {
        if (startSlider != null)
            startSlider.onValueChanged.AddListener(OnSliderChanged);

        // 시작 시 항상 0으로 세팅
        if (startSlider != null)
            startSlider.value = 0f;
    }

    private void OnSliderChanged(float value)
    {
        if (started) return; // 중복 실행 방지

        if (value >= threshold)
        {
            started = true;
            Debug.Log("게임 시작!");
            SceneManager.LoadScene(mapSceneName);
        }
    }
}
