using UnityEngine;

public class DestroyOnRightClick : MonoBehaviour
{

    // 마우스 커서가 이 오브젝트의 콜라이더 위에 있는 동안 매 프레임 호출됩니다.
    private void OnMouseOver()
    {
        // 만약 마우스 오른쪽 버튼을 "누르는 순간"이라면
        if (Input.GetMouseButtonDown(1))
        {
            // 이 오브젝트의 부모 계층에서 WorldSpaceSlider 컴포넌트를 찾습니다.
            WorldSpaceSlider rootSlider = GetComponentInParent<WorldSpaceSlider>();

            // 찾았다면, 그 컴포넌트가 붙어있는 최상위 게임 오브젝트를 파괴합니다.
            if (rootSlider != null)
            {
                Destroy(rootSlider.gameObject);
            }
        }
    }
}