// WaterZone.cs
using UnityEngine;

public class WaterZone : MonoBehaviour
{
    // 다른 콜라이더가 이 트리거 영역에서 '나갔을 때' 호출됩니다.
    private void OnTriggerExit2D(Collider2D other)
    {
        // 나간 오브젝트의 태그가 "Lotus" 라면
        if (other.CompareTag("Lotus"))
        {
            // 해당 연꽃 오브젝트를 파괴합니다.
            Destroy(other.gameObject);
        }
    }
}