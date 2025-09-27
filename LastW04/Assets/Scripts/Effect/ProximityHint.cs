using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class ProximityHint : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform player;
    [SerializeField] private SpriteRenderer hint;

    [Header("Options")]
    [SerializeField] private float radius = 1.5f;
    [SerializeField] private float fadeDuration = 0.2f; // ���̵� �ð�

    private Coroutine fading;
    private float initialAlpha = 1f;

    private void Awake()
    {
        if (hint)
        {
            initialAlpha = hint.color.a;
            // ���� �� �����ϰ�
            Color c = hint.color;
            hint.color = new Color(c.r, c.g, c.b, 0f);
            hint.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!player || !hint) return;

        bool near = Vector2.Distance(player.position, transform.position) <= radius;

        if (near && !hint.gameObject.activeSelf)
        {
            hint.gameObject.SetActive(true);
            StartFade(visible: true);
        }
        else if (!near && hint.gameObject.activeSelf)
        {
            StartFade(visible: false);
        }
    }

    private void StartFade(bool visible)
    {
        if (fading != null) StopCoroutine(fading);
        fading = StartCoroutine(FadeTo(visible ? initialAlpha : 0f));
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        float startAlpha = hint.color.a;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);
            Color c = hint.color;
            hint.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }

        // ������ ����
        Color final = hint.color;
        hint.color = new Color(final.r, final.g, final.b, targetAlpha);

        // ������ ������� ���ֱ�
        if (Mathf.Approximately(targetAlpha, 0f))
            hint.gameObject.SetActive(false);

        fading = null;
    }
}
