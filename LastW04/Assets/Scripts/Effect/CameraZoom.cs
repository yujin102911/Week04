using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private Camera cam;
    private UIEditor uiEditor;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomEditing;
    [SerializeField] private float zoomNormal;
    [SerializeField] private float zoomSpeed; // 부드러운 전환 속도

    private float targetSize;

    void Awake()
    {
        cam = GetComponent<Camera>();
        uiEditor = FindObjectOfType<UIEditor>(); // 씬에서 UIEditor 찾아옴
        targetSize = cam.orthographicSize;
    }

    void Update()
    {
        if (uiEditor == null) return;

        // UIEditor의 모드 상태 확인해서 목표 줌 크기 결정
        if (uiEditor.mode == Mode.Editing)
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
