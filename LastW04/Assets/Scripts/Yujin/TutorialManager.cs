using UnityEngine;
using System.Collections;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("튜토리얼 UI 요소")]
    [SerializeField] private Animator mouseHintAnimator;

    private int tutorialStep = 0;
    private bool isHintPlaying = false;
    private bool isHintPlayingTwo = false;
    private bool isHintPlayingThree = false;
    private int tab1 = 0;
    private int tab2 = 0;
    private int tab3 = 0;

    public Camera camera1;
    public Camera camera2;
    public Camera camera3;

    [SerializeField] private TextMeshProUGUI tutoText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            
            if(!isHintPlaying && camera1.gameObject.activeInHierarchy && tab1 % 2 == 0)
                StartCoroutine(PlayHintAnimationOnce());

            if (!isHintPlayingTwo && camera2.gameObject.activeInHierarchy && tab2 % 2 == 0)
                StartCoroutine(PlayHintAnimationTwo());

            if (!isHintPlayingThree && camera3.gameObject.activeInHierarchy && tab3 % 2 == 0)
                StartCoroutine(PlayHintAnimationThird());

            if (camera1.gameObject.activeInHierarchy) tab1++;
            if (camera2.gameObject.activeInHierarchy)tab2++;
            if (camera3.gameObject.activeInHierarchy) tab3++;

        }
    }

    /// <summary>
    /// 힌트 애니메이션을 한 번 재생하고 사라지게 하는 코루틴입니다.
    /// </summary>
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

    public void AdvanceTutorialStep()
    {
        tutorialStep++;
        Debug.Log("튜토리얼 단계가 " + tutorialStep + "(으)로 변경되었습니다.");
    }
}