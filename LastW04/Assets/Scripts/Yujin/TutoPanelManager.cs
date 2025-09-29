using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class TutoPanelManager : MonoBehaviour
{
    [Header("튜토리얼 패널")]
    [SerializeField] private GameObject panel1;   // Region_07
    [SerializeField] private GameObject panel2;   // Region_08
    [SerializeField] private GameObject panel3;   // Region_09

    [Header("튜토리얼 텍스트 (TMP)")]
    [SerializeField] private TextMeshProUGUI panel1Text;
    [SerializeField] private TextMeshProUGUI panel2Text;
    [SerializeField] private TextMeshProUGUI panel3Text;

    [Header("추가 조작(Sub) 패널")]
    [SerializeField] private GameObject subPanel;
    [SerializeField] private TextMeshProUGUI subPanelText;

    [Header("힌트 애니메이션")]
    [SerializeField] private Animator mouseHintAnimator;

    [Header("라운드별 설치/삭제 추적 대상 (UIDragManager)")]
    [Tooltip("Region_07: 슬라이더 설치 추적용")]
    [SerializeField] private UIDragManager sliderManager_R1;
    [Tooltip("Region_08: 토글 설치 추적용")]
    [SerializeField] private UIDragManager toggleManager_R2;
    [Tooltip("Region_09: X 설치/삭제 추적용")]
    [SerializeField] private UIDragManager xManager_R3;

    [Header("라운드별 버튼(트리거) 추적 대상 (SimpleButton)")]
    [Tooltip("Region_07: 버튼 누르기 감지용")]
    [SerializeField] private SimpleButton[] buttons_R1;
    [Tooltip("Region_08: 버튼 누르기 감지용")]
    [SerializeField] private SimpleButton[] buttons_R2;

    [Header("타이핑(한 줄씩 표시) 옵션")]
    [SerializeField, Min(0f)] private float lineInterval = 0.35f;
    [SerializeField] private bool restartTypingOnChange = true;

    [Header("튜토리얼 캔버스 루트")]
    [SerializeField] private GameObject tutorialCanvasRoot; // 튜토리얼 전체 캔버스

    // 내부 상태(애니메이션/탭 토글)
    private bool isHintPlaying, isHintPlayingTwo, isHintPlayingThree;
    private int tab1, tab2, tab3;

    // ── 리전별 공통: 편집모드 진입/퇴장 ──
    private bool editEnabledOnce;   // 이 리전에서 Tap으로 편집 모드 ON
    private bool editDisabledOnce;  // 이 리전에서 Tap으로 편집 모드 OFF
    private Mode lastMode;

    // ── 라운드별 진행 플래그 ──
    // R1: 슬라이더 설치 → (OFF) → 버튼 누르기
    private bool r1_Installed, r1_ButtonPressed;

    // R2: 토글 설치 → (OFF) → 버튼 누르기 → 문으로 나가기
    private bool r2_Installed, r2_ButtonPressed, r2_DoorExited;

    // R3: X 설치 → (OFF) → 가시 삭제
    private bool r3_XInstalled, r3_ThornDeleted;

    // ── 카운트 스냅샷(설치/삭제 감지) ──
    private int lastCount_R1 = -1;  // sliderManager_R1.PlacedInstance.Count
    private int lastCount_R2 = -1;  // toggleManager_R2.PlacedInstance.Count
    private int lastCount_R3 = -1;  // xManager_R3.PlacedInstance.Count

    // 타입라이팅 관리
    private string lastPanel1Rendered = "", lastPanel2Rendered = "", lastPanel3Rendered = "";
    private Coroutine typeCo1, typeCo2, typeCo3;

    // 리전 전환 감지
    private string prevRegionId = "";

    private void Awake()
    {
        // 버튼 이벤트 구독(누르기 완료 처리)
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

        // 리전 전환 시 진행 플래그/카운터 리셋(각 리전마다 순서가 처음부터 시작되도록)
        if (currentId != prevRegionId)
        {
            ResetProgressForRegion(currentId);
            prevRegionId = currentId;
        }

        TrackEditModeProgress();
        TrackPerRoundPlacementAndDeletion();

        // 패널 토글 및 힌트 트리거(기존 유지)
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

        // 튜토리얼 완료 시 전체 캔버스 비활성화
        if (tutorialCanvasRoot && IsTutorialDone(currentId))
        {
            tutorialCanvasRoot.SetActive(false);
        }

    }

    private void ResetProgressForRegion(string regionId)
    {
        // 공통 편집 플래그 리셋
        editEnabledOnce = false;
        editDisabledOnce = false;
        lastMode = GameManager.mode; // 현재 모드 기준으로 스냅샷

        // 카운터 리셋
        lastCount_R1 = lastCount_R2 = lastCount_R3 = -1;

        // 라운드별 리셋
        r1_Installed = r1_ButtonPressed = false;
        r2_Installed = r2_ButtonPressed = r2_DoorExited = false;
        r3_XInstalled = r3_ThornDeleted = false;
    }

    // ───────────────── 진행 추적 ─────────────────
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
        // R1 설치(증가) 감지
        if (sliderManager_R1)
        {
            int c = sliderManager_R1.PlacedInstance != null ? sliderManager_R1.PlacedInstance.Count : 0;
            if (lastCount_R1 < 0) lastCount_R1 = c;
            else { if (c > lastCount_R1) r1_Installed = true; lastCount_R1 = c; }
        }

        // R2 설치(증가) 감지
        if (toggleManager_R2)
        {
            int c = toggleManager_R2.PlacedInstance != null ? toggleManager_R2.PlacedInstance.Count : 0;
            if (lastCount_R2 < 0) lastCount_R2 = c;
            else { if (c > lastCount_R2) r2_Installed = true; lastCount_R2 = c; }
        }

        // R3: X 설치(증가) & (설치 이후 + 편집 OFF 상태에서) 삭제(감소) → 가시 삭제 완료로 간주
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

    // 외부에서 호출(문 트리거, 기타)
    public void NotifyDoorExited() { r2_DoorExited = true; }
    public void NotifyThornDeleted() { r3_ThornDeleted = true; } // 필요 시 외부 이벤트로도 완료 처리

    // ───────────────── 텍스트 & 서브패널 ─────────────────
    private void UpdatePanelTextsAndSub(string currentRegionId)
    {
        var (currentPrompt, subPrompt) = DetermineCurrentAndSubPrompt(currentRegionId);

        string MakeLine(string label, bool done) => done ? $"<color=#9ED36A>■</color> {label}" : $"□ {label}";

        // Region_07: Tap ON → (설치) 슬라이더 → Tap OFF → 버튼 → 완료
        string[] p1Lines = new[]
        {
            $"> {currentPrompt}",
            MakeLine("Tap을 눌러 편집 모드 활성화.",   editEnabledOnce),
            MakeLine("슬라이더 설치하기.",            r1_Installed),
            MakeLine("Tap을 눌러 편집 모드 비활성화.", editDisabledOnce),
            MakeLine("버튼을 눌러 장치를 작동하기.",   r1_ButtonPressed),
        };

        // Region_08: Tap ON → (설치) 토글 → Tap OFF → 버튼 → 문으로 나가기 → 완료
        string[] p2Lines = new[]
        {
            $"> {currentPrompt}",
            MakeLine("Tap을 눌러 편집 모드 활성화.",   editEnabledOnce),
            MakeLine("토글 설치하기.",                r2_Installed),
            MakeLine("Tap을 눌러 편집 모드 비활성화.", editDisabledOnce),
            MakeLine("버튼을 눌러 장치를 작동하기.",   r2_ButtonPressed),
            MakeLine("문으로 나가기.",                r2_DoorExited),
        };

        // Region_09: Tap ON → (설치) X → Tap OFF → 가시 삭제 → 완료
        string[] p3Lines = new[]
        {
            $"> {currentPrompt}",
            MakeLine("Tap을 눌러 편집 모드 활성화.",   editEnabledOnce),
            MakeLine("X 설치하기.",                  r3_XInstalled),
            MakeLine("Tap을 눌러 편집 모드 비활성화.", editDisabledOnce),
            MakeLine("가시 삭제하기.",                r3_ThornDeleted),
        };

        // 라인 단위 타이핑 적용
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

        // Sub 패널 (상황별 표시)
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

    /// Region별 현재 단계의 “한 줄 요약”과 Sub 안내문 (순서 강제)
    private (string currentPrompt, string subPrompt) DetermineCurrentAndSubPrompt(string regionId)
    {
        // 1) 진입: Tap ON
        if (!editEnabledOnce)
            return ("배치 모드로 진입하세요.", "<b>Tap</b> : 모드 전환");

        // 2) 설치 단계 (리전별 가젯)
        if (regionId == "Region_07" && !r1_Installed)
            return ("슬라이더 가젯을 설치하세요.", "<b>가젯 이동</b>: 드래그\n<b>가젯 삭제</b>: 우클릭");

        if (regionId == "Region_08" && !r2_Installed)
            return ("토글 가젯을 설치하세요.", "<b>가젯 이동</b>: 드래그\n<b>가젯 삭제</b>: 우클릭");

        if (regionId == "Region_09" && !r3_XInstalled)
            return ("X 가젯을 설치하세요.", "<b>가젯 이동</b>: 드래그");

        // 3) 퇴장: Tap OFF (설치 이후에만 요구)
        if (!editDisabledOnce)
            return ("배치 모드에서 퇴장하세요.", "<b>Tap</b> : 모드 전환");

        // 4) 작동 단계 (리전별)
        if (regionId == "Region_07" && !r1_ButtonPressed)
            return ("슬라이더를 사용해 장치를 작동하세요.", "");

        if (regionId == "Region_08" && !r2_ButtonPressed)
            return ("토글을 사용해 장치를 작동하세요.", "");

        if (regionId == "Region_09" && !r3_ThornDeleted)
            return ("가시를 삭제하세요.", "<b>가젯 삭제</b>: 우클릭");

        // 5) 문으로 나가기(2스테이지만 요구)
        if (regionId == "Region_08" && !r2_DoorExited)
            return ("문을 통해 나가세요.", "");

        // 완료
        return ("문을 통해 나가세요.", "");
    }

    // ───────────────── 타이핑(라인 단위) ─────────────────
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

    // ───────────────── 힌트 애니메이션 ─────────────────
    private IEnumerator PlayHintAnimationOnce()
    {
        isHintPlaying = true;
        if (mouseHintAnimator)
        {
            mouseHintAnimator.gameObject.SetActive(true);
            float clipLength = GetAnimationClipLength("Tuto1");
            if (clipLength == 0f) { Debug.LogError("애니메이션 클립 'Tuto1' 없음/길이 0"); isHintPlaying = false; yield break; }
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
            if (clipLength == 0f) { Debug.LogError("애니메이션 클립 'Tuto2' 없음/길이 0"); isHintPlayingTwo = false; yield break; }
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
            if (clipLength == 0f) { Debug.LogError("애니메이션 클립 'Tuto3' 없음/길이 0"); isHintPlayingThree = false; yield break; }
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
