using UnityEngine;

public class HandleAttachment : MonoBehaviour
{
    // --- 참조 변수들 ---
    private WorldSpaceSlider parentSlider;
    private SliderHandle sliderHandle;
    private Transform attachedObjectTransform;
    private Rigidbody2D attachedObjectRigidbody;

    // --- 상태 변수들 ---
    private bool isObjectInside = false;
    private bool wasHandleDraggingLastFrame = false;

    void Start()
    {
        parentSlider = GetComponentInParent<WorldSpaceSlider>();
        sliderHandle = GetComponentInChildren<SliderHandle>();
    }

    void Update()
    {
        if (!isObjectInside || sliderHandle == null) return;

        bool isHandleDraggingThisFrame = sliderHandle.IsDragging;

        if (isHandleDraggingThisFrame && !wasHandleDraggingLastFrame)
        {
            LockAttachedObject(true);
        }
        else if (!isHandleDraggingThisFrame && wasHandleDraggingLastFrame)
        {
            LockAttachedObject(false);
        }

        wasHandleDraggingLastFrame = isHandleDraggingThisFrame;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (parentSlider == null || !parentSlider.IsInstalled)
        {
            return;
        }

        if (other.CompareTag("Box"))
        {
            isObjectInside = true;
            attachedObjectTransform = other.transform;
            attachedObjectRigidbody = other.GetComponent<Rigidbody2D>();
            attachedObjectTransform.SetParent(this.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Box"))
        {
            // ▼▼▼ 이 부분이 오류 해결의 핵심입니다! ▼▼▼
            // sliderHandle이 존재하는지 먼저 확인하여 Null 오류를 방지합니다.
            if (sliderHandle != null && sliderHandle.IsDragging)
            {
                LockAttachedObject(false);
            }
            // ▲▲▲ 핵심 수정 부분 ▲▲▲

            isObjectInside = false;
            if (attachedObjectTransform != null)
            {
                attachedObjectTransform.SetParent(null);
            }

            attachedObjectTransform = null;
            attachedObjectRigidbody = null;
        }
    }

    private void LockAttachedObject(bool shouldLock)
    {
        if (attachedObjectRigidbody == null) return;

        // 박스는 항상 Kinematic을 유지해야 하므로, 이 함수는 사실상 필요 없지만
        // 다른 오브젝트와의 호환성을 위해 남겨둡니다.
        // Lock이 필요하다면 여기에 Kinematic으로 바꾸는 코드를 넣을 수 있습니다.
    }

    void OnDestroy()
    {
        if (attachedObjectTransform != null)
        {
            LockAttachedObject(false);
            attachedObjectTransform.SetParent(null);
        }
    }
}