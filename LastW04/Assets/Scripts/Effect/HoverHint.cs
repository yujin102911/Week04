using UnityEngine;

public class HoverHint : MonoBehaviour
{
    public enum ShowCondition { OnlyEditing, OnlyNonEditing, Always }

    [Header("When to Show")]
    [SerializeField] private ShowCondition showWhen = ShowCondition.OnlyEditing;

    [Header("Hint Visual")]
    [Tooltip("���콺 ���� �� ǥ���� ���� ��������Ʈ")]
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
        hintSR.enabled = false; // ���� �� ����
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

    // Event Trigger �� Pointer Enter �� ����
    public void OnPointerEnterFromET()
    {
        if (!hintSR) return;
        if (!ModeAllows()) return;

        hintGO.transform.position = transform.position + (Vector3)worldOffset;
        Debug.Log("��Ʈ ����");
        hintSR.enabled = true; // ��� �ѱ�
    }

    // Event Trigger �� Pointer Exit �� ����
    public void OnPointerExitFromET()
    {
        if (!hintSR) return;
        hintSR.enabled = false; // ��� ����
    }
}
