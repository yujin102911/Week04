using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartSlider : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Slider startSlider;       // UI �����̴�
    [SerializeField] private string mapSceneName;      // �ҷ��� �� �� �̸�
    [SerializeField] private Image fadeImage;          // ���̵�� ���� �̹��� (Canvas�� ��üȭ������ ��ġ)

    [Header("Options")]
    [SerializeField, Min(0f)] private float threshold = 0.99f;     // ������ �зȴٰ� �ν��� ����
    [SerializeField, Min(0.01f)] private float fadeDuration = 0.5f; // ���̵� �ð�

    private bool started = false;

    private void Awake()
    {
        if (startSlider != null)
            startSlider.onValueChanged.AddListener(OnSliderChanged);

        if (startSlider != null)
            startSlider.value = 0f;

        if (fadeImage != null)
        {
            // ó������ ��Ȱ��ȭ
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

            // ���� ���� 0���� �ʱ�ȭ
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
