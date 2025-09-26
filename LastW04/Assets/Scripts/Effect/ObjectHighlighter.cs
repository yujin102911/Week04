using UnityEngine;

[DisallowMultipleComponent]
public class ObjectHighlighter : MonoBehaviour
{
    [Header("Trigger Detection")]
    [SerializeField] private float radius = 1.6f;         // ���� �ݰ�(Ʈ���� �ݶ��̴� ũ��)
    [SerializeField] private string playerTag = "Player"; // �÷��̾� �±�

    [Header("Outline Visual")]
    [SerializeField] private Color outlineColor = new Color(1f, 0.92f, 0.16f, 1f); // ��� ���̶���Ʈ
    [SerializeField, Range(1f, 1.3f)] private float outlineScale = 1.06f;          // �ܰ��� �β�(������)
    [SerializeField] private int orderOffset = 2;          // �������� �տ� ���̵��� ���ļ��� ������
    [SerializeField] private bool pulse = false;           // ������(�޽�) ȿ��
    [SerializeField] private float pulseAmount = 0.02f;
    [SerializeField] private float pulseSpeed = 4f;

    private SpriteRenderer mainSR;     // ���� ��������Ʈ
    private SpriteRenderer outlineSR;  // �ܰ����� ��������Ʈ(�ڽ�)
    private CircleCollider2D trigger;  // ������ Ʈ����
    private bool visible = false;
    private Vector3 baseOutlineLocalScale;

    void Awake()
    {
        // ���� ��������Ʈ ã��(�ڽ� ����)
        mainSR = GetComponent<SpriteRenderer>();
        if (!mainSR) mainSR = GetComponentInChildren<SpriteRenderer>(includeInactive: true);

        SetupOutline();
        SetupTrigger();
    }

    void Start()
    {
        Show(false, force: true); // ���� �� ��Ȱ��
    }

    void LateUpdate()
    {
        // �ִϸ��̼�/�ø�/���� ����ȭ
        if (outlineSR && mainSR)
        {
            if (outlineSR.sprite != mainSR.sprite) outlineSR.sprite = mainSR.sprite;
            if (outlineSR.flipX != mainSR.flipX) outlineSR.flipX = mainSR.flipX;
            if (outlineSR.flipY != mainSR.flipY) outlineSR.flipY = mainSR.flipY;

            if (outlineSR.sortingLayerID != mainSR.sortingLayerID)
                outlineSR.sortingLayerID = mainSR.sortingLayerID;

            int desiredOrder = mainSR.sortingOrder + orderOffset;
            if (outlineSR.sortingOrder != desiredOrder)
                outlineSR.sortingOrder = desiredOrder;
        }

        // �޽�(������) ȿ��
        if (pulse && outlineSR && visible)
        {
            float s = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            outlineSR.transform.localScale = baseOutlineLocalScale * s;
        }
    }

    // Ʈ���� ����: ���̶���Ʈ ON
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
            Show(true);
    }

    // Ʈ���� ��Ż: ���̶���Ʈ OFF
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
            Show(false);
    }

    // �ܺο��� ������ �Ѱ�/���� ���� �� ȣ�� ����
    public void SetHighlight(bool on) => Show(on);

    private void Show(bool on, bool force = false)
    {
        if (!outlineSR) return;
        if (!force && visible == on) return;
        visible = on;
        outlineSR.enabled = on;
    }

    private void SetupOutline()
    {
        // �ڽ� ������Ʈ "_Outline" ����/�غ�
        var holder = transform.Find("_Outline");
        if (!holder)
        {
            var go = new GameObject("_Outline");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = Vector3.zero;
            holder = go.transform;
        }

        outlineSR = holder.GetComponent<SpriteRenderer>();
        if (!outlineSR) outlineSR = holder.gameObject.AddComponent<SpriteRenderer>();

        if (mainSR)
        {
            outlineSR.sprite = mainSR.sprite;
            outlineSR.sortingLayerID = mainSR.sortingLayerID;
            outlineSR.sortingOrder = mainSR.sortingOrder + orderOffset;
            outlineSR.flipX = mainSR.flipX;
            outlineSR.flipY = mainSR.flipY;
        }

        outlineSR.color = outlineColor;
        holder.localScale = Vector3.one * outlineScale;
        baseOutlineLocalScale = holder.localScale;

        outlineSR.enabled = false; // �⺻ ���α�
    }

    private void SetupTrigger()
    {
        // ���� Ʈ���� �غ�
        trigger = GetComponent<CircleCollider2D>();
        if (!trigger) trigger = gameObject.AddComponent<CircleCollider2D>();
        trigger.isTrigger = true;
        trigger.radius = radius;

        // Ʈ���� ������ ���� ���ʿ��� Rigidbody2D �ʿ� �� ��� �淮 ������ٵ� ����
        var rb = GetComponent<Rigidbody2D>();
        if (!rb)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.isKinematic = true; // ���� ���� ����
            rb.simulated = true;
            rb.gravityScale = 0f;
        }
    }

    void OnValidate()
    {
        // �ν����� �� ���� �� ��� �ݿ�
        if (!outlineSR) return;

        outlineSR.color = outlineColor;
        outlineSR.sortingOrder = (mainSR ? mainSR.sortingOrder + orderOffset : orderOffset);
        outlineSR.transform.localScale = Vector3.one * outlineScale;
        baseOutlineLocalScale = outlineSR.transform.localScale;

        if (!trigger) trigger = GetComponent<CircleCollider2D>();
        if (trigger)
        {
            trigger.isTrigger = true;
            trigger.radius = radius;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.9f, 0.1f, 0.6f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
