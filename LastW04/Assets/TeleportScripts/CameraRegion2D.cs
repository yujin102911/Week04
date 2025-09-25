using UnityEngine;

public class CameraRegion2D : MonoBehaviour
{
    [Tooltip("�� ������ ���� ID (�ڷ���Ʈ Ʈ���ſ��� ����)")]
    public string regionId = "Region_01";

    [Header("Framing")]
    [Tooltip("ī�޶� �ٶ� �߽���(���� �� �߾�). ���� �� ������Ʈ�� Ʈ������ ���")]
    public Transform pivot;
    [Tooltip("ī�޶� orthographicSize ��")]
    public float orthoSize = 10f;

    private void OnValidate()
    {
        if (!pivot) pivot = transform;
    }
}
