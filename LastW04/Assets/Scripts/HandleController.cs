using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // �̺�Ʈ �ý��� ����� ���� �ʼ�

// ���콺 �巡�� �̺�Ʈ�� �����ϱ� ���� �������̽��� �����մϴ�.
public class HandleController : MonoBehaviour, IDragHandler
{
    // private���� �����ϰ� �ν����Ϳ��� ���� �Ҵ����ִ� ���� �� �����մϴ�.
    [SerializeField]
    private Slider slider;

    // Canvas�� Render Mode�� "Screen Space - Camera"�� ��� ī�޶� �Ҵ��ؾ� �մϴ�.
    [SerializeField]
    private Camera uiCamera;

    void Awake()
    {
        // ���� �ν����Ϳ��� �����̴��� �Ҵ����� �ʾҴٸ�, �θ𿡼� ���� ã���ϴ�.
        if (slider == null)
        {
            slider = GetComponentInParent<Slider>();
        }

        // uiCamera�� �Ҵ���� �ʾҰ�, Canvas�� ī�޶� �ʿ�� �ϴ� ����� ���
        // �ڵ����� ���� ī�޶� ã�� �Ҵ��մϴ�.
        if (uiCamera == null && slider.GetComponentInParent<Canvas>().renderMode == RenderMode.ScreenSpaceCamera)
        {
            uiCamera = Camera.main;
        }
    }

    // ���콺�� �ڵ��� �巡���ϴ� ���� ��� ȣ��˴ϴ�.
    public void OnDrag(PointerEventData eventData)
    {
        // �����̴��� ä���� ����(Fill Area)�� �������� ��ġ�� ����� ���Դϴ�.
        RectTransform sliderRect = slider.fillRect;

        // ���콺�� ��ũ�� ��ǥ�� �����̴��� ���� ��ǥ�� ��ȯ�մϴ�.
        // �� ��ȯ�� ���� �����̴��� ȭ�� ��� �ֵ� ��Ȯ�� ��ġ�� ����� �� �ֽ��ϴ�.
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(sliderRect, eventData.position, uiCamera, out localPoint))
        {
            // ��ȯ�� x��ǥ�� �����̴� ��ü �ʺ�� ������ 0~1 ������ ������ ����ϴ�.
            float sliderValue = Mathf.Clamp01(localPoint.x / sliderRect.rect.width);

            // ���������� ���� ���� �����̴��� value�� �����մϴ�.
            slider.value = sliderValue;
        }
    }
}