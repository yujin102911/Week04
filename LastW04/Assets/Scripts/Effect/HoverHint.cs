using UnityEngine;
using TMPro;

public class HoverHint : MonoBehaviour
{
    public enum ShowCondition { OnlyEditing, OnlyNonEditing, Always }
    public enum HorizontalSide { Auto, Left, Right }

    [Header("When to Show")]
    [SerializeField] private ShowCondition showWhen = ShowCondition.OnlyEditing;
    private bool isPointerInside; // �����Ͱ� ���� ������Ʈ ���� �ִ���

    [Header("Hint Visual")]
    [Tooltip("���콺 ���� �� ǥ���� ���� ��������Ʈ(���). 9-Slice border ���� ����")]
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
    [Tooltip("ȭ�� �����ڸ� ����(����Ʈ ���� 0~0.2 ����)")]
    [SerializeField, Range(0f, 0.2f)] private float viewportPadding = 0.03f;

    [Header("Label (TextMeshPro, ��������Ʈ �� ����)")]
    [SerializeField] private bool showLabel = true;
    [SerializeField, TextArea] private string labelText = "���� �ؽ�Ʈ";
    [SerializeField] private TMP_FontAsset labelFont;
    [SerializeField, Min(0.1f)] private float labelFontSize = 4f;
    [SerializeField] private Color labelColor = Color.white;
    [Tooltip("��������Ʈ �������� �� ���� ������")]
    [SerializeField] private Vector3 labelLocalOffset = Vector3.zero;

    [Header("Auto Resize (����� �ؽ�Ʈ�� ����)")]
    [Tooltip("hintSprite�� 9-Slice(border)�� �� �ڵ� ��������")]
    [SerializeField] private bool autoResizeBackground = true;
    [Tooltip("�ؽ�Ʈ �ѷ� ����(�¿�/����, ���� ����)")]
    [SerializeField] private Vector2 backgroundPadding = new Vector2(0.2f, 0.2f);
    [Tooltip("��� �ּ� ũ��(���� ����)")]
    [SerializeField] private Vector2 backgroundMinSize = new Vector2(0.6f, 0.4f);
    [Tooltip("��� �ִ� ũ��(0�̸� ���� ����)")]
    [SerializeField] private Vector2 backgroundMaxSize = Vector2.zero;

    private GameObject hintGO;
    private SpriteRenderer hintSR;
    private TextMeshPro labelTMP;
    private Renderer labelRenderer;

    private Camera cam;
    private bool visible;

    private void Awake()
    {
        cam = Camera.main;
        if (!cam) Debug.LogWarning("[HoverHint] Main Camera�� �����ϴ�.");

        // ��Ʈ ��Ʈ
        hintGO = new GameObject("HoverHint");
        hintGO.transform.SetParent(transform, worldPositionStays: true);
        hintGO.transform.position = transform.position + (Vector3)rightOffset;

        // ��� ��������Ʈ
        if (hintSprite)
        {
            hintSR = hintGO.AddComponent<SpriteRenderer>();
            hintSR.sprite = hintSprite;
            hintSR.enabled = false;
            hintSR.sortingOrder = sortingOrder;
            if (!string.IsNullOrEmpty(sortingLayerName))
                hintSR.sortingLayerName = sortingLayerName;

            // 9-Slice�� Sliced ����
            if (HasSpriteBorder(hintSprite))
                hintSR.drawMode = SpriteDrawMode.Sliced;
        }

        // ��
        if (showLabel)
        {
            var labelGO = new GameObject("Label");
            labelGO.transform.SetParent(hintGO.transform, worldPositionStays: false);
            labelGO.transform.localPosition = labelLocalOffset;

            labelTMP = labelGO.AddComponent<TextMeshPro>();
            labelTMP.text = labelText;
            labelTMP.font = labelFont;     // null�̸� �⺻ ��Ʈ
            labelTMP.fontSize = labelFontSize;
            labelTMP.color = labelColor;
            labelTMP.alignment = TextAlignmentOptions.Center;
            labelTMP.enableWordWrapping = false;
            labelTMP.overflowMode = TextOverflowModes.Overflow;

            labelRenderer = labelGO.GetComponent<Renderer>();
            if (!string.IsNullOrEmpty(sortingLayerName))
                labelRenderer.sortingLayerName = sortingLayerName;
            labelRenderer.sortingOrder = sortingOrder + 1;
            labelRenderer.enabled = false;
        }

        // �ʱ� ��� ũ�� ����
        if (autoResizeBackground) RefreshBackgroundSize();
    }

    private void OnValidate()
    {
        // �����Ϳ��� �� �ٲ� ���� ��� �ݿ�
        if (hintSR && autoResizeBackground) RefreshBackgroundSize();
        if (labelTMP) ApplyLabelStyle();
    }

    private void ApplyLabelStyle()
    {
        labelTMP.font = labelFont;
        labelTMP.fontSize = labelFontSize;
        labelTMP.color = labelColor;
        labelTMP.alignment = TextAlignmentOptions.Center;
        labelTMP.enableWordWrapping = false;
        labelTMP.overflowMode = TextOverflowModes.Overflow;
        labelTMP.transform.localPosition = labelLocalOffset;
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
        isPointerInside = true;
        if (!cam) return;

        // ��� üũ
        if (!ModeAllows()) return;

        // �ؽ�Ʈ�� �ٲ���� ���� ������ ���� �� �ѹ� ����
        // (autoResizeBackground �ɼ��� ���� ��쿡��)
        if (autoResizeBackground) RefreshBackgroundSize();

        Vector2 chosenOffset = PickOffsetBySide();
        Vector3 target = transform.position + (Vector3)chosenOffset;
        target = ClampToCameraView(target);

        hintGO.transform.position = target;

        SetVisible(true);
        visible = true;
    }

    // Event Trigger �� Pointer Exit
    public void OnPointerExitFromET()
    {
        isPointerInside = false;
        SetVisible(false);
        visible = false;
    }

    private void LateUpdate()
    {
        if (!cam) return;

        // ��� ��ȯ ��� �ݿ�
        bool shouldBeVisible = isPointerInside && ModeAllows();
        if (shouldBeVisible != visible)
        {
            if (shouldBeVisible)
            {
                // �ʿ� �� ��� ��������
                // if (autoResizeBackground) RefreshBackgroundSize();

                Vector2 chosenOffset = PickOffsetBySide();
                Vector3 target = transform.position + (Vector3)chosenOffset;
                target = ClampToCameraView(target);
                hintGO.transform.position = target;
            }

            SetVisible(shouldBeVisible);
            visible = shouldBeVisible;
        }

        // �̵�/Ŭ���� ���� (���� ����)
        if (visible)
        {
            Vector2 chosenOffset = PickOffsetBySide();
            Vector3 target = transform.position + (Vector3)chosenOffset;
            target = ClampToCameraView(target);
            hintGO.transform.position = target;
        }
    }

    private void SetVisible(bool on)
    {
        if (hintSR) hintSR.enabled = on;
        if (labelRenderer) labelRenderer.enabled = on;
    }

    private Vector2 PickOffsetBySide()
    {
        if (side == HorizontalSide.Left) return leftOffset;
        if (side == HorizontalSide.Right) return rightOffset;

        if (!cam) return rightOffset;
        var vp = cam.WorldToViewportPoint(transform.position);
        return (vp.x < 0.5f) ? rightOffset : leftOffset;
    }

    private Vector3 ClampToCameraView(Vector3 worldPos)
    {
        if (!cam) return worldPos;

        Vector3 vp = cam.WorldToViewportPoint(worldPos);
        Vector2 halfVp = GetHintHalfViewportSize(worldPos);

        float pad = viewportPadding;
        vp.x = Mathf.Clamp(vp.x, pad + halfVp.x, 1f - pad - halfVp.x);
        vp.y = Mathf.Clamp(vp.y, pad + halfVp.y, 1f - pad - halfVp.y);

        Vector3 clamped = cam.ViewportToWorldPoint(vp);
        clamped.z = worldPos.z;
        return clamped;
    }

    private Vector2 GetHintHalfViewportSize(Vector3 aroundWorldPos)
    {
        if (hintSR == null || hintSR.sprite == null) return Vector2.zero;

        // SpriteRenderer.size�� ��ȿ�ϸ� �װ� ���(= ���� ���� ������)
        Vector2 worldSize;
        if (hintSR.drawMode != SpriteDrawMode.Simple)
        {
            worldSize = hintSR.size;
            worldSize = Vector2.Scale(worldSize, (Vector2)hintSR.transform.lossyScale);
        }
        else
        {
            worldSize = Vector2.Scale(hintSR.sprite.bounds.size, (Vector2)hintSR.transform.lossyScale);
        }

        Vector2 halfWorld = 0.5f * worldSize;

        Vector3 centerVP = cam.WorldToViewportPoint(aroundWorldPos);
        float halfVx = Mathf.Abs(cam.WorldToViewportPoint(aroundWorldPos + new Vector3(halfWorld.x, 0f, 0f)).x - centerVP.x);
        float halfVy = Mathf.Abs(cam.WorldToViewportPoint(aroundWorldPos + new Vector3(0f, halfWorld.y, 0f)).y - centerVP.y);
        return new Vector2(halfVx, halfVy);
    }

    // --- �ٽ�: �� �ؽ�Ʈ ũ�⿡ ���� 9-Slice ��� �ڵ� �������� ---
    private void RefreshBackgroundSize()
    {
        if (!hintSR || hintSR.sprite == null || !HasSpriteBorder(hintSR.sprite)) return;
        if (!labelTMP) { hintSR.drawMode = SpriteDrawMode.Sliced; hintSR.size = Vector2.Max(backgroundMinSize, Vector2.zero); return; }

        // ���� ���� ���� ���� ũ�� ���
        labelTMP.ForceMeshUpdate();
        var bounds = labelTMP.textBounds;                // ���� ��ǥ ���� Bounds
        Vector2 labelWorldSize = bounds.size;

        // ���� ���� + �ּ�/�ִ� ����
        Vector2 targetWorldSize = labelWorldSize + backgroundPadding * 2f;
        targetWorldSize = new Vector2(
            Mathf.Max(targetWorldSize.x, backgroundMinSize.x),
            Mathf.Max(targetWorldSize.y, backgroundMinSize.y)
        );
        if (backgroundMaxSize != Vector2.zero)
        {
            targetWorldSize = new Vector2(
                Mathf.Min(targetWorldSize.x, backgroundMaxSize.x),
                Mathf.Min(targetWorldSize.y, backgroundMaxSize.y)
            );
        }

        // Sliced ���� ������ ����(���� �����̹Ƿ� �θ� ������ ���)
        hintSR.drawMode = SpriteDrawMode.Sliced;
        Vector3 lossy = hintSR.transform.lossyScale;
        Vector2 localSize = new Vector2(
            targetWorldSize.x / Mathf.Max(lossy.x, 1e-6f),
            targetWorldSize.y / Mathf.Max(lossy.y, 1e-6f)
        );
        hintSR.size = localSize;

        // ���� ��� �߾ӿ� ��ġ(�߾� ����)
        if (labelTMP)
        {
            labelTMP.alignment = TextAlignmentOptions.Center;
            labelTMP.transform.localPosition = labelLocalOffset;
        }
    }

    private static bool HasSpriteBorder(Sprite s)
    {
        // 9-Slice ��� ���� ����(�׵θ� �� �ȼ��̶� ������ true)
        var b = s.border;
        return b.x > 0 || b.y > 0 || b.z > 0 || b.w > 0;
    }

    // �� �ؽ�Ʈ�� ��Ÿ������ �ٲ� ���� ������ �ڵ� ����
    public void SetLabel(string text)
    {
        if (labelTMP == null) return;
        labelTMP.text = text;
        if (autoResizeBackground) RefreshBackgroundSize();
    }
}
