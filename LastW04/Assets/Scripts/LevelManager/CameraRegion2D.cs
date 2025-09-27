using UnityEngine;

[DisallowMultipleComponent]
public class CameraRegion2D : MonoBehaviour
{
    [Tooltip("�� ������ ���� ID (�ڷ���Ʈ/������ ���� Ű)")]
    public string regionId;

    [Header("Framing")]
    [Tooltip("ī�޶� �ٶ� �߽���(���� ����)")]
    public Transform pivot;
    [Tooltip("ī�޶� orthographicSize ��")]
    public float orthoSize;

    [Header("Spawn")]
    [Tooltip("�� ������ �⺻ ���� ����")]
    public Transform defaultSpawn;

}
