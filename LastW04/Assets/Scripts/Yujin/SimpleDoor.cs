using UnityEngine;

public class SimpleDoor: MonoBehaviour
{
    [Header("구성요소")]
    [SerializeField] private SpriteRenderer doorSpriteRenderer;
    [SerializeField] private Collider2D doorCollider;

    [Header("스프라이트")]
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openSprite;

    private void Awake()
    {
        if (doorSpriteRenderer == null)
        {
            doorSpriteRenderer = GetComponent<SpriteRenderer>();
        }
        if(doorSpriteRenderer != null && closedSprite!=null)doorSpriteRenderer.sprite = closedSprite;
        if (doorCollider != null) doorCollider.enabled = true;
    }

    public void OpenDoor()
    {
        Debug.Log("문 열림");
        if (doorSpriteRenderer != null && openSprite != null)
        {
            doorSpriteRenderer.sprite = openSprite;
        } 
        if(doorCollider != null) doorCollider.enabled = false;
    }
    public void CloseDoor()
    {
        Debug.Log("문 닫힘");
        if(doorSpriteRenderer != null && closedSprite != null)
        {
            doorSpriteRenderer.sprite = closedSprite;
            if(doorCollider!= null) doorCollider.enabled = true;
        }
    }

}