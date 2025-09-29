using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // �ڷ�ƾ�� ����ϱ� ���� �ʼ������� �߰��ؾ� �մϴ�.

public class SceneManagement : MonoBehaviour
{
    public void LoadSceneByNameWithDelay(string sceneName)
    {
        // ���� �޾ƿ� �� �̸��� ����ִٸ�, ������ ����ϰ� �Լ��� �����մϴ�.
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("�� �̸��� ����ֽ��ϴ�! �� �̸��� ��Ȯ�� �Է����ּ���.");
            return;
        }

        // �ڷ�ƾ�� ���۽�ŵ�ϴ�. �� �̸��� ���ڷ� �����մϴ�.
        StartCoroutine(LoadSceneAfterDelay(sceneName));
    }

    /// <summary>
    /// �ð����� �ΰ� ���� �ҷ����� Ư���� �Լ�, �ڷ�ƾ�Դϴ�.
    /// </summary>
    /// <param name="sceneName">�ҷ��� ���� �̸�</param>
    private IEnumerator LoadSceneAfterDelay(string sceneName)
    {
        Debug.Log("1�� �ڿ� " + sceneName + " ���� �ҷ��ɴϴ�...");

        // ���⼭ 1�� ���� �Լ��� ������ '�Ͻ�����'�մϴ�.
        yield return new WaitForSeconds(1f);

        // 1�ʰ� ������, �� �Ʒ��κ��� �ڵ尡 ���� ����˴ϴ�.
        Debug.Log(sceneName + " �� �ε�!");
        SceneManager.LoadScene(sceneName);
    }

}
