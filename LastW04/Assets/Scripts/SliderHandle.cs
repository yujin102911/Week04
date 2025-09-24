using UnityEngine;

public class SliderHandle : MonoBehaviour
{
    private WorldSpaceSlider parentSlider;
    private Camera mainCamera;

    void Start()
    {
        // 자신의 부모에게서 WorldSpaceSlider 스크립트를 찾아옵니다.
        parentSlider = GetComponentInParent<WorldSpaceSlider>();
        mainCamera = Camera.main;
    }

    // 이 오브젝트의 콜라이더 위에서 마우스를 드래그하는 동안 계속 호출됩니다.
    private void OnMouseDrag()
    {
        if (parentSlider != null)
        {
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            // 마우스 위치를 부모 슬라이더에게 전달하여 값을 업데이트하도록 요청합니다.
            parentSlider.UpdateValueFromHandlePosition(mousePos);
        }
    }
}