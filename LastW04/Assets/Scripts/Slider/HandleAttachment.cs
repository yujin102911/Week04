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

    [Header("논리 설정")]
    [SerializeField] private LayerMask waterLayer; // Inspector에서 "Water" 레이어를 할당해주세요.
    private Vector3 lastValidPosition; // 물에 들어가기 직전의 위치를 저장

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
    void FixedUpdate()
    {
        if (attachedObjectTransform != null)
        {
            // 박스를 잡고 있을 때만 위치를 기록합니다.
            lastValidPosition = attachedObjectTransform.position;
        }
    }
    private void LateUpdate()
    {
        // 박스를 잡고 있을 때만 검사
        if (attachedObjectTransform != null && attachedObjectTransform.CompareTag("Box"))
        {
            // 박스의 현재 위치에 물이 있는지 확인
            if (Physics2D.OverlapCircle(attachedObjectTransform.position, 0.2f, waterLayer))
            {
                PushableBox box = attachedObjectTransform.GetComponent<PushableBox>();
                // 박스가 있고, 연꽃 위에 있지 않다면
                if (box != null && !box.IsOnLotus)
                {
                    Debug.Log("박스는 물에 들어갈 수 없습니다. 핸들에서 분리합니다.");

                    // 핸들과의 연결을 끊고
                    attachedObjectTransform.SetParent(null);
                    // 물에 들어가기 직전의 안전한 위치로 박스를 이동
                    attachedObjectTransform.position = lastValidPosition;

                    // 핸들의 상태를 초기화하여 다른 객체를 잡을 수 있게 함
                    isObjectInside = false;
                    attachedObjectTransform = null;
                    attachedObjectRigidbody = null;
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (parentSlider == null || !parentSlider.IsInstalled)
        {
            return;
        }

        // ▼▼▼ [수정됨] 이미 무언가를 잡고 있다면 새로 잡지 않음 ▼▼▼
        if (attachedObjectTransform != null)
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
        // ▼▼▼ [수정됨] 나간 객체가 현재 잡고 있는 객체와 다를 수 있으므로, 정확히 일치하는지 확인 ▼▼▼
        if (other.transform == attachedObjectTransform)
        {
            if (sliderHandle != null && sliderHandle.IsDragging)
            {
                LockAttachedObject(false);
            }

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