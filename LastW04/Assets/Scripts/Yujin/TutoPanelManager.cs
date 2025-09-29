using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;


public class TutoPanelManager : MonoBehaviour
{
    [SerializeField] private GameObject panel1;
    [SerializeField] private GameObject panel2;
    [SerializeField] private GameObject panel3;

    [Header("튜토리얼 UI 요소")]
    [SerializeField] private Animator mouseHintAnimator;

    private bool isHintPlaying = false;
    private bool isHintPlayingTwo = false;
    private bool isHintPlayingThree = false;
    private int tab1 = 0;
    private int tab2 = 0;
    private int tab3 = 0;

    private void Update()
    {
        string currentId = LevelManager.Instance.CurrentRegionId;

        if(currentId == "Region_07")
        {
            if (!panel1.activeInHierarchy)
            {
                panel1.SetActive(true); panel2.SetActive(false); panel3.SetActive(false);
            }
            if (Input.GetKeyDown(KeyCode.Tab)){
                if (!isHintPlaying && tab1 % 2 == 0)
                    StartCoroutine(PlayHintAnimationOnce());
                tab1++;
            }
        }
        if(currentId == "Region_08")
        {
            if (!panel2.activeInHierarchy)
            {
                panel1.SetActive(false); panel2.SetActive(true); panel3.SetActive(false);
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!isHintPlayingTwo && tab2 % 2 == 0)
                    StartCoroutine(PlayHintAnimationTwo());
                tab2++;
            }
        }
        if (currentId == "Region_09")
        {
            if (!panel3.activeInHierarchy)
            {
                panel1.SetActive(false) ; panel2.SetActive(false); panel3.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!isHintPlayingThree && tab3 % 2 == 0)
                    StartCoroutine(PlayHintAnimationThird());
                tab3++;
            }
        }
        if(currentId != "Region_07" && currentId != "Region_08" && currentId != "Region_09")
        {
            panel1.SetActive(false);
            panel2.SetActive(false);
            panel3.SetActive(false);
        }

    }
    private IEnumerator PlayHintAnimationOnce()
    {
        isHintPlaying = true;

        if (mouseHintAnimator != null)
        {
            // 1. 애니메이션 시작 전에 힌트 오브젝트를 활성화합니다.
            mouseHintAnimator.gameObject.SetActive(true);

            // 2. "MouseDragHint" 라는 이름의 애니메이션 클립 길이를 미리 안전하게 찾아옵니다.
            float clipLength = GetAnimationClipLength("Tuto1");
            if (clipLength == 0f)
            {
                Debug.LogError("애니메이션 클립 'MouseDragHint'를 찾을 수 없거나 길이가 0입니다.");
                isHintPlaying = false;
                yield break; // 코루틴을 안전하게 중단합니다.
            }

            // 3. 애니메이션을 실행합니다.
            mouseHintAnimator.SetTrigger("DoDrag");

            // 4. 애니메이션이 끝날 때까지 기다립니다.
            yield return new WaitForSeconds(clipLength);

            // 5. 애니메이션이 끝나면 힌트 오브젝트를 비활성화합니다.
            mouseHintAnimator.gameObject.SetActive(false);
        }

        isHintPlaying = false; // 힌트 재생이 끝났음을 알립니다.
    }
    private IEnumerator PlayHintAnimationTwo()
    {
        isHintPlaying = true;

        if (mouseHintAnimator != null)
        {
            // 1. 애니메이션 시작 전에 힌트 오브젝트를 활성화합니다.
            mouseHintAnimator.gameObject.SetActive(true);

            // 2. "MouseDragHint" 라는 이름의 애니메이션 클립 길이를 미리 안전하게 찾아옵니다.
            float clipLength = GetAnimationClipLength("Tuto2");
            if (clipLength == 0f)
            {
                Debug.LogError("애니메이션 클립 'MouseDragHint'를 찾을 수 없거나 길이가 0입니다.");
                isHintPlaying = false;
                yield break; // 코루틴을 안전하게 중단합니다.
            }

            // 3. 애니메이션을 실행합니다.
            mouseHintAnimator.SetTrigger("DoDrag2");

            // 4. 애니메이션이 끝날 때까지 기다립니다.
            yield return new WaitForSeconds(clipLength);

            // 5. 애니메이션이 끝나면 힌트 오브젝트를 비활성화합니다.

        }

        isHintPlaying = false; // 힌트 재생이 끝났음을 알립니다.
    }
    private IEnumerator PlayHintAnimationThird()
    {
        isHintPlaying = true;

        if (mouseHintAnimator != null)
        {
            // 1. 애니메이션 시작 전에 힌트 오브젝트를 활성화합니다.
            mouseHintAnimator.gameObject.SetActive(true);

            // 2. "MouseDragHint" 라는 이름의 애니메이션 클립 길이를 미리 안전하게 찾아옵니다.
            float clipLength = GetAnimationClipLength("Tuto3");
            if (clipLength == 0f)
            {
                Debug.LogError("애니메이션 클립 'MouseDragHint'를 찾을 수 없거나 길이가 0입니다.");
                isHintPlaying = false;
                yield break; // 코루틴을 안전하게 중단합니다.
            }

            // 3. 애니메이션을 실행합니다.
            mouseHintAnimator.SetTrigger("DoDrag3");

            // 4. 애니메이션이 끝날 때까지 기다립니다.
            yield return new WaitForSeconds(clipLength);

            // 5. 애니메이션이 끝나면 힌트 오브젝트를 비활성화합니다.
            mouseHintAnimator.gameObject.SetActive(false);
        }

        isHintPlaying = false; // 힌트 재생이 끝났음을 알립니다.
    }
    // 애니메이터에 포함된 모든 클립 중에서 특정 이름의 클립 길이를 찾아 반환하는 안정적인 함수입니다.
    private float GetAnimationClipLength(string clipName)
    {
        if (mouseHintAnimator == null || mouseHintAnimator.runtimeAnimatorController == null) return 0f;

        foreach (var clip in mouseHintAnimator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }
        return 0f;
    }

}
