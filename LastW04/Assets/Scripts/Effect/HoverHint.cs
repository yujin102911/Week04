using UnityEngine;

public class HoverHint : MonoBehaviour
{
    public enum ShowCondition { OnlyEditing, OnlyNonEditing, Always }
    public enum HorizontalSide { Auto, Left, Right }

    [Header("When to Show")]
    [SerializeField] private ShowCondition showWhen = ShowCondition.OnlyEditing;

    [Header("Hint Visual")]
    [Tooltip("마우스 오버 시 표시할 설명 스프라이트")]
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
    [Tooltip("화면 가장자리 여백(뷰포트 비율 0~0.2 정도 권장)")]
    [SerializeField, Range(0f, 0.2f)] private float viewportPadding = 0.03f;

    private GameObject hintGO;
    private SpriteRenderer hintSR;
    private Camera cam;          // Main Camera 캐시
    private bool visible;        // 현재 표시 중인지

    private void Awake()
    {
        cam = Camera.main;
        if (!cam) Debug.LogWarning("[HoverHintET2DClamp] Main Camera가 없습니다.");

        if (!hintSprite) return;

        hintGO = new GameObject("HoverHint");
        hintGO.transform.SetParent(transform, worldPositionStays: true);
        hintSR = hintGO.AddComponent<SpriteRenderer>();
        hintSR.sprite = hintSprite;
        hintSR.enabled = false; // 시작은 숨김
        hintSR.sortingOrder = sortingOrder;
        if (!string.IsNullOrEmpty(sortingLayerName))
            hintSR.sortingLayerName = sortingLayerName;

        // 초기 위치(오른쪽 기준)
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

    // Event Trigger → Pointer Enter
    public void OnPointerEnterFromET()
    {
        if (!hintSR || !cam) return;
        if (!ModeAllows()) return;

        // 우/좌 배치 결정
        Vector2 chosenOffset = PickOffsetBySide();

        // 우선 기준 위치로 놓고
        Vector3 target = transform.position + (Vector3)chosenOffset;

        // 화면 밖이면 클램프(뷰포트 패딩과 스프라이트 실제 크기 고려)
        target = ClampToCameraView(target);

        hintGO.transform.position = target;
        hintSR.enabled = true;
        visible = true;
    }

    // Event Trigger → Pointer Exit
    public void OnPointerExitFromET()
    {
        if (!hintSR) return;
        hintSR.enabled = false;
        visible = false;
    }

    private void LateUpdate()
    {
        if (!visible || !hintSR || !cam) return;

        // 마우스가 여전히 위에 있는 동안, 대상이 움직여도 계속 따라가서 보정되도록 갱신
        Vector2 chosenOffset = PickOffsetBySide();
        Vector3 target = transform.position + (Vector3)chosenOffset;
        target = ClampToCameraView(target);
        hintGO.transform.position = target;
    }

    private Vector2 PickOffsetBySide()
    {
        if (side == HorizontalSide.Left) return leftOffset;
        if (side == HorizontalSide.Right) return rightOffset;

        // Auto: 오브젝트가 화면 왼쪽에 있으면 오른쪽에 띄우고, 오른쪽에 있으면 왼쪽에 띄움
        if (!cam) return rightOffset;
        var vp = cam.WorldToViewportPoint(transform.position);
        return (vp.x < 0.5f) ? rightOffset : leftOffset;
    }

    private Vector3 ClampToCameraView(Vector3 worldPos)
    {
        if (!cam || !hintSR || hintSR.sprite == null) return worldPos;

        // 타겟의 뷰포트 좌표
        Vector3 vp = cam.WorldToViewportPoint(worldPos);

        // 스프라이트 실제 크기(월드) → 뷰포트 반쪽 크기
        Vector2 halfVp = GetHintHalfViewportSize(worldPos);

        // 패딩과 스프라이트 반쪽 크기를 고려해 클램프
        float pad = viewportPadding;
        vp.x = Mathf.Clamp(vp.x, pad + halfVp.x, 1f - pad - halfVp.x);
        vp.y = Mathf.Clamp(vp.y, pad + halfVp.y, 1f - pad - halfVp.y);

        // 다시 월드로 변환 (z는 원래 깊이 유지)
        Vector3 clamped = cam.ViewportToWorldPoint(vp);
        clamped.z = worldPos.z;
        return clamped;
    }

    private Vector2 GetHintHalfViewportSize(Vector3 aroundWorldPos)
    {
        // 스프라이트 로컬 크기(유닛) * 스케일 = 월드 크기
        Vector2 worldSize = Vector2.Scale(
            hintSR.sprite.bounds.size,
            (Vector2)hintSR.transform.lossyScale
        );
        Vector2 halfWorld = 0.5f * worldSize;

        // 월드 반쪽 크기를 뷰포트 단위로 변환
        // 기준점 aroundWorldPos에서 +x, +y 만큼 이동시킨 뒤 차이로 뷰포트 폭/높이 계산
        Vector3 centerVP = cam.WorldToViewportPoint(aroundWorldPos);
        float halfVx = Mathf.Abs(cam.WorldToViewportPoint(aroundWorldPos + new Vector3(halfWorld.x, 0f, 0f)).x - centerVP.x);
        float halfVy = Mathf.Abs(cam.WorldToViewportPoint(aroundWorldPos + new Vector3(0f, halfWorld.y, 0f)).y - centerVP.y);
        return new Vector2(halfVx, halfVy);
    }
}
