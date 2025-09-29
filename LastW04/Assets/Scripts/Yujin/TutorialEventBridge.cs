using UnityEngine;
using UnityEngine.Rendering;

public class TutorialEventBridge : MonoBehaviour
{
    [Header("연결할 대상")]
    [Tooltip("카메라를 전환할 CameraSwitcher 스크립트")]
    [SerializeField] private TutoCameraMoving cameraSwitcher;

    [Header("업데이트할 정보")]
    [Tooltip("이 이벤트를 실행했을 때 LevelManager에게 알려줄 새로운 Region ID")]
    [SerializeField] private string targetRegionId;

    /// <summary>
    /// 버튼의 UnityEvent에 연결할 메인 함수입니다.
    /// </summary>
    public void TriggerSwitchAndRegionUpdate()
    {
        // 1. 카메라 전환을 요청합니다.
        if (cameraSwitcher != null)
        {
            cameraSwitcher.SwitchToNextStep(); // 시간 지연 기능이 있는 함수 호출
        }
        else
        {
            Debug.LogWarning("연결된 CameraSwitcher가 없습니다!", this.gameObject);
        }

        // 2. LevelManager에게 현재 지역이 바뀌었다고 알려줍니다.
        if (LevelManager.Instance != null && !string.IsNullOrEmpty(targetRegionId))
        {
            // LevelManager의 public 함수를 호출하여 카메라 전환 없이 Region ID만 업데이트합니다.
            LevelManager.Instance.SetCurrentRegion(targetRegionId, affectCamera: false);
        }
        else
        {
            Debug.LogWarning("LevelManager를 찾을 수 없거나 Target Region ID가 비어있습니다!", this.gameObject);
        }
    }
}