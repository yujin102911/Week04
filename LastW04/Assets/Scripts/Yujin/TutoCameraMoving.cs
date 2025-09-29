using UnityEngine;
using System.Collections; // �ڷ�ƾ�� ����ϱ� ���� �ʿ��մϴ�.


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
        // �ڷ�ƾ�� ���۽�ŵ�ϴ�.
        StartCoroutine(SwitchAfterDelay());

    }

    private IEnumerator SwitchAfterDelay()
    {
        isSwitching = true; // ��ȯ ����!
        Debug.Log(switchDelay + "�� �ڿ� ī�޶�� �г��� ��ȯ�մϴ�...");

        // ���� �� �ڵ尡 �ٷ� �����Դϴ�! ����
        // ���⼭ switchDelay�� ������ �ð�(��)��ŭ �Լ��� ������ '�Ͻ�����'�մϴ�.
        // ���� ��ü�� ���ߴ� �� �ƴ϶�, �� �Լ��� ��� ����ϴ�.
        yield return new WaitForSeconds(switchDelay);

        // �Ͻ������� ������, �� �Ʒ��κ��� �ڵ尡 ���� ����˴ϴ�.
        Debug.Log("��ȯ!");

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

        isSwitching = false; // ��ȯ �Ϸ�! ���� �ٽ� ��ư�� ���� �� �ֽ��ϴ�.
    }

}
