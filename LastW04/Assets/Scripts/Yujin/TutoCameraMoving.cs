using UnityEngine;
using System.Collections; // 코루틴을 사용하기 위해 필요합니다.


public class TutoCameraMoving : MonoBehaviour
{
    [SerializeField] private Camera cameraToDisable;
    [SerializeField] private Camera cameraToEnable;

    [SerializeField] private GameObject panelToDisable;
    [SerializeField] private GameObject panelToEnable;



    [SerializeField] private float switchDelay = 1.0f;

    private bool isSwitching = false;

    public void SwitchToNextStep()
    {
        if (isSwitching)
        {
            return;
        }
        // 코루틴을 시작시킵니다.
        StartCoroutine(SwitchAfterDelay());

    }

    private IEnumerator SwitchAfterDelay()
    {
        isSwitching = true; // 전환 시작!
        Debug.Log(switchDelay + "초 뒤에 카메라와 패널을 전환합니다...");

        // ▼▼▼ 이 코드가 바로 마법입니다! ▼▼▼
        // 여기서 switchDelay에 설정된 시간(초)만큼 함수의 실행을 '일시정지'합니다.
        // 게임 전체가 멈추는 게 아니라, 이 함수만 잠시 멈춥니다.
        yield return new WaitForSeconds(switchDelay);

        // 일시정지가 끝나면, 이 아랫부분의 코드가 마저 실행됩니다.
        Debug.Log("전환!");

        if (cameraToDisable != null)
        {
            cameraToDisable.gameObject.SetActive(false);
        }

        if (cameraToEnable != null)
        {
            cameraToEnable.gameObject.SetActive(true);
        }
        if (panelToDisable != null)
        {
            panelToDisable.SetActive(false);
        }
        if (panelToEnable != null)
        {
            panelToEnable.SetActive(true);
        }

        isSwitching = false; // 전환 완료! 이제 다시 버튼을 누를 수 있습니다.
    }

}
