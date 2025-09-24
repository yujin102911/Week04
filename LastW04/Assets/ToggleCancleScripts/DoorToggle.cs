using UnityEngine;

public class DoorToggle : MonoBehaviour
{
    [SerializeField] private Collider2D solid;
    [SerializeField] private SpriteRenderer sr;

    [Header("Sprites")]
    [SerializeField] private Sprite openSprite;   // 열렸을 때
    [SerializeField] private Sprite closedSprite; // 닫혔을 때

    [SerializeField] private bool isOn; // true=열림

    public bool IsOn => isOn;

    public void SetState(bool on)
    {
        isOn = on;

        if (solid) solid.enabled = !isOn;                    // 열리면 충돌 off
        if (sr) sr.sprite = isOn ? openSprite : closedSprite; // 스프라이트 교체
    }

    public void Toggle() => SetState(!isOn);

    void Start() => SetState(isOn);
}
