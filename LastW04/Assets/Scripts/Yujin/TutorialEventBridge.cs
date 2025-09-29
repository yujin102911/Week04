using UnityEngine;
using UnityEngine.Rendering;

public class TutorialEventBridge : MonoBehaviour
{
    [Header("������ ���")]
    [Tooltip("ī�޶� ��ȯ�� CameraSwitcher ��ũ��Ʈ")]
    [SerializeField] private TutoCameraMoving cameraSwitcher;

    [Header("������Ʈ�� ����")]
    [Tooltip("�� �̺�Ʈ�� �������� �� LevelManager���� �˷��� ���ο� Region ID")]
    [SerializeField] private string targetRegionId;

    /// <summary>
    /// ��ư�� UnityEvent�� ������ ���� �Լ��Դϴ�.
    /// </summary>
    public void TriggerSwitchAndRegionUpdate()
    {
        // 1. ī�޶� ��ȯ�� ��û�մϴ�.
        if (cameraSwitcher != null)
        {
            cameraSwitcher.SwitchToNextStep(); // �ð� ���� ����� �ִ� �Լ� ȣ��
        }
        else
        {
            Debug.LogWarning("����� CameraSwitcher�� �����ϴ�!", this.gameObject);
        }

        // 2. LevelManager���� ���� ������ �ٲ���ٰ� �˷��ݴϴ�.
        if (LevelManager.Instance != null && !string.IsNullOrEmpty(targetRegionId))
        {
            // LevelManager�� public �Լ��� ȣ���Ͽ� ī�޶� ��ȯ ���� Region ID�� ������Ʈ�մϴ�.
            LevelManager.Instance.SetCurrentRegion(targetRegionId, affectCamera: false);
        }
        else
        {
            Debug.LogWarning("LevelManager�� ã�� �� ���ų� Target Region ID�� ����ֽ��ϴ�!", this.gameObject);
        }
    }
}