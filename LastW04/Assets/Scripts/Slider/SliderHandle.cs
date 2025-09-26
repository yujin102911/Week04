// SliderHandle.cs
using UnityEngine;

public class SliderHandle : MonoBehaviour
{
    private WorldSpaceSlider parentSlider;
    private Camera mainCamera;
    // private bool isDragging = false; // 이 줄을 아래 public 프로퍼티로 변경
    public bool IsDragging { get; private set; } = false;
    private Vector3 offset;

    void Start()
    {
        parentSlider = GetComponentInParent<WorldSpaceSlider>();
        mainCamera = Camera.main;
    }

    private void OnMouseDown()
    {
        IsDragging = true;
        offset = transform.position - GetMouseWorldPos();
    }

    private void OnMouseUp()
    {
        IsDragging = false;
    }

    void Update()
    {
        if (IsDragging)
        {
            if(GameManager.mode==Mode.None)//기본 모드일때만 이동
            parentSlider.UpdateValueFromHandlePosition(GetMouseWorldPos() + offset);
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mainCamera.WorldToScreenPoint(transform.position).z;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }
}