using UnityEngine;

public class AttachPlayerToPlatform : MonoBehaviour
{
    private int playerLayer; // 'Player' 레이어의 번호를 저장할 변수
    private int lotusLayer;  // 'Lotus' 레이어의 번호를 저장할 변수

    void Awake()
    {
        // 이름으로 레이어 번호를 찾아 저장해둡니다.
        // 이렇게 하면 나중에 레이어 순서가 바뀌어도 코드를 수정할 필요가 없습니다.
        playerLayer = LayerMask.NameToLayer("Player");
        lotusLayer = LayerMask.NameToLayer("Lotus");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ▼▼▼ 이 줄을 추가하세요! ▼▼▼
            // 플레이어의 레이어를 'Lotus' 레이어로 잠시 변경합니다.
            other.gameObject.layer = lotusLayer;

            // 플레이어를 이 플랫폼(연꽃)의 자식으로 만듭니다.
            other.transform.SetParent(this.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ▼▼▼ 이 줄을 추가하세요! ▼▼▼
            // 플레이어의 레이어를 원래 'Player' 레이어로 되돌립니다.
            other.gameObject.layer = playerLayer;

            // 플레이어의 부모-자식 관계를 해제합니다.
            other.transform.SetParent(null);
        }
    }
}