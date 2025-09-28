using UnityEngine;

[DisallowMultipleComponent]
public class DoorToggle : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openSprite;

    [Header("Blockers (막는 콜라이더들)")]
    [Tooltip("문 본체를 막는 콜라이더 1개(없으면 비워도 됨)")]
    [SerializeField] private Collider2D doorCollider;
    [Tooltip("자식의 AttachPoint 등, 함께 꺼야 하는 추가 콜라이더들")]
    [SerializeField] private Collider2D[] extraColliders;

    [Header("State")]
    [SerializeField] private bool isOpen = false;

    [Header("Options")]
    [Tooltip("열렸을 때 위의 모든 콜라이더들을 비활성화할지 여부")]
    [SerializeField] private bool disableCollidersWhenOpen = true;

    public void Toggle() => SetOpen(!isOpen);

    public void SetOpen(bool open)
    {
        isOpen = open;

        // 스프라이트
        if (sr) sr.sprite = isOpen ? openSprite : closedSprite;

        // 콜라이더 on/off (문 + 추가들 전부)
        bool enable = !(disableCollidersWhenOpen && isOpen);
        if (doorCollider) doorCollider.enabled = enable;

        if (extraColliders != null)
        {
            for (int i = 0; i < extraColliders.Length; i++)
            {
                if (extraColliders[i]) extraColliders[i].enabled = enable;
            }
        }

        Physics2D.SyncTransforms();
    }

    private void Start()
    {
        // 시작 시 상태 반영
        SetOpen(isOpen);
    }
}
