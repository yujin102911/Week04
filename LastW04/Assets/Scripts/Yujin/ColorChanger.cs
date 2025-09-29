using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    [Header("����")]
    [Tooltip("������ ������ ��� Sprite Renderer")]
    [SerializeField] private SpriteRenderer targetSpriteRenderer;

    [Tooltip("�⺻ ���� (��: ������)")]
    [SerializeField] private Color defaultColor = Color.red;

    [Tooltip("������ ���� (��: �ʷϻ�)")]
    [SerializeField] private Color targetColor = Color.green;

    // ������ ���۵� �� ȣ��˴ϴ�.
    private void Awake()
    {
        // ���� �ν����Ϳ��� Sprite Renderer�� ���� �������� �ʾҴٸ�,
        // �� ������Ʈ�� �پ��ִ� Sprite Renderer�� �ڵ����� ã�ƺ��ϴ�.
        if (targetSpriteRenderer == null)
        {
            targetSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        // ���� �� �⺻ �������� �����մϴ�.
        SetToDefaultColor();
    }

    /// <summary>
    /// ������ Target Color(�ʷϻ�)�� �����մϴ�.
    /// </summary>
    public void ChangeToTargetColor()
    {
        if (targetSpriteRenderer != null)
        {
            targetSpriteRenderer.color = targetColor;
        }
    }

    /// <summary>
    /// ������ Default Color(������)�� �ǵ����ϴ�.
    /// </summary>
    public void SetToDefaultColor()
    {
        if (targetSpriteRenderer != null)
        {
            targetSpriteRenderer.color = defaultColor;
        }
    }

    /// <summary>
    /// �� ���� ������ ������ ���� ����մϴ�.
    /// </summary>
    public void ToggleColor()
    {
        if (targetSpriteRenderer != null)
        {
            // ���� ������ �⺻ ����� ���ٸ� Ÿ�� ��������, �׷��� �ʴٸ� �⺻ �������� �����մϴ�.
            if (targetSpriteRenderer.color == defaultColor)
            {
                targetSpriteRenderer.color = targetColor;
            }
            else
            {
                targetSpriteRenderer.color = defaultColor;
            }
        }
    }
}
