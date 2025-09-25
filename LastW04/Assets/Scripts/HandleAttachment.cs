using UnityEngine;

public class HandleAttachment : MonoBehaviour
{
    // 박스가 트리거 영역에 들어왔을 때
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 들어온 것이 'Box' 태그를 가졌다면
        if (other.CompareTag("Box"))
        {
            // 간단히 핸들의 자식으로 만듭니다.
            other.transform.SetParent(this.transform);
        }
    }

    // 박스가 트리거 영역에서 나갔을 때
    private void OnTriggerExit2D(Collider2D other)
    {
        // 나간 것이 'Box' 태그를 가졌다면
        if (other.CompareTag("Box"))
        {
            // 간단히 부모-자식 관계를 해제합니다.
            other.transform.SetParent(null);
        }
    }
}