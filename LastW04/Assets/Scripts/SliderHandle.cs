using UnityEngine;

public class SliderHandle : MonoBehaviour
{
    private WorldSpaceSlider parentSlider;
    private Camera mainCamera;

    void Start()
    {
        // �ڽ��� �θ𿡰Լ� WorldSpaceSlider ��ũ��Ʈ�� ã�ƿɴϴ�.
        parentSlider = GetComponentInParent<WorldSpaceSlider>();
        mainCamera = Camera.main;
    }

    // �� ������Ʈ�� �ݶ��̴� ������ ���콺�� �巡���ϴ� ���� ��� ȣ��˴ϴ�.
    private void OnMouseDrag()
    {
        if (parentSlider != null)
        {
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            // ���콺 ��ġ�� �θ� �����̴����� �����Ͽ� ���� ������Ʈ�ϵ��� ��û�մϴ�.
            parentSlider.UpdateValueFromHandlePosition(mousePos);
        }
    }
}