using UnityEngine;

public class SimpleDoor: MonoBehaviour
{
    [Header("�������")]
    [SerializeField] private SpriteRenderer doorSpriteRenderer;
    [SerializeField] private Collider2D doorCollider;

    [Header("��������Ʈ")]
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
        Debug.Log("�� ����");
        if (doorSpriteRenderer != null && openSprite != null)
        {
            doorSpriteRenderer.sprite = openSprite;
        } 
        if(doorCollider != null) doorCollider.enabled = false;
    }
    public void CloseDoor()
    {
        Debug.Log("�� ����");
        if(doorSpriteRenderer != null && closedSprite != null)
        {
            doorSpriteRenderer.sprite = closedSprite;
            if(doorCollider!= null) doorCollider.enabled = true;
        }
    }

}