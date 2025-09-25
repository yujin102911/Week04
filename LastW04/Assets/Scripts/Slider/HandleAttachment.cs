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
        // 영역 안에 오브젝트가 없으면 아무것도 하지 않습니다.
        if (!isObjectInside) return;

        // ▼▼▼ 이 안전장치 코드를 추가하세요! ▼▼▼
        // 만약 붙어있던 박스가 다른 스크립트에 의해 부모 관계가 해제되었다면,
        // 여기서도 상태를 초기화해줍니다.
        if (attachedObjectTransform != null && attachedObjectTransform.parent != this.transform)
        {
            OnTriggerExit2D(attachedObjectTransform.GetComponent<Collider2D>());
            return; // 즉시 종료
        }
        // ▲▲▲ 안전장치 코드 끝 ▲▲▲

        // --- 아래는 기존 드래그 감지 코드 ---
        if (sliderHandle == null) return;

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

        if (other.CompareTag("Box") || other.CompareTag("Lotus"))
        {
            isObjectInside = true;
            attachedObjectTransform = other.transform;
            attachedObjectRigidbody = other.GetComponent<Rigidbody2D>();
            attachedObjectTransform.SetParent(this.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Box")|| other.CompareTag("Lotus"))
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