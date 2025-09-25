using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    // 플레이어가 발판 위에 올라왔을 때 호출됩니다.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌한 오브젝트가 'Player' 태그를 가지고 있는지 확인합니다.
        if (collision.gameObject.CompareTag("Player"))
        {
            // 플레이어가 발판의 '위쪽'에서 충돌했는지 확인합니다.
            // 충돌 지점의 법선(normal) 벡터의 y값이 음수이면 위에서 충돌한 것입니다.
            ContactPoint2D contact = collision.GetContact(0);
            if (contact.normal.y < -0.5f)
            {
                // 플레이어를 이 발판(transform)의 자식으로 만듭니다.
                collision.transform.SetParent(this.transform);
            }
        }
    }

    // 플레이어가 발판에서 내려갔을 때 호출됩니다.
    private void OnCollisionExit2D(Collision2D collision)
    {
        // 충돌이 끝난 오브젝트가 'Player' 태그를 가지고 있는지 확인합니다.
        if (collision.gameObject.CompareTag("Player"))
        {
            // 플레이어의 부모-자식 관계를 해제하여 다시 월드의 최상단으로 옮깁니다.
            collision.transform.SetParent(null);
        }
    }
}