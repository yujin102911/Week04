using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private Camera cam;

    [Header("Zoom Settings")]
    [Tooltip("일반 모드 카메라 크기 (기본 뷰)")]
    [SerializeField] private float zoomNormal = 5f;

    [Tooltip("에디팅 모드 카메라 크기 (줌아웃)")]
    [SerializeField] private float zoomEditing = 8f;

    [Tooltip("부드러운 전환 속도")]
    [SerializeField] private float zoomSpeed = 5f;

    private float targetSize;

    void Awake()
    {
        cam = GetComponent<Camera>();
        targetSize = cam.orthographicSize;
    }

    void Update()
    {
        // 에디팅 모드일 때 → 줌아웃(값 더 크게)
        if (GameManager.mode == Mode.Editing)
            targetSize = zoomEditing;
        else
            targetSize = zoomNormal;

        // 현재 사이즈를 목표 사이즈로 부드럽게 보간
        if (Mathf.Abs(cam.orthographicSize - targetSize) > 0.01f)
        {
            cam.orthographicSize = Mathf.Lerp(
                cam.orthographicSize,
                targetSize,
                Time.deltaTime * zoomSpeed
            );
        }
    }
}
