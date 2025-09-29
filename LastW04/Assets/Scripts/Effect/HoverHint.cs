using UnityEngine;
using TMPro;

public class HoverHint : MonoBehaviour
{
    public enum ShowCondition { OnlyEditing, OnlyNonEditing, Always }
    public enum HorizontalSide { Auto, Left, Right }

    [Header("When to Show")]
    [SerializeField] private ShowCondition showWhen = ShowCondition.OnlyEditing;
    private bool isPointerInside; // 포인터가 현재 오브젝트 위에 있는지

    [Header("Hint Visual")]
    [Tooltip("마우스 오버 시 표시할 설명 스프라이트(배경). 9-Slice border 설정 권장")]
    [SerializeField] private Sprite hintSprite;
    [Tooltip("우측에 붙일 때의 기준 오프셋(월드 좌표)")]
    [SerializeField] private Vector2 rightOffset = new Vector2(1.0f, 0.9f);
    [Tooltip("좌측에 붙일 때의 기준 오프셋(월드 좌표)")]
    [SerializeField] private Vector2 leftOffset = new Vector2(-1.0f, 0.9f);
    [Tooltip("좌/우 배치 방식")]
    [SerializeField] private HorizontalSide side = HorizontalSide.Auto;

    [Header("Sorting")]
    [SerializeField] private string sortingLayerName = "";
    [SerializeField] private int sortingOrder = 100;

    [Header("Clamp-to-Camera")]
    [Tooltip("화면 가장자리 여백(뷰포트 비율 0~0.2 권장)")]
    [SerializeField, Range(0f, 0.2f)] private float viewportPadding = 0.03f;

    [Header("Label (TextMeshPro, 스프라이트 위 고정)")]
    [SerializeField] private bool showLabel = true;
    [SerializeField, TextArea] private string labelText = "설명 텍스트";
    [SerializeField] private TMP_FontAsset labelFont;
    [SerializeField, Min(0.1f)] private float labelFontSize = 4f;
    [SerializeField] private Color labelColor = Color.white;
    [Tooltip("스프라이트 위에서의 라벨 로컬 오프셋")]
    [SerializeField] private Vector3 labelLocalOffset = Vector3.zero;

    [Header("Auto Resize (배경을 텍스트에 맞춤)")]
    [Tooltip("hintSprite가 9-Slice(border)일 때 자동 리사이즈")]
    [SerializeField] private bool autoResizeBackground = true;
    [Tooltip("텍스트 둘레 여백(좌우/상하, 월드 단위)")]
    [SerializeField] private Vector2 backgroundPadding = new Vector2(0.2f, 0.2f);
    [Tooltip("배경 최소 크기(월드 단위)")]
    [SerializeField] private Vector2 backgroundMinSize = new Vector2(0.6f, 0.4f);
    [Tooltip("배경 최대 크기(0이면 제한 없음)")]
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
        if (!cam) Debug.LogWarning("[HoverHint] Main Camera가 없습니다.");

        // 힌트 루트
        hintGO = new GameObject("HoverHint");
        hintGO.transform.SetParent(transform, worldPositionStays: true);
        hintGO.transform.position = transform.position + (Vector3)rightOffset;

        // 배경 스프라이트
        if (hintSprite)
        {
            hintSR = hintGO.AddComponent<SpriteRenderer>();
            hintSR.sprite = hintSprite;
            hintSR.enabled = false;
            hintSR.sortingOrder = sortingOrder;
            if (!string.IsNullOrEmpty(sortingLayerName))
                hintSR.sortingLayerName = sortingLayerName;

            // 9-Slice면 Sliced 모드로
            if (HasSpriteBorder(hintSprite))
                hintSR.drawMode = SpriteDrawMode.Sliced;
        }

        // 라벨
        if (showLabel)
        {
            var labelGO = new GameObject("Label");
            labelGO.transform.SetParent(hintGO.transform, worldPositionStays: false);
            labelGO.transform.localPosition = labelLocalOffset;

            labelTMP = labelGO.AddComponent<TextMeshPro>();
            labelTMP.text = labelText;
            labelTMP.font = labelFont;     // null이면 기본 폰트
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

        // 초기 배경 크기 맞춤
        if (autoResizeBackground) RefreshBackgroundSize();
    }

    private void OnValidate()
    {
        // 에디터에서 값 바꿀 때도 즉시 반영
        if (hintSR && autoResizeBackground) RefreshBackgroundSize();
        if (labelTMP) ApplyLabelStyle();
    }

    private void ApplyLabelStyle()
    {
        labelTMP.font = labelFont;
        labelTMP.fontSize = labelFontSize;
        labelTMP.color = labelColor;
        labelTMP.alignment = TextAlignmentOptions.Center;
        //labelTMP.enableWordWrapping = false;<<여기 안쓰인다고 계속 에러떠서 주석처리
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

    // Event Trigger → Pointer Enter
    public void OnPointerEnterFromET()
    {
        isPointerInside = true;
        if (!cam) return;

        // 모드 체크
        if (!ModeAllows()) return;

        // 텍스트가 바뀌었을 수도 있으니 진입 시 한번 맞춤
        // (autoResizeBackground 옵션을 쓰는 경우에만)
        if (autoResizeBackground) RefreshBackgroundSize();

        Vector2 chosenOffset = PickOffsetBySide();
        Vector3 target = transform.position + (Vector3)chosenOffset;
        target = ClampToCameraView(target);

        hintGO.transform.position = target;

        SetVisible(true);
        visible = true;
    }

    // Event Trigger → Pointer Exit
    public void OnPointerExitFromET()
    {
        isPointerInside = false;
        SetVisible(false);
        visible = false;
    }

    private void LateUpdate()
    {
        if (!cam) return;

        // 모드 전환 즉시 반영
        bool shouldBeVisible = isPointerInside && ModeAllows();
        if (shouldBeVisible != visible)
        {
            if (shouldBeVisible)
            {
                // 필요 시 배경 리사이즈
                // if (autoResizeBackground) RefreshBackgroundSize();

                Vector2 chosenOffset = PickOffsetBySide();
                Vector3 target = transform.position + (Vector3)chosenOffset;
                target = ClampToCameraView(target);
                hintGO.transform.position = target;
            }

            SetVisible(shouldBeVisible);
            visible = shouldBeVisible;
        }

        // 이동/클램프 갱신 (보일 때만)
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

        // SpriteRenderer.size가 유효하면 그걸 사용(= 실제 렌더 사이즈)
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

    // --- 핵심: 라벨 텍스트 크기에 맞춰 9-Slice 배경 자동 리사이즈 ---
    private void RefreshBackgroundSize()
    {
        if (!hintSR || hintSR.sprite == null || !HasSpriteBorder(hintSR.sprite)) return;
        if (!labelTMP) { hintSR.drawMode = SpriteDrawMode.Sliced; hintSR.size = Vector2.Max(backgroundMinSize, Vector2.zero); return; }

        // 현재 라벨의 실제 렌더 크기 계산
        labelTMP.ForceMeshUpdate();
        var bounds = labelTMP.textBounds;                // 월드 좌표 기준 Bounds
        Vector2 labelWorldSize = bounds.size;

        // 여백 적용 + 최소/최대 보정
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

        // Sliced 모드로 사이즈 지정(로컬 기준이므로 부모 스케일 고려)
        hintSR.drawMode = SpriteDrawMode.Sliced;
        Vector3 lossy = hintSR.transform.lossyScale;
        Vector2 localSize = new Vector2(
            targetWorldSize.x / Mathf.Max(lossy.x, 1e-6f),
            targetWorldSize.y / Mathf.Max(lossy.y, 1e-6f)
        );
        hintSR.size = localSize;

        // 라벨을 배경 중앙에 위치(중앙 정렬)
        if (labelTMP)
        {
            labelTMP.alignment = TextAlignmentOptions.Center;
            labelTMP.transform.localPosition = labelLocalOffset;
        }
    }

    private static bool HasSpriteBorder(Sprite s)
    {
        // 9-Slice 사용 가능 여부(테두리 한 픽셀이라도 있으면 true)
        var b = s.border;
        return b.x > 0 || b.y > 0 || b.z > 0 || b.w > 0;
    }

    // 라벨 텍스트를 런타임으로 바꿀 때도 사이즈 자동 갱신
    public void SetLabel(string text)
    {
        if (labelTMP == null) return;
        labelTMP.text = text;
        if (autoResizeBackground) RefreshBackgroundSize();
    }
}
