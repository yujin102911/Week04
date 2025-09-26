// HandleAttachment.cs
using UnityEngine;

public class HandleAttachment : MonoBehaviour
{
    [Header("슬라이더 핸들 참조")]
    [Tooltip("인스펙터에서 실제 핸들 오브젝트의 Transform을 연결해주세요.")]
    [SerializeField] private Transform handleTransform;

    private SliderHandle sliderHandle;
    private PushableBox attachedBox;

    void Start()
    {
        sliderHandle = GetComponentInChildren<SliderHandle>();
        if (sliderHandle == null)
        {
            Debug.LogError("자식 오브젝트에서 SliderHandle 컴포넌트를 찾을 수 없습니다!", this.gameObject);
        }
        if (handleTransform == null)
        {
            Debug.LogError("handleTransform이 인스펙터에 할당되지 않았습니다!", this.gameObject);
        }
    }

    void Update()
    {
        if (handleTransform == null || sliderHandle == null || attachedBox == null) return;

        // ▼▼▼ 로직 대폭 단순화 ▼▼▼
        if (sliderHandle.IsDragging)
        {
            // 1. 핸들의 현재 위치를 박스에게 계속 알려줍니다.
            attachedBox.MoveWithHandle(handleTransform.position);

            // 2. 박스를 밀고 난 후 핸들의 위치를 박스에 다시 맞춥니다.
            //    이것은 박스가 벽에 막혔을 때 핸들도 멈추게 하는 중요한 역할입니다.
            handleTransform.position = attachedBox.transform.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Box") && attachedBox == null)
        {
            if (other.TryGetComponent(out PushableBox box))
            {
                Attach(box);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (attachedBox != null && other.gameObject == attachedBox.gameObject && sliderHandle != null)
        {
            if (!sliderHandle.IsDragging)
            {
                Detach();
            }
        }
    }

    private void Attach(PushableBox box)
    {
        if (handleTransform == null) return;
        attachedBox = box;
        handleTransform.position = box.transform.position;
    }

    private void Detach()
    {
        attachedBox = null;
    }

    void OnDestroy()
    {
        Detach();
    }
}