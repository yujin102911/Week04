using UnityEngine;

public class AttachPlayerToPlatform : MonoBehaviour
{
    private int playerLayer;
    private int lotusLayer;

    void Awake()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        lotusLayer = LayerMask.NameToLayer("LotusPad");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ▼▼▼ 이 조건을 수정합니다! ▼▼▼
        // 들어온 오브젝트의 태그가 "Player" 이거나 "Box" 라면,
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            // 오브젝트를 이 플랫폼(연꽃)의 자식으로 만듭니다.
            other.transform.SetParent(this.transform);

            // ▼▼▼ 추가된 조건문! ▼▼▼
            // 만약 들어온 것이 "Player"일 경우에만 레이어를 변경합니다.
            // (박스는 레이어를 바꿀 필요가 없으므로)
            if (other.CompareTag("Player"))
            {
                other.gameObject.layer = lotusLayer;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // ▼▼▼ 여기도 똑같이 수정합니다! ▼▼▼
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            // 오브젝트의 부모-자식 관계를 해제합니다.
            other.transform.SetParent(null);

            // ▼▼▼ 추가된 조건문! ▼▼▼
            // 나가는 것이 "Player"일 경우에만 레이어를 원래대로 되돌립니다.
            if (other.CompareTag("Player"))
            {
                other.gameObject.layer = playerLayer;
            }
        }
    }
}