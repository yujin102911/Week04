using UnityEngine;

[DisallowMultipleComponent]
public sealed class DoorSwitch : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private SpriteRenderer doorRenderer; // 문을 보여줄 SpriteRenderer
    [SerializeField] private Sprite closedSprite;         // 닫힌 상태 스프라이트
    [SerializeField] private Sprite openSprite;           // 열린 상태 스프라이트

    [Header("Collider")]
    [SerializeField] private Collider2D doorCollider;     // 문을 막는 콜라이더
    [SerializeField] private Collider2D[] attachPointColliders; // 문 위/주변 부착점 콜라이더들

    [Header("Options")]
    [SerializeField] private bool openDisablesCollider = true; // 열리면 콜라이더 비활성
    [SerializeField] private bool openDisablesAttachPoints = true; // 열리면 부착점도 비활성

    private bool isOpen;

    public void ApplyState(bool isPressed)
    {
        SetOpen(isPressed);
    }

    public void SetOpen(bool open)
    {
        if (isOpen == open) return; // 이미 같은 상태면 패스
        isOpen = open;

        // 메인 문 콜라이더 처리
        if (doorCollider && openDisablesCollider)
            doorCollider.enabled = !open;

        // 부착점 콜라이더 처리
        if (attachPointColliders != null && openDisablesAttachPoints)
        {
            foreach (var col in attachPointColliders)
            {
                if (col) col.enabled = !open;
            }
        }

        // 스프라이트 교체
        if (doorRenderer)
        {
            doorRenderer.sprite = open ? openSprite : closedSprite;
        }
    }
}
