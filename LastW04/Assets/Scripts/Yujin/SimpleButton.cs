using UnityEngine;
using UnityEngine.Events;

public class SimpleButton : MonoBehaviour
{
    [Tooltip("다른 스크립트에서 이 버튼이 눌렸는지 확인할 때 사용하는 변수")]
    public bool IsPressed { get; private set; } = false; //기본적으로 false : 안눌린 상태

    private int occupantCount = 0; //버튼 위에 올라온 오브젝트의 수

    [Header("버튼 스프라이트")]
    [SerializeField, Tooltip("버튼 몸통(스프라이트렌더러)")]
    private SpriteRenderer buttonSpriteRenderer;
    [SerializeField, Tooltip("버튼 안눌렸을 때 보일 스프라이트")]
    private Sprite unpressedSprite;
    [SerializeField, Tooltip("버튼 눌렸을 때 보일 스프라이트")]
    private Sprite pressedSprite;

    [Header("이벤트")]
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
            Debug.Log("버튼 상태 변경: " + IsPressed);
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
