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
        // waterCollider ������ ��������� �ƹ��͵� ���� �ʽ��ϴ�.
        if (waterCollider == null) return;

        Vector3 currentPosition = transform.position;

        // 1. ���� �������� ��ġ�� �� �ݶ��̴��� '���� ���' �ȿ� �ִ��� Ȯ���մϴ�.
        //    (Bounds�� �ƴ�, PolygonCollider�� CompositeCollider�� ����� ���� Ȯ��)
        if (waterCollider.OverlapPoint(currentPosition))
        {
            // ��� �ȿ� �ִٸ�, ��ġ�� �ٲ� �ʿ䰡 �����Ƿ� ���⼭ �Լ��� �����մϴ�.
            return;
        }
        else
        {
            // 2. ���� ��� �ۿ� �ִٸ�, �ݶ��̴��� �����ڸ� �߿��� ���� ��ġ�� ���� ����� ������ ã���ϴ�.
            Vector3 closestPointOnEdge = waterCollider.ClosestPoint(currentPosition);

            // 3. �������� �� ���� ����� �������� ��� �̵����Ѽ� Ż���� �����ϴ�.
            transform.position = closestPointOnEdge;
        }
    }
}