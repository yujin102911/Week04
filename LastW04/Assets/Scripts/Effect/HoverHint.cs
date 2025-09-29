using UnityEngine;

public class HoverHint : MonoBehaviour
{
    public enum ShowCondition { OnlyEditing, OnlyNonEditing, Always }
    public enum HorizontalSide { Auto, Left, Right }

    [Header("When to Show")]
    [SerializeField] private ShowCondition showWhen = ShowCondition.OnlyEditing;

    [Header("Hint Visual")]
    [Tooltip("���콺 ���� �� ǥ���� ���� ��������Ʈ")]
    [SerializeField] private Sprite hintSprite;
    [Tooltip("������ ���� ���� ���� ������(���� ��ǥ)")]
    [SerializeField] private Vector2 rightOffset = new Vector2(1.0f, 0.9f);
    [Tooltip("������ ���� ���� ���� ������(���� ��ǥ)")]
    [SerializeField] private Vector2 leftOffset = new Vector2(-1.0f, 0.9f);
    [Tooltip("��/�� ��ġ ���")]
    [SerializeField] private HorizontalSide side = HorizontalSide.Auto;

    [Header("Sorting")]
    [SerializeField] private string sortingLayerName = "";
    [SerializeField] private int sortingOrder = 100;

    [Header("Clamp-to-Camera")]
    [Tooltip("ȭ�� �����ڸ� ����(����Ʈ ���� 0~0.2 ���� ����)")]
    [SerializeField, Range(0f, 0.2f)] private float viewportPadding = 0.03f;

    private GameObject hintGO;
    private SpriteRenderer hintSR;
    private Camera cam;          // Main Camera ĳ��
    private bool visible;        // ���� ǥ�� ������

    private void Awake()
    {
        cam = Camera.main;
        if (!cam) Debug.LogWarning("[HoverHintET2DClamp] Main Camera�� �����ϴ�.");

        if (!hintSprite) return;

        hintGO = new GameObject("HoverHint");
        hintGO.transform.SetParent(transform, worldPositionStays: true);
        hintSR = hintGO.AddComponent<SpriteRenderer>();
        hintSR.sprite = hintSprite;
        hintSR.enabled = false; // ������ ����
        hintSR.sortingOrder = sortingOrder;
        if (!string.IsNullOrEmpty(sortingLayerName))
            hintSR.sortingLayerName = sortingLayerName;

        // �ʱ� ��ġ(������ ����)
        hintGO.transform.position = transform.position + (Vector3)rightOffset;
    }

    private bool ModeAllows()
    {
        switch (showWhen)
        {
            case ShowCondition.OnlyEditing: return GameManager.mode == Mode.Editing;
            case ShowCondition.OnlyNonEditing: return GameManager.mode != Mode.Editing;
            default: return true;
        }
    }

    // Event Trigger �� Pointer Enter
    public void OnPointerEnterFromET()
    {
        if (!hintSR || !cam) return;
        if (!ModeAllows()) return;

        // ��/�� ��ġ ����
        Vector2 chosenOffset = PickOffsetBySide();

        // �켱 ���� ��ġ�� ����
        Vector3 target = transform.position + (Vector3)chosenOffset;

        // ȭ�� ���̸� Ŭ����(����Ʈ �е��� ��������Ʈ ���� ũ�� ���)
        target = ClampToCameraView(target);

        hintGO.transform.position = target;
        hintSR.enabled = true;
        visible = true;
    }

    // Event Trigger �� Pointer Exit
    public void OnPointerExitFromET()
    {
        if (!hintSR) return;
        hintSR.enabled = false;
        visible = false;
    }

    private void LateUpdate()
    {
        if (!visible || !hintSR || !cam) return;

        // ���콺�� ������ ���� �ִ� ����, ����� �������� ��� ���󰡼� �����ǵ��� ����
        Vector2 chosenOffset = PickOffsetBySide();
        Vector3 target = transform.position + (Vector3)chosenOffset;
        target = ClampToCameraView(target);
        hintGO.transform.position = target;
    }

    private Vector2 PickOffsetBySide()
    {
        if (side == HorizontalSide.Left) return leftOffset;
        if (side == HorizontalSide.Right) return rightOffset;

        // Auto: ������Ʈ�� ȭ�� ���ʿ� ������ �����ʿ� ����, �����ʿ� ������ ���ʿ� ���
        if (!cam) return rightOffset;
        var vp = cam.WorldToViewportPoint(transform.position);
        return (vp.x < 0.5f) ? rightOffset : leftOffset;
    }

    private Vector3 ClampToCameraView(Vector3 worldPos)
    {
        if (!cam || !hintSR || hintSR.sprite == null) return worldPos;

        // Ÿ���� ����Ʈ ��ǥ
        Vector3 vp = cam.WorldToViewportPoint(worldPos);

        // ��������Ʈ ���� ũ��(����) �� ����Ʈ ���� ũ��
        Vector2 halfVp = GetHintHalfViewportSize(worldPos);

        // �е��� ��������Ʈ ���� ũ�⸦ ����� Ŭ����
        float pad = viewportPadding;
        vp.x = Mathf.Clamp(vp.x, pad + halfVp.x, 1f - pad - halfVp.x);
        vp.y = Mathf.Clamp(vp.y, pad + halfVp.y, 1f - pad - halfVp.y);

        // �ٽ� ����� ��ȯ (z�� ���� ���� ����)
        Vector3 clamped = cam.ViewportToWorldPoint(vp);
        clamped.z = worldPos.z;
        return clamped;
    }

    private Vector2 GetHintHalfViewportSize(Vector3 aroundWorldPos)
    {
        // ��������Ʈ ���� ũ��(����) * ������ = ���� ũ��
        Vector2 worldSize = Vector2.Scale(
            hintSR.sprite.bounds.size,
            (Vector2)hintSR.transform.lossyScale
        );
        Vector2 halfWorld = 0.5f * worldSize;

        // ���� ���� ũ�⸦ ����Ʈ ������ ��ȯ
        // ������ aroundWorldPos���� +x, +y ��ŭ �̵���Ų �� ���̷� ����Ʈ ��/���� ���
        Vector3 centerVP = cam.WorldToViewportPoint(aroundWorldPos);
        float halfVx = Mathf.Abs(cam.WorldToViewportPoint(aroundWorldPos + new Vector3(halfWorld.x, 0f, 0f)).x - centerVP.x);
        float halfVy = Mathf.Abs(cam.WorldToViewportPoint(aroundWorldPos + new Vector3(0f, halfWorld.y, 0f)).y - centerVP.y);
        return new Vector2(halfVx, halfVy);
    }
}
