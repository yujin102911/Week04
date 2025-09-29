using UnityEngine;

public class HoverHint : MonoBehaviour
{
    public enum ShowCondition { OnlyEditing, OnlyNonEditing, Always }

    [Header("When to Show")]
    [SerializeField] private ShowCondition showWhen = ShowCondition.OnlyEditing;

    [Header("Hint Visual")]
    [Tooltip("마우스 오버 시 표시할 설명 스프라이트")]
    [SerializeField] private Sprite hintSprite;
    [SerializeField] private Vector2 worldOffset = new Vector2(0f, 0.9f);
    [SerializeField] private string sortingLayerName = "";
    [SerializeField] private int sortingOrder = 100;

    private GameObject hintGO;
    private SpriteRenderer hintSR;

    private void Awake()
    {
        if (!hintSprite) return;

        hintGO = new GameObject("HoverHint");
        hintGO.transform.SetParent(transform, worldPositionStays: true);
        hintGO.transform.position = transform.position + (Vector3)worldOffset;

        hintSR = hintGO.AddComponent<SpriteRenderer>();
        hintSR.sprite = hintSprite;
        hintSR.enabled = false; // 시작 시 숨김
        hintSR.sortingOrder = sortingOrder;

        if (!string.IsNullOrEmpty(sortingLayerName))
            hintSR.sortingLayerName = sortingLayerName;
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

    // Event Trigger → Pointer Enter 에 연결
    public void OnPointerEnterFromET()
    {
        if (!hintSR) return;
        if (!ModeAllows()) return;

        hintGO.transform.position = transform.position + (Vector3)worldOffset;
        Debug.Log("힌트 나옴");
        hintSR.enabled = true; // 즉시 켜기
    }

    // Event Trigger → Pointer Exit 에 연결
    public void OnPointerExitFromET()
    {
        if (!hintSR) return;
        hintSR.enabled = false; // 즉시 끄기
    }
}
