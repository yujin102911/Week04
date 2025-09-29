using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // 코루틴을 사용하기 위해 필수적으로 추가해야 합니다.

public class SceneManagement : MonoBehaviour
{
    public void LoadSceneByNameWithDelay(string sceneName)
    {
        // 만약 받아온 씬 이름이 비어있다면, 오류를 출력하고 함수를 종료합니다.
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("씬 이름이 비어있습니다! 씬 이름을 정확히 입력해주세요.");
            return;
        }

        // 코루틴을 시작시킵니다. 씬 이름을 인자로 전달합니다.
        StartCoroutine(LoadSceneAfterDelay(sceneName));
    }

    /// <summary>
    /// 시간차를 두고 씬을 불러오는 특별한 함수, 코루틴입니다.
    /// </summary>
    /// <param name="sceneName">불러올 씬의 이름</param>
    private IEnumerator LoadSceneAfterDelay(string sceneName)
    {
        Debug.Log("1초 뒤에 " + sceneName + " 씬을 불러옵니다...");

        // 여기서 1초 동안 함수의 실행을 '일시정지'합니다.
        yield return new WaitForSeconds(1f);

        // 1초가 지나면, 이 아랫부분의 코드가 마저 실행됩니다.
        Debug.Log(sceneName + " 씬 로딩!");
        SceneManager.LoadScene(sceneName);
    }

}
