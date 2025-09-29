using UnityEngine;
using System.Collections;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("Ʃ�丮�� UI ���")]
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
    /// ��Ʈ �ִϸ��̼��� �� �� ����ϰ� ������� �ϴ� �ڷ�ƾ�Դϴ�.
    /// </summary>
    private IEnumerator PlayHintAnimationOnce()
    {
        isHintPlaying = true;

        if (mouseHintAnimator != null)
        {
            // 1. �ִϸ��̼� ���� ���� ��Ʈ ������Ʈ�� Ȱ��ȭ�մϴ�.
            mouseHintAnimator.gameObject.SetActive(true);

            // 2. "MouseDragHint" ��� �̸��� �ִϸ��̼� Ŭ�� ���̸� �̸� �����ϰ� ã�ƿɴϴ�.
            float clipLength = GetAnimationClipLength("Tuto1");
            if (clipLength == 0f)
            {
                Debug.LogError("�ִϸ��̼� Ŭ�� 'MouseDragHint'�� ã�� �� ���ų� ���̰� 0�Դϴ�.");
                isHintPlaying = false;
                yield break; // �ڷ�ƾ�� �����ϰ� �ߴ��մϴ�.
            }

            // 3. �ִϸ��̼��� �����մϴ�.
            mouseHintAnimator.SetTrigger("DoDrag");

            // 4. �ִϸ��̼��� ���� ������ ��ٸ��ϴ�.
            yield return new WaitForSeconds(clipLength);

            // 5. �ִϸ��̼��� ������ ��Ʈ ������Ʈ�� ��Ȱ��ȭ�մϴ�.
            mouseHintAnimator.gameObject.SetActive(false);
        }

        isHintPlaying = false; // ��Ʈ ����� �������� �˸��ϴ�.
    }
    private IEnumerator PlayHintAnimationTwo()
    {
        isHintPlaying = true;

        if (mouseHintAnimator != null)
        {
            // 1. �ִϸ��̼� ���� ���� ��Ʈ ������Ʈ�� Ȱ��ȭ�մϴ�.
            mouseHintAnimator.gameObject.SetActive(true);

            // 2. "MouseDragHint" ��� �̸��� �ִϸ��̼� Ŭ�� ���̸� �̸� �����ϰ� ã�ƿɴϴ�.
            float clipLength = GetAnimationClipLength("Tuto2");
            if (clipLength == 0f)
            {
                Debug.LogError("�ִϸ��̼� Ŭ�� 'MouseDragHint'�� ã�� �� ���ų� ���̰� 0�Դϴ�.");
                isHintPlaying = false;
                yield break; // �ڷ�ƾ�� �����ϰ� �ߴ��մϴ�.
            }

            // 3. �ִϸ��̼��� �����մϴ�.
            mouseHintAnimator.SetTrigger("DoDrag2");

            // 4. �ִϸ��̼��� ���� ������ ��ٸ��ϴ�.
            yield return new WaitForSeconds(clipLength);

            // 5. �ִϸ��̼��� ������ ��Ʈ ������Ʈ�� ��Ȱ��ȭ�մϴ�.
                
        }

        isHintPlaying = false; // ��Ʈ ����� �������� �˸��ϴ�.
    }
    private IEnumerator PlayHintAnimationThird()
    {
        isHintPlaying = true;

        if (mouseHintAnimator != null)
        {
            // 1. �ִϸ��̼� ���� ���� ��Ʈ ������Ʈ�� Ȱ��ȭ�մϴ�.
            mouseHintAnimator.gameObject.SetActive(true);

            // 2. "MouseDragHint" ��� �̸��� �ִϸ��̼� Ŭ�� ���̸� �̸� �����ϰ� ã�ƿɴϴ�.
            float clipLength = GetAnimationClipLength("Tuto3");
            if (clipLength == 0f)
            {
                Debug.LogError("�ִϸ��̼� Ŭ�� 'MouseDragHint'�� ã�� �� ���ų� ���̰� 0�Դϴ�.");
                isHintPlaying = false;
                yield break; // �ڷ�ƾ�� �����ϰ� �ߴ��մϴ�.
            }

            // 3. �ִϸ��̼��� �����մϴ�.
            mouseHintAnimator.SetTrigger("DoDrag3");

            // 4. �ִϸ��̼��� ���� ������ ��ٸ��ϴ�.
            yield return new WaitForSeconds(clipLength);

            // 5. �ִϸ��̼��� ������ ��Ʈ ������Ʈ�� ��Ȱ��ȭ�մϴ�.
            mouseHintAnimator.gameObject.SetActive(false);
        }

        isHintPlaying = false; // ��Ʈ ����� �������� �˸��ϴ�.
    }
    // �ִϸ����Ϳ� ���Ե� ��� Ŭ�� �߿��� Ư�� �̸��� Ŭ�� ���̸� ã�� ��ȯ�ϴ� �������� �Լ��Դϴ�.
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
        Debug.Log("Ʃ�丮�� �ܰ谡 " + tutorialStep + "(��)�� ����Ǿ����ϴ�.");
    }
}