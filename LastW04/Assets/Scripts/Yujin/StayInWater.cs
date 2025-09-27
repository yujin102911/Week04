using UnityEngine;

public class StayInWater : MonoBehaviour
{
    private Collider2D waterCollider; // �� ������ �ݶ��̴�

    void Start()
    {
        GameObject[] waterZones = GameObject.FindGameObjectsWithTag("Water");

        if (waterZones.Length == 0)
        {
            Debug.LogError("���� 'Water' �±׸� ���� ������Ʈ�� �����ϴ�!");
            return;
        }

        GameObject closestWaterZone = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject waterZone in waterZones)
        {
            // ���� ���Ⱑ �ٽ�! �� �κ��� �����մϴ�. ����
            Collider2D zoneCollider = waterZone.GetComponent<Collider2D>();
            if (zoneCollider == null) continue; // �ݶ��̴��� ���� Water ���� �ǳʶݴϴ�.

            // �� �������� ��ġ�������� Water �� �ݶ��̴��� '���� ����� ����'������ �Ÿ��� ����մϴ�.
            float distance = zoneCollider.Distance(transform.GetComponent<Collider2D>()).distance;
            // ���� ������ �κ� ����

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