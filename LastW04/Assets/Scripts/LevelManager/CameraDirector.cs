using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraDirector : MonoBehaviour
{
    public static CameraDirector Instance { get; private set; }

    [Header("Target Camera")]
    public Camera targetCamera;

    [Header("Blend (optional)")]
    [SerializeField, Min(0f)] private float blendSeconds = 0f; // 0이면 즉시 워프

    private Dictionary<string, CameraRegion2D> _regions;

    void Awake()
    {
        Instance = this;
        if (!targetCamera) targetCamera = Camera.main;

        // 씬의 모든 CameraRegion2D를 캐시
        _regions = FindObjectsByType<CameraRegion2D>(FindObjectsSortMode.None)
            .Where(r => r && !string.IsNullOrEmpty(r.regionId))
            .GroupBy(r => r.regionId)
            .ToDictionary(g => g.Key, g => g.First());
    }

    public void WarpToRegion(string regionId, bool instant = true)
    {
        if (!_regions.TryGetValue(regionId, out var region) || !targetCamera) return;

        var pivot = region.pivot ? region.pivot : region.transform;
        var fromPos = targetCamera.transform.position;
        var toPos = new Vector3(pivot.position.x, pivot.position.y, fromPos.z);

        if (instant || blendSeconds <= 0f)
        {
            targetCamera.transform.position = toPos;
            targetCamera.orthographicSize = region.orthoSize;
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(CoBlend(toPos, region.orthoSize, blendSeconds));
        }
    }

    private System.Collections.IEnumerator CoBlend(Vector3 toPos, float toSize, float dur)
    {
        float t = 0f;
        var cam = targetCamera;
        var startPos = cam.transform.position;
        var startSize = cam.orthographicSize;

        while (t < dur)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / dur);
            cam.transform.position = Vector3.Lerp(startPos, toPos, a);
            cam.orthographicSize = Mathf.Lerp(startSize, toSize, a);
            yield return null;
        }
        cam.transform.position = toPos;
        cam.orthographicSize = toSize;
    }
}
