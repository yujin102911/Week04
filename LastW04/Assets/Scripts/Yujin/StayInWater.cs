using UnityEngine;

public class StayInWater : MonoBehaviour
{
    private Collider2D waterCollider; // 물 영역의 콜라이더

    void Start()
    {
        // "Water" 태그를 가진 오브젝트를 찾아서 그 콜라이더를 가져옵니다.
        GameObject waterZone = GameObject.FindGameObjectWithTag("Water");
        if (waterZone != null)
        {
            waterCollider = waterZone.GetComponent<Collider2D>();
        }
        else
        {
            Debug.LogError("씬에 'Water' 태그를 가진 오브젝트가 없습니다!");
        }
    }

    // 모든 Update 함수가 실행된 후 마지막에 호출됩니다.
    // 오브젝트의 최종 위치를 보정하기에 가장 적합합니다.
    void LateUpdate()
    {
        if (waterCollider == null) return;

        // 물 영역의 경계(Bounds)를 가져옵니다.
        Bounds waterBounds = waterCollider.bounds;

        // 현재 연꽃의 위치를 가져옵니다.
        Vector3 currentPosition = transform.position;

        // Mathf.Clamp를 사용하여 연꽃의 x, y 좌표가 물의 경계를 벗어나지 않도록 제한합니다.
        float clampedX = Mathf.Clamp(currentPosition.x, waterBounds.min.x, waterBounds.max.x);
        float clampedY = Mathf.Clamp(currentPosition.y, waterBounds.min.y, waterBounds.max.y);

        // 최종적으로 보정된 위치를 연꽃의 위치에 다시 적용합니다.
        transform.position = new Vector3(clampedX, clampedY, currentPosition.z);
    }
}