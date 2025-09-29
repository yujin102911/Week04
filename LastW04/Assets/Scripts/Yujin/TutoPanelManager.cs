using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class TutoPanelManager : MonoBehaviour
{
    [Header("Ʃ�丮�� �г�")]
    [SerializeField] private GameObject panel1;   // Region_07
    [SerializeField] private GameObject panel2;   // Region_08
    [SerializeField] private GameObject panel3;   // Region_09

    [Header("Ʃ�丮�� �ؽ�Ʈ (TMP)")]
    [SerializeField] private TextMeshProUGUI panel1Text;
    [SerializeField] private TextMeshProUGUI panel2Text;
    [SerializeField] private TextMeshProUGUI panel3Text;

    [Header("�߰� ����(Sub) �г�")]
    [SerializeField] private GameObject subPanel;
    [SerializeField] private TextMeshProUGUI subPanelText;

    [Header("��Ʈ �ִϸ��̼�")]
    [SerializeField] private Animator mouseHintAnimator;

    [Header("���庰 ��ġ/���� ���� ��� (UIDragManager)")]
    [Tooltip("Region_07: �����̴� ��ġ ������")]
    [SerializeField] private UIDragManager sliderManager_R1;
    [Tooltip("Region_08: ��� ��ġ ������")]
    [SerializeField] private UIDragManager toggleManager_R2;
    [Tooltip("Region_09: X ��ġ/���� ������")]
    [SerializeField] private UIDragManager xManager_R3;

    [Header("���庰 ��ư(Ʈ����) ���� ��� (SimpleButton)")]
    [Tooltip("Region_07: ��ư ������ ������")]
    [SerializeField] private SimpleButton[] buttons_R1;
    [Tooltip("Region_08: ��ư ������ ������")]
    [SerializeField] private SimpleButton[] buttons_R2;

    [Header("Ÿ����(�� �پ� ǥ��) �ɼ�")]
    [SerializeField, Min(0f)] private float lineInterval = 0.35f;
    [SerializeField] private bool restartTypingOnChange = true;

    [Header("Ʃ�丮�� ĵ���� ��Ʈ")]
    [SerializeField] private GameObject tutorialCanvasRoot; // Ʃ�丮�� ��ü ĵ����

    // ���� ����(�ִϸ��̼�/�� ���)
    private bool isHintPlaying, isHintPlayingTwo, isHintPlayingThree;
    private int tab1, tab2, tab3;

    // ���� ������ ����: ������� ����/���� ����
    private bool editEnabledOnce;   // �� �������� Tap���� ���� ��� ON
    private bool editDisabledOnce;  // �� �������� Tap���� ���� ��� OFF
    private Mode lastMode;

    // ���� ���庰 ���� �÷��� ����
    // R1: �����̴� ��ġ �� (OFF) �� ��ư ������
    private bool r1_Installed, r1_ButtonPressed;

    // R2: ��� ��ġ �� (OFF) �� ��ư ������ �� ������ ������
    private bool r2_Installed, r2_ButtonPressed, r2_DoorExited;

    // R3: X ��ġ �� (OFF) �� ���� ����
    private bool r3_XInstalled, r3_ThornDeleted;

    // ���� ī��Ʈ ������(��ġ/���� ����) ����
    private int lastCount_R1 = -1;  // sliderManager_R1.PlacedInstance.Count
    private int lastCount_R2 = -1;  // toggleManager_R2.PlacedInstance.Count
    private int lastCount_R3 = -1;  // xManager_R3.PlacedInstance.Count

    // Ÿ�Զ����� ����
    private string lastPanel1Rendered = "", lastPanel2Rendered = "", lastPanel3Rendered = "";
    private Coroutine typeCo1, typeCo2, typeCo3;

    // ���� ��ȯ ����
    private string prevRegionId = "";

    private void Awake()
    {
        // ��ư �̺�Ʈ ����(������ �Ϸ� ó��)
        if (buttons_R1 != null)
            foreach (var b in buttons_R1) if (b) b.onPressed.AddListener(() => r1_ButtonPressed = true);

        if (buttons_R2 != null)
            foreach (var b in buttons_R2) if (b) b.onPressed.AddListener(() => r2_ButtonPressed = true);

        lastMode = GameManager.mode;

        if (subPanel) subPanel.SetActive(false);
        if (subPanelText) subPanelText.richText = true;
    }

    private void Update()
    {
        string currentId = LevelManager.Instance.CurrentRegionId;

        // ���� ��ȯ �� ���� �÷���/ī���� ����(�� �������� ������ ó������ ���۵ǵ���)
        if (currentId != prevRegionId)
        {
            ResetProgressForRegion(currentId);
            prevRegionId = currentId;
        }

        TrackEditModeProgress();
        TrackPerRoundPlacementAndDeletion();

        // �г� ��� �� ��Ʈ Ʈ����(���� ����)
        if (currentId == "Region_07")
        {
            if (!panel1.activeInHierarchy) { panel1.SetActive(true); panel2.SetActive(false); panel3.SetActive(false); }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!isHintPlaying && tab1 % 2 == 0) StartCoroutine(PlayHintAnimationOnce());
                tab1++;
            }
        }
        else if (currentId == "Region_08")
        {
            if (!panel2.activeInHierarchy) { panel1.SetActive(false); panel2.SetActive(true); panel3.SetActive(false); }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!isHintPlayingTwo && tab2 % 2 == 0) StartCoroutine(PlayHintAnimationTwo());
                tab2++;
            }
        }
        else if (currentId == "Region_09")
        {
            if (!panel3.activeInHierarchy) { panel1.SetActive(false); panel2.SetActive(false); panel3.SetActive(true); }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!isHintPlayingThree && tab3 % 2 == 0) StartCoroutine(PlayHintAnimationThird());
                tab3++;
            }
        }
        else
        {
            if (panel1) panel1.SetActive(false);
            if (panel2) panel2.SetActive(false);
            if (panel3) panel3.SetActive(false);
        }

        UpdatePanelTextsAndSub(currentId);

        // Ʃ�丮�� �Ϸ� �� ��ü ĵ���� ��Ȱ��ȭ
        if (tutorialCanvasRoot && IsTutorialDone(currentId))
        {
            tutorialCanvasRoot.SetActive(false);
        }

    }

    private void ResetProgressForRegion(string regionId)
    {
        // ���� ���� �÷��� ����
        editEnabledOnce = false;
        editDisabledOnce = false;
        lastMode = GameManager.mode; // ���� ��� �������� ������

        // ī���� ����
        lastCount_R1 = lastCount_R2 = lastCount_R3 = -1;

        // ���庰 ����
        r1_Installed = r1_ButtonPressed = false;
        r2_Installed = r2_ButtonPressed = r2_DoorExited = false;
        r3_XInstalled = r3_ThornDeleted = false;
    }

    // ���������������������������������� ���� ���� ����������������������������������
    private void TrackEditModeProgress()
    {
        if (GameManager.mode != lastMode)
        {
            if (GameManager.mode == Mode.Editing) editEnabledOnce = true;
            if (lastMode == Mode.Editing) editDisabledOnce = true;
            lastMode = GameManager.mode;
        }
    }

    private void TrackPerRoundPlacementAndDeletion()
    {
        // R1 ��ġ(����) ����
        if (sliderManager_R1)
        {
            int c = sliderManager_R1.PlacedInstance != null ? sliderManager_R1.PlacedInstance.Count : 0;
            if (lastCount_R1 < 0) lastCount_R1 = c;
            else { if (c > lastCount_R1) r1_Installed = true; lastCount_R1 = c; }
        }

        // R2 ��ġ(����) ����
        if (toggleManager_R2)
        {
            int c = toggleManager_R2.PlacedInstance != null ? toggleManager_R2.PlacedInstance.Count : 0;
            if (lastCount_R2 < 0) lastCount_R2 = c;
            else { if (c > lastCount_R2) r2_Installed = true; lastCount_R2 = c; }
        }

        // R3: X ��ġ(����) & (��ġ ���� + ���� OFF ���¿���) ����(����) �� ���� ���� �Ϸ�� ����
        if (xManager_R3)
        {
            int c = xManager_R3.PlacedInstance != null ? xManager_R3.PlacedInstance.Count : 0;
            if (lastCount_R3 < 0) lastCount_R3 = c;
            else
            {
                if (c > lastCount_R3) r3_XInstalled = true;
                if (c < lastCount_R3 && r3_XInstalled && editDisabledOnce) r3_ThornDeleted = true;
                lastCount_R3 = c;
            }
        }
    }

    // �ܺο��� ȣ��(�� Ʈ����, ��Ÿ)
    public void NotifyDoorExited() { r2_DoorExited = true; }
    public void NotifyThornDeleted() { r3_ThornDeleted = true; } // �ʿ� �� �ܺ� �̺�Ʈ�ε� �Ϸ� ó��

    // ���������������������������������� �ؽ�Ʈ & �����г� ����������������������������������
    private void UpdatePanelTextsAndSub(string currentRegionId)
    {
        var (currentPrompt, subPrompt) = DetermineCurrentAndSubPrompt(currentRegionId);

        string MakeLine(string label, bool done) => done ? $"<color=#9ED36A>��</color> {label}" : $"�� {label}";

        // Region_07: Tap ON �� (��ġ) �����̴� �� Tap OFF �� ��ư �� �Ϸ�
        string[] p1Lines = new[]
        {
            $"> {currentPrompt}",
            MakeLine("Tap�� ���� ���� ��� Ȱ��ȭ.",   editEnabledOnce),
            MakeLine("�����̴� ��ġ�ϱ�.",            r1_Installed),
            MakeLine("Tap�� ���� ���� ��� ��Ȱ��ȭ.", editDisabledOnce),
            MakeLine("��ư�� ���� ��ġ�� �۵��ϱ�.",   r1_ButtonPressed),
        };

        // Region_08: Tap ON �� (��ġ) ��� �� Tap OFF �� ��ư �� ������ ������ �� �Ϸ�
        string[] p2Lines = new[]
        {
            $"> {currentPrompt}",
            MakeLine("Tap�� ���� ���� ��� Ȱ��ȭ.",   editEnabledOnce),
            MakeLine("��� ��ġ�ϱ�.",                r2_Installed),
            MakeLine("Tap�� ���� ���� ��� ��Ȱ��ȭ.", editDisabledOnce),
            MakeLine("��ư�� ���� ��ġ�� �۵��ϱ�.",   r2_ButtonPressed),
            MakeLine("������ ������.",                r2_DoorExited),
        };

        // Region_09: Tap ON �� (��ġ) X �� Tap OFF �� ���� ���� �� �Ϸ�
        string[] p3Lines = new[]
        {
            $"> {currentPrompt}",
            MakeLine("Tap�� ���� ���� ��� Ȱ��ȭ.",   editEnabledOnce),
            MakeLine("X ��ġ�ϱ�.",                  r3_XInstalled),
            MakeLine("Tap�� ���� ���� ��� ��Ȱ��ȭ.", editDisabledOnce),
            MakeLine("���� �����ϱ�.",                r3_ThornDeleted),
        };

        // ���� ���� Ÿ���� ����
        if (currentRegionId == "Region_07" && panel1Text)
        {
            var text = string.Join("\n", p1Lines);
            StartTypingIfChanged(ref lastPanel1Rendered, text, ref typeCo1, panel1Text);
        }
        else if (panel1Text && string.IsNullOrEmpty(currentRegionId))
        {
            StopTyping(ref typeCo1); panel1Text.text = ""; lastPanel1Rendered = "";
        }

        if (currentRegionId == "Region_08" && panel2Text)
        {
            var text = string.Join("\n", p2Lines);
            StartTypingIfChanged(ref lastPanel2Rendered, text, ref typeCo2, panel2Text);
        }
        else if (panel2Text && string.IsNullOrEmpty(currentRegionId))
        {
            StopTyping(ref typeCo2); panel2Text.text = ""; lastPanel2Rendered = "";
        }

        if (currentRegionId == "Region_09" && panel3Text)
        {
            var text = string.Join("\n", p3Lines);
            StartTypingIfChanged(ref lastPanel3Rendered, text, ref typeCo3, panel3Text);
        }
        else if (panel3Text && string.IsNullOrEmpty(currentRegionId))
        {
            StopTyping(ref typeCo3); panel3Text.text = ""; lastPanel3Rendered = "";
        }

        // Sub �г� (��Ȳ�� ǥ��)
        if (subPanel && subPanelText)
        {
            if (!string.IsNullOrWhiteSpace(subPrompt))
            {
                if (!subPanel.activeSelf) subPanel.SetActive(true);
                subPanelText.text = subPrompt;
            }
            else
            {
                if (subPanel.activeSelf) subPanel.SetActive(false);
                subPanelText.text = "";
            }
        }
    }

    /// Region�� ���� �ܰ��� ���� �� ��ࡱ�� Sub �ȳ��� (���� ����)
    private (string currentPrompt, string subPrompt) DetermineCurrentAndSubPrompt(string regionId)
    {
        // 1) ����: Tap ON
        if (!editEnabledOnce)
            return ("��ġ ���� �����ϼ���.", "<b>Tap</b> : ��� ��ȯ");

        // 2) ��ġ �ܰ� (������ ����)
        if (regionId == "Region_07" && !r1_Installed)
            return ("�����̴� ������ ��ġ�ϼ���.", "<b>���� �̵�</b>: �巡��\n<b>���� ����</b>: ��Ŭ��");

        if (regionId == "Region_08" && !r2_Installed)
            return ("��� ������ ��ġ�ϼ���.", "<b>���� �̵�</b>: �巡��\n<b>���� ����</b>: ��Ŭ��");

        if (regionId == "Region_09" && !r3_XInstalled)
            return ("X ������ ��ġ�ϼ���.", "<b>���� �̵�</b>: �巡��");

        // 3) ����: Tap OFF (��ġ ���Ŀ��� �䱸)
        if (!editDisabledOnce)
            return ("��ġ ��忡�� �����ϼ���.", "<b>Tap</b> : ��� ��ȯ");

        // 4) �۵� �ܰ� (������)
        if (regionId == "Region_07" && !r1_ButtonPressed)
            return ("�����̴��� ����� ��ġ�� �۵��ϼ���.", "");

        if (regionId == "Region_08" && !r2_ButtonPressed)
            return ("����� ����� ��ġ�� �۵��ϼ���.", "");

        if (regionId == "Region_09" && !r3_ThornDeleted)
            return ("���ø� �����ϼ���.", "<b>���� ����</b>: ��Ŭ��");

        // 5) ������ ������(2���������� �䱸)
        if (regionId == "Region_08" && !r2_DoorExited)
            return ("���� ���� ��������.", "");

        // �Ϸ�
        return ("���� ���� ��������.", "");
    }

    // ���������������������������������� Ÿ����(���� ����) ����������������������������������
    private void StartTypingIfChanged(ref string lastRendered, string newContent, ref Coroutine co, TextMeshProUGUI target)
    {
        if (!restartTypingOnChange && newContent == lastRendered) return;
        StopTyping(ref co);
        co = StartCoroutine(TypeLinesCo(newContent, target));
        lastRendered = newContent;
    }

    private void StopTyping(ref Coroutine co)
    {
        if (co != null) { StopCoroutine(co); co = null; }
    }

    private IEnumerator TypeLinesCo(string fullText, TextMeshProUGUI target)
    {
        if (!target) yield break;

        var lines = fullText.Split('\n');
        target.text = "";
        for (int i = 0; i < lines.Length; i++)
        {
            if (i == 0) target.text = lines[0];
            else target.text += "\n" + lines[i];

            if (lineInterval > 0f) yield return new WaitForSeconds(lineInterval);
            else yield return null;
        }
    }

    // ���������������������������������� ��Ʈ �ִϸ��̼� ����������������������������������
    private IEnumerator PlayHintAnimationOnce()
    {
        isHintPlaying = true;
        if (mouseHintAnimator)
        {
            mouseHintAnimator.gameObject.SetActive(true);
            float clipLength = GetAnimationClipLength("Tuto1");
            if (clipLength == 0f) { Debug.LogError("�ִϸ��̼� Ŭ�� 'Tuto1' ����/���� 0"); isHintPlaying = false; yield break; }
            mouseHintAnimator.SetTrigger("DoDrag");
            yield return new WaitForSeconds(clipLength);
            mouseHintAnimator.gameObject.SetActive(false);
        }
        isHintPlaying = false;
    }

    private IEnumerator PlayHintAnimationTwo()
    {
        isHintPlayingTwo = true;
        if (mouseHintAnimator)
        {
            mouseHintAnimator.gameObject.SetActive(true);
            float clipLength = GetAnimationClipLength("Tuto2");
            if (clipLength == 0f) { Debug.LogError("�ִϸ��̼� Ŭ�� 'Tuto2' ����/���� 0"); isHintPlayingTwo = false; yield break; }
            mouseHintAnimator.SetTrigger("DoDrag2");
            yield return new WaitForSeconds(clipLength);
        }
        isHintPlayingTwo = false;
    }

    private IEnumerator PlayHintAnimationThird()
    {
        isHintPlayingThree = true;
        if (mouseHintAnimator)
        {
            mouseHintAnimator.gameObject.SetActive(true);
            float clipLength = GetAnimationClipLength("Tuto3");
            if (clipLength == 0f) { Debug.LogError("�ִϸ��̼� Ŭ�� 'Tuto3' ����/���� 0"); isHintPlayingThree = false; yield break; }
            mouseHintAnimator.SetTrigger("DoDrag3");
            yield return new WaitForSeconds(clipLength);
            mouseHintAnimator.gameObject.SetActive(false);
        }
        isHintPlayingThree = false;
    }

    private float GetAnimationClipLength(string clipName)
    {
        if (!mouseHintAnimator || !mouseHintAnimator.runtimeAnimatorController) return 0f;
        foreach (var clip in mouseHintAnimator.runtimeAnimatorController.animationClips)
            if (clip.name == clipName) return clip.length;
        return 0f;
    }

    private bool IsTutorialDone(string regionId)
    {
        if (regionId == "Region_07")
            return editEnabledOnce && r1_Installed && editDisabledOnce && r1_ButtonPressed;

        if (regionId == "Region_08")
            return editEnabledOnce && r2_Installed && editDisabledOnce && r2_ButtonPressed && r2_DoorExited;

        if (regionId == "Region_09")
            return editEnabledOnce && r3_XInstalled && editDisabledOnce && r3_ThornDeleted;

        return false;
    }

}
