using UnityEngine;

// 이 스크립트는 디버깅 용도로만 사용하고, 문제가 해결되면 삭제해도 됩니다.
public class AlphaWatcher : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color lastColor;

    void Awake()
    {
        // 이 오브젝트의 SpriteRenderer를 가져옵니다.
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            lastColor = sr.color;
        }
    }

    void Update()
    {
        // 만약 현재 색상이 이전 프레임의 색상과 다르다면 (누군가 색을 바꿨다면)
        if (sr != null && sr.color != lastColor)
        {
            // 특히 알파 값이 0으로 바뀌었다면
            if (sr.color.a < 1.0f)
            {
                Debug.LogError("알파 값이 " + sr.color.a + "로 변경되었습니다! 범인을 찾기 위해 에디터를 일시정지합니다.", this.gameObject);

                // ▼▼▼ 이게 핵심입니다! ▼▼▼
                // 에디터를 즉시 일시정지시켜서, 어떤 코드 때문에 변경되었는지 확인할 시간을 줍니다.
                Debug.Break();
            }

            // 다음 프레임과 비교하기 위해 현재 색상을 저장합니다.
            lastColor = sr.color;
        }
    }
}