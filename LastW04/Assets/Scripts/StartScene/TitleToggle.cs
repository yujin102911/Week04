using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleToggle : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Button toggleButton;           // 눌러줄 버튼
    [SerializeField] private Image toggleImage;             // 버튼의 이미지
    [SerializeField] private TextMeshProUGUI titleText;     // 타이틀 글자

    [Header("Sprites")]
    [SerializeField] private Sprite offSprite;              // 꺼졌을 때 이미지
    [SerializeField] private Sprite onSprite;               // 켜졌을 때 이미지

    [Header("Texts")]
    [SerializeField] private string offText;
    [SerializeField] private string onText;

    private bool isOn = false;

    private void Awake()
    {
        if (toggleButton != null)
            toggleButton.onClick.AddListener(OnToggle);
        ApplyState();
    }

    private void OnToggle()
    {
        isOn = !isOn;
        ApplyState();
    }

    private void ApplyState()
    {
        if (toggleImage != null)
            toggleImage.sprite = isOn ? onSprite : offSprite;

        if (titleText != null)
            titleText.text = isOn ? onText : offText;
    }
}
