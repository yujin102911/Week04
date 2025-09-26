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

    [Header("�� ����")]
    [SerializeField] private LayerMask waterLayer; // Inspector���� "Water" ���̾ �Ҵ����ּ���.
    private Vector3 lastValidPosition; // ���� ���� ������ ��ġ�� ����

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
    void FixedUpdate()
    {
        if (attachedObjectTransform != null)
        {
            // �ڽ��� ��� ���� ���� ��ġ�� ����մϴ�.
            lastValidPosition = attachedObjectTransform.position;
        }
    }
    private void LateUpdate()
    {
        // �ڽ��� ��� ���� ���� �˻�
        if (attachedObjectTransform != null && attachedObjectTransform.CompareTag("Box"))
        {
            // �ڽ��� ���� ��ġ�� ���� �ִ��� Ȯ��
            if (Physics2D.OverlapCircle(attachedObjectTransform.position, 0.2f, waterLayer))
            {
                PushableBox box = attachedObjectTransform.GetComponent<PushableBox>();
                // �ڽ��� �ְ�, ���� ���� ���� �ʴٸ�
                if (box != null && !box.IsOnLotus)
                {
                    Debug.Log("�ڽ��� ���� �� �� �����ϴ�. �ڵ鿡�� �и��մϴ�.");

                    // �ڵ���� ������ ����
                    attachedObjectTransform.SetParent(null);
                    // ���� ���� ������ ������ ��ġ�� �ڽ��� �̵�
                    attachedObjectTransform.position = lastValidPosition;

                    // �ڵ��� ���¸� �ʱ�ȭ�Ͽ� �ٸ� ��ü�� ���� �� �ְ� ��
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

        // ���� [������] �̹� ���𰡸� ��� �ִٸ� ���� ���� ���� ����
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
        // ���� [������] ���� ��ü�� ���� ��� �ִ� ��ü�� �ٸ� �� �����Ƿ�, ��Ȯ�� ��ġ�ϴ��� Ȯ�� ����
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