using UnityEngine;

public class StayInWater : MonoBehaviour
{
    private Collider2D waterCollider; // 물 영역의 콜라이더

    void Start()
    {
        GameObject[] waterZones = GameObject.FindGameObjectsWithTag("Water");

        if (waterZones.Length == 0)
        {
            Debug.LogError("씬에 'Water' 태그를 가진 오브젝트가 없습니다!");
            return;
        }

        GameObject closestWaterZone = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject waterZone in waterZones)
        {
            // ▼▼▼ 여기가 핵심! 이 부분을 수정합니다. ▼▼▼
            Collider2D zoneCollider = waterZone.GetComponent<Collider2D>();
            if (zoneCollider == null) continue; // 콜라이더가 없는 Water 존은 건너뜁니다.

            // 이 연꽃잎의 위치에서부터 Water 존 콜라이더의 '가장 가까운 지점'까지의 거리를 계산합니다.
            float distance = zoneCollider.Distance(transform.GetComponent<Collider2D>()).distance;
            // ▲▲▲ 수정된 부분 ▲▲▲

            if (distance < minDistance)
            {
                minDistance = distance;
                closestWaterZone = waterZone;
            }
        }

        if (closestWaterZone != null)
        {
            waterCollider = closestWaterZone.GetComponent<Collider2D>();
        }
    }

    void LateUpdate()
    {
        if (waterCollider == null) return;

        Bounds waterBounds = waterCollider.bounds;
        Vector3 currentPosition = transform.position;

        float clampedX = Mathf.Clamp(currentPosition.x, waterBounds.min.x, waterBounds.max.x);
        float clampedY = Mathf.Clamp(currentPosition.y, waterBounds.min.y, waterBounds.max.y);

        transform.position = new Vector3(clampedX, clampedY, currentPosition.z);
    }
}