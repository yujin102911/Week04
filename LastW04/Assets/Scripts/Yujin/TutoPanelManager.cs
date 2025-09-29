using UnityEngine;
using System.Collections; // �ڷ�ƾ�� ����ϱ� ���� �ʿ��մϴ�.


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
        Debug.Log($"{switchDelay}�� �ڿ� �г� ��ȯ!");

        yield return new WaitForSeconds(switchDelay);

        if (panelToDisable != null)
            panelToDisable.SetActive(false);

        if (panelToEnable != null)
            panelToEnable.SetActive(true);

        Debug.Log("�г� ��ȯ �Ϸ�!");
        isSwitching = false;
    }

}
