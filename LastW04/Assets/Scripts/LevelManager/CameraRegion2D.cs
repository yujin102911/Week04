using UnityEngine;

[DisallowMultipleComponent]
public class CameraRegion2D : MonoBehaviour
{
    [Tooltip("이 지역의 고유 ID (텔레포트/리스폰 기준 키)")]
    public string regionId;

    [Header("Framing")]
    [Tooltip("카메라가 바라볼 중심점(비우면 본인)")]
    public Transform pivot;
    [Tooltip("카메라 orthographicSize 값")]
    public float orthoSize;

    [Header("Spawn")]
    [Tooltip("이 지역의 기본 스폰 지점")]
    public Transform defaultSpawn;

}
