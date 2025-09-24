using UnityEngine;

public class DoorToggle : MonoBehaviour
{
    [SerializeField] private Collider2D solid;
    [SerializeField] private SpriteRenderer sr;

    [Header("Sprites")]
    [SerializeField] private Sprite openSprite;   // ������ ��
    [SerializeField] private Sprite closedSprite; // ������ ��

    [SerializeField] private bool isOn; // true=����

    public bool IsOn => isOn;

    public void SetState(bool on)
    {
        isOn = on;

        if (solid) solid.enabled = !isOn;                    // ������ �浹 off
        if (sr) sr.sprite = isOn ? openSprite : closedSprite; // ��������Ʈ ��ü
    }

    public void Toggle() => SetState(!isOn);

    void Start() => SetState(isOn);
}
