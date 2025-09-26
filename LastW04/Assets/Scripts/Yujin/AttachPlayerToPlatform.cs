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
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            other.transform.SetParent(this.transform);

            if (other.CompareTag("Player"))
            {
                other.gameObject.layer = lotusLayer;
            }
            // ▼▼▼ [추가됨] 박스가 닿으면, 박스의 상태를 '연꽃 위'로 변경 ▼▼▼
            else if (other.CompareTag("Box"))
            {
                if (other.TryGetComponent(out PushableBox box))
                {
                    box.SetOnLotus(true);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            other.transform.SetParent(null);

            if (other.CompareTag("Player"))
            {
                other.gameObject.layer = playerLayer;
            }
            // ▼▼▼ [추가됨] 박스가 떠나면, 박스의 상태를 '연꽃 위 아님'으로 변경 ▼▼▼
            else if (other.CompareTag("Box"))
            {
                if (other.TryGetComponent(out PushableBox box))
                {
                    box.SetOnLotus(false);
                }
            }
        }
    }
}