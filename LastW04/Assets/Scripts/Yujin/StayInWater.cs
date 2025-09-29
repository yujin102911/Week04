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
        // waterCollider 변수가 비어있으면 아무것도 하지 않습니다.
        if (waterCollider == null) return;

        Vector3 currentPosition = transform.position;

        // 1. 현재 연꽃잎의 위치가 물 콜라이더의 '실제 모양' 안에 있는지 확인합니다.
        //    (Bounds가 아닌, PolygonCollider나 CompositeCollider의 모양을 직접 확인)
        if (waterCollider.OverlapPoint(currentPosition))
        {
            // 모양 안에 있다면, 위치를 바꿀 필요가 없으므로 여기서 함수를 종료합니다.
            return;
        }
        else
        {
            // 2. 만약 모양 밖에 있다면, 콜라이더의 가장자리 중에서 현재 위치와 가장 가까운 지점을 찾습니다.
            Vector3 closestPointOnEdge = waterCollider.ClosestPoint(currentPosition);

            // 3. 연꽃잎을 그 가장 가까운 지점으로 즉시 이동시켜서 탈출을 막습니다.
            transform.position = closestPointOnEdge;
        }
    }
}