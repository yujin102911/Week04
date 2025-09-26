// SliderHandle.cs
using UnityEngine;
using UnityEngine.EventSystems; // Event System을 사용하기 위해 추가!

// 마우스 드래그 이벤트를 받기 위한 인터페이스들을 추가합니다.
public class SliderHandle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private WorldSpaceSlider parentSlider;
    private Camera mainCamera;
<<<<<<< Updated upstream
    // private bool isDragging = false; // �� ���� �Ʒ� public ������Ƽ�� ����
=======

    // 외부 스크립트(HandleAttachment)가 이 값을 읽어서 드래그 상태를 확인합니다.
>>>>>>> Stashed changes
    public bool IsDragging { get; private set; } = false;

    private Vector3 offset;

    void Start()
    {
        parentSlider = GetComponentInParent<WorldSpaceSlider>();
        mainCamera = Camera.main;
    }

    // 드래그가 '시작'될 때 한 번 호출됩니다.
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GameManager.mode != Mode.None) return; // 기본 모드가 아닐 경우 드래그 안됨

        IsDragging = true;
        offset = transform.position - GetMouseWorldPos(eventData.position);
    }

    // 드래그하는 '동안' 계속 호출됩니다.
    public void OnDrag(PointerEventData eventData)
    {
        if (IsDragging)
        {
            parentSlider.UpdateValueFromHandlePosition(GetMouseWorldPos(eventData.position) + offset);
        }
    }

    // 드래그가 '끝'났을 때 한 번 호출됩니다. (마우스를 어디서 놓든 호출됨)
    public void OnEndDrag(PointerEventData eventData)
    {
        IsDragging = false;
    }

    private Vector3 GetMouseWorldPos(Vector2 screenPosition)
    {
<<<<<<< Updated upstream
        if (IsDragging)
        {
            parentSlider.UpdateValueFromHandlePosition(GetMouseWorldPos() + offset);
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
=======
        Vector3 mousePoint = screenPosition;
>>>>>>> Stashed changes
        mousePoint.z = mainCamera.WorldToScreenPoint(transform.position).z;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }
}