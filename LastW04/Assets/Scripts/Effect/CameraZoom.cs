using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private Camera cam;
    private UIEditor uiEditor;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomEditing;
    [SerializeField] private float zoomNormal;
    [SerializeField] private float zoomSpeed; // �ε巯�� ��ȯ �ӵ�

    private float targetSize;

    void Awake()
    {
        cam = GetComponent<Camera>();
        uiEditor = FindObjectOfType<UIEditor>(); // ������ UIEditor ã�ƿ�
        targetSize = cam.orthographicSize;
    }

    void Update()
    {
        if (uiEditor == null) return;

        // UIEditor�� ��� ���� Ȯ���ؼ� ��ǥ �� ũ�� ����
        if (uiEditor.mode == Mode.Editing)
            targetSize = zoomEditing;
        else
            targetSize = zoomNormal;

        // ���� ����� ��ǥ ������� �ε巴�� ����
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
