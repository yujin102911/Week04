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
    [SerializeField] private float fadeDuration = 0.2f; // 페이드 시간

    private Coroutine fading;
    private float initialAlpha = 1f;

    private void Awake()
    {
        if (hint)
        {
            initialAlpha = hint.color.a;
            // 시작 시 투명하게
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

        // 마지막 보정
        Color final = hint.color;
        hint.color = new Color(final.r, final.g, final.b, targetAlpha);

        // 완전히 사라지면 꺼주기
        if (Mathf.Approximately(targetAlpha, 0f))
            hint.gameObject.SetActive(false);

        fading = null;
    }
}
