using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleToggle : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Button toggleButton;           // ������ ��ư
    [SerializeField] private Image toggleImage;             // ��ư�� �̹���
    [SerializeField] private TextMeshProUGUI titleText;     // Ÿ��Ʋ ����

    [Header("Sprites")]
    [SerializeField] private Sprite offSprite;              // ������ �� �̹���
    [SerializeField] private Sprite onSprite;               // ������ �� �̹���

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
