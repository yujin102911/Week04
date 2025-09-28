using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleToggle : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Button toggleButton;           // 눌러줄 버튼
    [SerializeField] private Image toggleImage;             // 버튼의 이미지
    [SerializeField] private TextMeshProUGUI titleText;     // 타이틀 글자
    [SerializeField] private TextMeshProUGUI slideText;     // 슬라이드 안내 글자

    [Header("Sprites")]
    [SerializeField] private Sprite offSprite;              // 꺼졌을 때 이미지
    [SerializeField] private Sprite onSprite;               // 켜졌을 때 이미지

    [Header("Texts")]
    [SerializeField] private string offText;
    [SerializeField] private string onText;
    [SerializeField] private string slideOffText;
    [SerializeField] private string slideOnText;

    [Header("Font Size Options")]
    [SerializeField] private float onFontScale = 0.9f;      // ON일 때 줄일 비율 (예: 0.9 → 90%)

    private bool isOn = false;
    private float defaultTitleSize;
    private float defaultSlideSize;

    private void Awake()
    {
        if (toggleButton != null)
            toggleButton.onClick.AddListener(OnToggle);

        // 기본 폰트 사이즈 저장
        if (titleText != null) defaultTitleSize = titleText.fontSize;
        if (slideText != null) defaultSlideSize = slideText.fontSize;

        ApplyState();
    }

    private void OnToggle()
    {
        isOn = !isOn;
        ApplyState();
    }

    private void ApplyState()
    {
        // 이미지 교체
        if (toggleImage != null)
            toggleImage.sprite = isOn ? onSprite : offSprite;

        // 텍스트 교체
        if (titleText != null)
        {
            titleText.text = isOn ? onText : offText;
            //titleText.fontSize = isOn ? defaultTitleSize * onFontScale : defaultTitleSize;
        }

        /*
        if (slideText != null)
        {
            slideText.text = isOn ? slideOnText : slideOffText;
            slideText.fontSize = isOn ? defaultSlideSize * onFontScale : defaultSlideSize;
        }
        */
    }
}
