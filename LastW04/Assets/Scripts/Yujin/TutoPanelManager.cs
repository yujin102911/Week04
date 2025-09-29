using UnityEngine;
using System.Collections; // 코루틴을 사용하기 위해 필요합니다.


public class TutoPanelManager : MonoBehaviour
{
    [SerializeField] private GameObject panel1;
    [SerializeField] private GameObject panel2;
    [SerializeField] private GameObject panel3;

    [SerializeField] private float switchDelay = 1.0f;

    private bool isSwitching = false;

    private void Update()
    {
    }
    public void SwitchPanel1To2()
    {
        if (!isSwitching)
            StartCoroutine(SwitchPanels(panel1, panel2));
    }

    public void SwitchPanel2To3()
    {
        if (!isSwitching)
            StartCoroutine(SwitchPanels(panel2, panel3));
    }

    public void DisablePanel3()
    {
        if (panel3 != null)
            panel3.SetActive(false);
    }

    private IEnumerator SwitchPanels(GameObject panelToDisable, GameObject panelToEnable)
    {
        isSwitching = true;
        Debug.Log($"{switchDelay}초 뒤에 패널 전환!");

        yield return new WaitForSeconds(switchDelay);

        if (panelToDisable != null)
            panelToDisable.SetActive(false);

        if (panelToEnable != null)
            panelToEnable.SetActive(true);

        Debug.Log("패널 전환 완료!");
        isSwitching = false;
    }

}
