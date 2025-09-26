using UnityEngine;
using System.Collections.Generic; // Dictionary를 사용하기 위해 추가

public class AttachPlayerToPlatform : MonoBehaviour
{
    private int lotusLayer; // 'Lotus' 레이어의 번호를 저장할 변수

    // 여러 객체(플레이어, 박스 등)의 원래 레이어를 저장하기 위한 Dictionary
    // Key: 트리거에 들어온 객체의 Collider2D
    // Value: 해당 객체의 원래 레이어 번호
    private Dictionary<Collider2D, int> originalLayers = new Dictionary<Collider2D, int>();

    void Awake()
    {
        // 이름으로 "Lotus" 레이어 번호를 찾아 저장합니다.
        // 이 레이어는 Physics 2D 설정에서 'Water'와 충돌하지 않도록 설정되어야 합니다.
        lotusLayer = LayerMask.NameToLayer("Lotus");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어 또는 박스가 들어왔을 때
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            if (originalLayers.ContainsKey(other)) return;

            originalLayers.Add(other, other.gameObject.layer);
            other.gameObject.layer = lotusLayer;
            other.transform.SetParent(this.transform);

            // ▼▼▼ 추가된 부분 ▼▼▼
            // 만약 들어온 것이 플레이어라면, PlayerMove 스크립트에게 플랫폼에 올라탔다고 알려줍니다.
            if (other.CompareTag("Player"))
            {
                PlayerMove playerMove = other.GetComponent<PlayerMove>();
                if (playerMove != null)
                {
                    playerMove.IsOnPlatform = true;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 플레이어 또는 박스가 나갔을 때
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            if (originalLayers.ContainsKey(other))
            {
                // ▼▼▼ 추가된 부분 ▼▼▼
                // 만약 나가는 것이 플레이어라면, PlayerMove 스크립트에게 플랫폼에서 내렸다고 알려줍니다.
                if (other.CompareTag("Player"))
                {
                    PlayerMove playerMove = other.GetComponent<PlayerMove>();
                    if (playerMove != null)
                    {
                        playerMove.IsOnPlatform = false;
                    }
                }

                other.gameObject.layer = originalLayers[other];
                originalLayers.Remove(other);
                other.transform.SetParent(null);
            }
        }
    }
}

