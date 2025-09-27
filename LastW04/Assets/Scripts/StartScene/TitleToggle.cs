using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleToggle : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Button toggleButton;           // ������ ��ư
    [SerializeField] private Image toggleImage;             // ��ư�� �̹���
    [SerializeField] private TextMeshProUGUI titleText;     // Ÿ��Ʋ ����
    [SerializeField] private TextMeshProUGUI slideText;     // �����̵� �ȳ� ����

    [Header("Sprites")]
    [SerializeField] private Sprite offSprite;              // ������ �� �̹���
    [SerializeField] private Sprite onSprite;               // ������ �� �̹���

    [Header("Texts")]
    [SerializeField] private string offText;
    [SerializeField] private string onText;
    [SerializeField] private string slideOffText;
    [SerializeField] private string slideOnText;

    [Header("Font Size Options")]
    [SerializeField] private float onFontScale = 0.9f;      // ON�� �� ���� ���� (��: 0.9 �� 90%)

    private bool isOn = false;
    private float defaultTitleSize;
    private float defaultSlideSize;

    private void Awake()
    {
        if (toggleButton != null)
            toggleButton.onClick.AddListener(OnToggle);

        // �⺻ ��Ʈ ������ ����
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
        // �̹��� ��ü
        if (toggleImage != null)
            toggleImage.sprite = isOn ? onSprite : offSprite;

        // �ؽ�Ʈ ��ü
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
