using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    [Header("설정")]
    [Tooltip("색상을 변경할 대상 Sprite Renderer")]
    [SerializeField] private SpriteRenderer targetSpriteRenderer;

    [Tooltip("기본 색상 (예: 빨간색)")]
    [SerializeField] private Color defaultColor = Color.red;

    [Tooltip("변경할 색상 (예: 초록색)")]
    [SerializeField] private Color targetColor = Color.green;

    // 게임이 시작될 때 호출됩니다.
    private void Awake()
    {
        // 만약 인스펙터에서 Sprite Renderer를 직접 연결하지 않았다면,
        // 이 오브젝트에 붙어있는 Sprite Renderer를 자동으로 찾아봅니다.
        if (targetSpriteRenderer == null)
        {
            targetSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        // 시작 시 기본 색상으로 설정합니다.
        SetToDefaultColor();
    }

    /// <summary>
    /// 색상을 Target Color(초록색)로 변경합니다.
    /// </summary>
    public void ChangeToTargetColor()
    {
        if (targetSpriteRenderer != null)
        {
            targetSpriteRenderer.color = targetColor;
        }
    }

    /// <summary>
    /// 색상을 Default Color(빨간색)로 되돌립니다.
    /// </summary>
    public void SetToDefaultColor()
    {
        if (targetSpriteRenderer != null)
        {
            targetSpriteRenderer.color = defaultColor;
        }
    }

    /// <summary>
    /// 두 가지 색상을 번갈아 가며 토글합니다.
    /// </summary>
    public void ToggleColor()
    {
        if (targetSpriteRenderer != null)
        {
            // 현재 색상이 기본 색상과 같다면 타겟 색상으로, 그렇지 않다면 기본 색상으로 변경합니다.
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
