// HandleAttachment.cs
using UnityEngine;

public class HandleAttachment : MonoBehaviour
{
    [Header("�����̴� �ڵ� ����")]
    [Tooltip("�ν����Ϳ��� ���� �ڵ� ������Ʈ�� Transform�� �������ּ���.")]
    [SerializeField] private Transform handleTransform;

    private SliderHandle sliderHandle;
    private PushableBox attachedBox;

    void Start()
    {
        sliderHandle = GetComponentInChildren<SliderHandle>();
        if (sliderHandle == null)
        {
            Debug.LogError("�ڽ� ������Ʈ���� SliderHandle ������Ʈ�� ã�� �� �����ϴ�!", this.gameObject);
        }
        if (handleTransform == null)
        {
            Debug.LogError("handleTransform�� �ν����Ϳ� �Ҵ���� �ʾҽ��ϴ�!", this.gameObject);
        }
    }

    void Update()
    {
        if (handleTransform == null || sliderHandle == null || attachedBox == null) return;

        // ���� ���� ���� �ܼ�ȭ ����
        if (sliderHandle.IsDragging)
        {
            // 1. �ڵ��� ���� ��ġ�� �ڽ����� ��� �˷��ݴϴ�.
            attachedBox.MoveWithHandle(handleTransform.position);

            // 2. �ڽ��� �а� �� �� �ڵ��� ��ġ�� �ڽ��� �ٽ� ����ϴ�.
            //    �̰��� �ڽ��� ���� ������ �� �ڵ鵵 ���߰� �ϴ� �߿��� �����Դϴ�.
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