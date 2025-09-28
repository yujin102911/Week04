using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartSlider : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Slider startSlider;       // UI 슬라이더
    [SerializeField] private string mapSceneName;      // 불러올 맵 씬 이름
    [SerializeField] private Image fadeImage;          // 페이드용 검정 이미지 (Canvas에 전체화면으로 배치)

    [Header("Options")]
    [SerializeField, Min(0f)] private float threshold = 0.99f;     // 끝까지 밀렸다고 인식할 기준
    [SerializeField, Min(0.01f)] private float fadeDuration = 0.5f; // 페이드 시간

    private bool started = false;

    private void Awake()
    {
        if (startSlider != null)
            startSlider.onValueChanged.AddListener(OnSliderChanged);

        if (startSlider != null)
            startSlider.value = 0f;

        if (fadeImage != null)
        {
            // 처음에는 비활성화
            fadeImage.gameObject.SetActive(false);
        }
    }

    private void OnSliderChanged(float value)
    {
        if (started) return;

        if (value >= threshold)
        {
            started = true;
            if (startSlider) startSlider.interactable = false;
            StartCoroutine(FadeAndLoad());
        }
    }

    private IEnumerator FadeAndLoad()
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);

            // 시작 알파 0으로 초기화
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;

            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                float a = Mathf.Clamp01(t / fadeDuration);
                c.a = a;
                fadeImage.color = c;
                yield return null;
            }
            c.a = 1f;
            fadeImage.color = c;
        }

        SceneManager.LoadScene(mapSceneName);
    }
}
