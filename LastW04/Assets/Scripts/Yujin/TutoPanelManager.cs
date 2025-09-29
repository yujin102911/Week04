using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;


public class TutoPanelManager : MonoBehaviour
{
    [SerializeField] private GameObject panel1;
    [SerializeField] private GameObject panel2;
    [SerializeField] private GameObject panel3;

    [Header("Ʃ�丮�� UI ���")]
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

}
