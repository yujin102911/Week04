using UnityEngine;

public class HandleAttachment : MonoBehaviour
{

    // 다른 콜라이더(Trigger)가 이 오브젝트의 트리거 영역으로 들어왔을 때 한 번 호출됩니다.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 들어온 오브젝트가 'Player' 태그를 가지고 있는지 확인합니다.
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어가 핸들 영역에 진입. 부모로 설정합니다.");
            // 플레이어(other)의 부모를 이 핸들(this.transform)로 설정합니다.
            other.transform.SetParent(this.transform);
        }
    }

    // 다른 콜라이더(Trigger)가 이 오브젝트의 트리거 영역에서 나갔을 때 한 번 호출됩니다.
    private void OnTriggerExit2D(Collider2D other)
    {
        // 나간 오브젝트가 'Player' 태그를 가지고 있는지 확인합니다.
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어가 핸들 영역을 이탈. 부모-자식 관계를 해제합니다.");
            // 플레이어의 부모-자식 관계를 해제하여 월드의 최상위 계층으로 돌려놓습니다.
            other.transform.SetParent(null);
        }
    }
}