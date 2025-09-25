using UnityEngine;

public class StayInWater : MonoBehaviour
{
    private Collider2D waterCollider; // �� ������ �ݶ��̴�

    void Start()
    {
        // "Water" �±׸� ���� ������Ʈ�� ã�Ƽ� �� �ݶ��̴��� �����ɴϴ�.
        GameObject waterZone = GameObject.FindGameObjectWithTag("Water");
        if (waterZone != null)
        {
            waterCollider = waterZone.GetComponent<Collider2D>();
        }
        else
        {
            Debug.LogError("���� 'Water' �±׸� ���� ������Ʈ�� �����ϴ�!");
        }
    }

    // ��� Update �Լ��� ����� �� �������� ȣ��˴ϴ�.
    // ������Ʈ�� ���� ��ġ�� �����ϱ⿡ ���� �����մϴ�.
    void LateUpdate()
    {
        if (waterCollider == null) return;

        // �� ������ ���(Bounds)�� �����ɴϴ�.
        Bounds waterBounds = waterCollider.bounds;

        // ���� ������ ��ġ�� �����ɴϴ�.
        Vector3 currentPosition = transform.position;

        // Mathf.Clamp�� ����Ͽ� ������ x, y ��ǥ�� ���� ��踦 ����� �ʵ��� �����մϴ�.
        float clampedX = Mathf.Clamp(currentPosition.x, waterBounds.min.x, waterBounds.max.x);
        float clampedY = Mathf.Clamp(currentPosition.y, waterBounds.min.y, waterBounds.max.y);

        // ���������� ������ ��ġ�� ������ ��ġ�� �ٽ� �����մϴ�.
        transform.position = new Vector3(clampedX, clampedY, currentPosition.z);
    }
}