using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // 이벤트 시스템 사용을 위해 필수

// 마우스 드래그 이벤트를 감지하기 위한 인터페이스를 구현합니다.
public class HandleController : MonoBehaviour, IDragHandler
{
    // private으로 선언하고 인스펙터에서 직접 할당해주는 것이 더 안전합니다.
    [SerializeField]
    private Slider slider;

    // Canvas의 Render Mode가 "Screen Space - Camera"일 경우 카메라를 할당해야 합니다.
    [SerializeField]
    private Camera uiCamera;

    void Awake()
    {
        // 만약 인스펙터에서 슬라이더를 할당하지 않았다면, 부모에서 직접 찾습니다.
        if (slider == null)
        {
            slider = GetComponentInParent<Slider>();
        }

        // uiCamera가 할당되지 않았고, Canvas가 카메라를 필요로 하는 모드일 경우
        // 자동으로 메인 카메라를 찾아 할당합니다.
        if (uiCamera == null && slider.GetComponentInParent<Canvas>().renderMode == RenderMode.ScreenSpaceCamera)
        {
            uiCamera = Camera.main;
        }
    }

    // 마우스로 핸들을 드래그하는 동안 계속 호출됩니다.
    public void OnDrag(PointerEventData eventData)
    {
        // 슬라이더의 채워진 영역(Fill Area)을 기준으로 위치를 계산할 것입니다.
        RectTransform sliderRect = slider.fillRect;

        // 마우스의 스크린 좌표를 슬라이더의 로컬 좌표로 변환합니다.
        // 이 변환을 통해 슬라이더가 화면 어디에 있든 정확한 위치를 계산할 수 있습니다.
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(sliderRect, eventData.position, uiCamera, out localPoint))
        {
            // 변환된 x좌표를 슬라이더 전체 너비로 나누어 0~1 사이의 값으로 만듭니다.
            float sliderValue = Mathf.Clamp01(localPoint.x / sliderRect.rect.width);

            // 최종적으로 계산된 값을 슬라이더의 value에 적용합니다.
            slider.value = sliderValue;
        }
    }
}