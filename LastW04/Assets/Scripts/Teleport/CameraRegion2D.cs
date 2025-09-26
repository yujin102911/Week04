using UnityEngine;

public class CameraRegion2D : MonoBehaviour
{
    [Tooltip("이 지역의 고유 ID (텔레포트 트리거에서 참조)")]
    public string regionId = "Region_01";

    [Header("Framing")]
    [Tooltip("카메라가 바라볼 중심점(보통 맵 중앙). 비우면 이 오브젝트의 트랜스폼 사용")]
    public Transform pivot;
    [Tooltip("카메라 orthographicSize 값")]
    public float orthoSize = 10f;

    private void OnValidate()
    {
        if (!pivot) pivot = transform;
    }
}
