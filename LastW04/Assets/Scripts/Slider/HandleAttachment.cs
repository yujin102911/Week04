using UnityEngine;

public class HandleAttachment : MonoBehaviour
{
    // --- ���� ������ ---
    private WorldSpaceSlider parentSlider;
    private SliderHandle sliderHandle;
    private Transform attachedObjectTransform;
    private Rigidbody2D attachedObjectRigidbody;

    // --- ���� ������ ---
    private bool isObjectInside = false;
    private bool wasHandleDraggingLastFrame = false;

    void Start()
    {
        parentSlider = GetComponentInParent<WorldSpaceSlider>();
        sliderHandle = GetComponentInChildren<SliderHandle>();
    }

    void Update()
    {
        // ���� �ȿ� ������Ʈ�� ������ �ƹ��͵� ���� �ʽ��ϴ�.
        if (!isObjectInside) return;

        // ���� �� ������ġ �ڵ带 �߰��ϼ���! ����
        // ���� �پ��ִ� �ڽ��� �ٸ� ��ũ��Ʈ�� ���� �θ� ���谡 �����Ǿ��ٸ�,
        // ���⼭�� ���¸� �ʱ�ȭ���ݴϴ�.
        if (attachedObjectTransform != null && attachedObjectTransform.parent != this.transform)
        {
            OnTriggerExit2D(attachedObjectTransform.GetComponent<Collider2D>());
            return; // ��� ����
        }
        // ���� ������ġ �ڵ� �� ����

        // --- �Ʒ��� ���� �巡�� ���� �ڵ� ---
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
            // ���� �� �κ��� ���� �ذ��� �ٽ��Դϴ�! ����
            // sliderHandle�� �����ϴ��� ���� Ȯ���Ͽ� Null ������ �����մϴ�.
            if (sliderHandle != null && sliderHandle.IsDragging)
            {
                LockAttachedObject(false);
            }
            // ���� �ٽ� ���� �κ� ����

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

        // �ڽ��� �׻� Kinematic�� �����ؾ� �ϹǷ�, �� �Լ��� ��ǻ� �ʿ� ������
        // �ٸ� ������Ʈ���� ȣȯ���� ���� ���ܵӴϴ�.
        // Lock�� �ʿ��ϴٸ� ���⿡ Kinematic���� �ٲٴ� �ڵ带 ���� �� �ֽ��ϴ�.
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