using UnityEngine;
using UnityEngine.Events;

public class SimpleButton : MonoBehaviour
{
    [Tooltip("�ٸ� ��ũ��Ʈ���� �� ��ư�� ���ȴ��� Ȯ���� �� ����ϴ� ����")]
    public bool IsPressed { get; private set; } = false; //�⺻������ false : �ȴ��� ����

    private int occupantCount = 0; //��ư ���� �ö�� ������Ʈ�� ��

    [Header("��ư ��������Ʈ")]
    [SerializeField, Tooltip("��ư ����(��������Ʈ������)")]
    private SpriteRenderer buttonSpriteRenderer;
    [SerializeField, Tooltip("��ư �ȴ����� �� ���� ��������Ʈ")]
    private Sprite unpressedSprite;
    [SerializeField, Tooltip("��ư ������ �� ���� ��������Ʈ")]
    private Sprite pressedSprite;

    [Header("�̺�Ʈ")]
    public UnityEvent onPressed;
    public UnityEvent onReleased;

    private void Awake()
    {
        if (buttonSpriteRenderer == null)
        {
            buttonSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        UpdateSprite();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            occupantCount++;
            UpdatePressedState();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            occupantCount--;
            UpdatePressedState();
        }
    }

    private void UpdatePressedState()
    {
        bool shouldBePressed = occupantCount > 0;

        if (shouldBePressed != IsPressed)
        {
            IsPressed = shouldBePressed;
            Debug.Log("��ư ���� ����: " + IsPressed);
            UpdateSprite();
            if(IsPressed) onPressed.Invoke();
            else onReleased.Invoke();
        }
    }
    private void UpdateSprite()
    {
        if (buttonSpriteRenderer == null) return;
        if (IsPressed)
        {
            if(pressedSprite != null) buttonSpriteRenderer.sprite = pressedSprite;
        }
        else
        {
            if(unpressedSprite != null) buttonSpriteRenderer.sprite = unpressedSprite; 
        }
    }
}
