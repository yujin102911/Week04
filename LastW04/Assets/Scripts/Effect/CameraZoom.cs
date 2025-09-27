using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private Camera cam;

    [Header("Zoom Settings")]
    [Tooltip("�Ϲ� ��� ī�޶� ũ�� (�⺻ ��)")]
    [SerializeField] private float zoomNormal = 5f;

    [Tooltip("������ ��� ī�޶� ũ�� (�ܾƿ�)")]
    [SerializeField] private float zoomEditing = 8f;

    [Tooltip("�ε巯�� ��ȯ �ӵ�")]
    [SerializeField] private float zoomSpeed = 5f;

    private float targetSize;

    void Awake()
    {
        cam = GetComponent<Camera>();
        targetSize = cam.orthographicSize;
    }

    void Update()
    {
        // ������ ����� �� �� �ܾƿ�(�� �� ũ��)
        if (GameManager.mode == Mode.Editing)
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
