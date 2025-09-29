using TMPro;
using UnityEngine;
using System;

public class TutoPanelManager : MonoBehaviour
{
    [Header("튜토리얼 패널 (리전별로 하나만 표시)")]
    [SerializeField] private GameObject panel1;    // Region_07
    [SerializeField] private GameObject panel2;    // Region_08
    [SerializeField] private GameObject panel3;    // Region_09

    [Header("튜토리얼 텍스트 (TMP)")]
    [SerializeField] private TextMeshProUGUI panel1Text;
    [SerializeField] private TextMeshProUGUI panel2Text;
    [SerializeField] private TextMeshProUGUI panel3Text;

    [Header("추가 조작(Sub) 패널")]
    [SerializeField] private GameObject subPanel;
    [SerializeField] private TextMeshProUGUI subPanelText;

    [Header("힌트 애니메이션 (옵션)")]
    [SerializeField] private Animator mouseHintAnimator;

    [Header("라운드별 설치/삭제 추적 대상 (UIDragManager)")]
    [SerializeField] private UIDragManager sliderManager_R1; // Region_07
    [SerializeField] private UIDragManager toggleManager_R2; // Region_08
    [SerializeField] private UIDragManager xManager_R3;      // Region_09

    [Header("라운드별 버튼(트리거) 추적 대상 (SimpleButton)")]
    [SerializeField] private SimpleButton[] buttons_R1; // R1
    [SerializeField] private SimpleButton[] buttons_R2; // R2

    [Header("튜토리얼 캔버스 루트(전체 비/파괴 용도)")]
    [SerializeField] private GameObject tutorialCanvasRoot;

    [Header("디버그/테스트")]
    [Tooltip("씬 시작 시, 저장된 튜토리얼 완료 플래그를 전부 초기화합니다. (PlayerPrefs)")]
    [SerializeField] private bool resetProgressOnStart = false;

    // 내부 상태
    private Mode lastMode;
    private bool r1_Installed, r1_ButtonPressed; private int lastCount_R1 = 0;
    private bool r2_Installed, r2_ButtonPressed, r2_DoorExited; private int lastCount_R2 = 0;
    private bool r3_XInstalled, r3_ThornDeleted; private int lastCount_R3 = 0;
    private bool editEnabledOnce, editDisabledOnce;
    private bool r1AllDone, r2AllDone, r3AllDone;
    private const string PP_R1_DONE = "Tut_R1_Done";
    private const string PP_R2_DONE = "Tut_R2_Done";
    private const string PP_R3_DONE = "Tut_R3_Done";
    private string prevRegionId = "";

    // **[강화된 로직]** 시작 시 R1 튜토리얼 강제 표시 플래그
    private bool forceR1StartOnce = true;

    private void Awake()
    {
        Debug.Log("=====================================================");
        Debug.Log($"[TutoPanelManager: Awake] 1. Starting Awake. resetProgressOnStart: {resetProgressOnStart}");

        // PlayerPrefs 초기화 로직 (강력하게 적용)
        if (resetProgressOnStart)
        {
            PlayerPrefs.DeleteKey(PP_R1_DONE);
            PlayerPrefs.DeleteKey(PP_R2_DONE);
            PlayerPrefs.DeleteKey(PP_R3_DONE);
            PlayerPrefs.Save();
            Debug.LogWarning("[TutoPanelManager: Awake] 2. PlayerPrefs CLEARED by resetProgressOnStart flag.");
        }

        // 버튼 완료 구독 (생략)
        if (buttons_R1 != null) foreach (var b in buttons_R1) if (b) b.onPressed.AddListener(() => r1_ButtonPressed = true);
        if (buttons_R2 != null) foreach (var b in buttons_R2) if (b) b.onPressed.AddListener(() => r2_ButtonPressed = true);

        lastMode = GameManager.mode;

        if (subPanel) subPanel.SetActive(false);
        if (subPanelText) subPanelText.richText = true;

        if (!tutorialCanvasRoot)
        {
            var canvas = GetComponentInParent<Canvas>();
            if (canvas) tutorialCanvasRoot = canvas.gameObject;
        }

        // 완료 상태 로드
        r1AllDone = PlayerPrefs.GetInt(PP_R1_DONE, 0) == 1;
        r2AllDone = PlayerPrefs.GetInt(PP_R2_DONE, 0) == 1;
        r3AllDone = PlayerPrefs.GetInt(PP_R3_DONE, 0) == 1;
        Debug.Log($"[TutoPanelManager: Awake] 3. Loaded PP State: R1={r1AllDone}, R2={r2AllDone}, R3={r3AllDone}");

        // 모든 튜토리얼 완료 시 제거 (R1~R3 모두 완료된 경우만)
        if (r1AllDone && r2AllDone && r3AllDone)
        {
            Debug.LogError("[TutoPanelManager: Awake] 4. ALL TUTORIALS GLOBALLY DONE. DESTROYING IMMEDIATELY.");
            DestroyTutorialObjectsImmediate();
            return;
        }

        // 튜토리얼 상태 초기화
        ResetProgressForRegion("INIT");
        Debug.Log("[TutoPanelManager: Awake] 5. Finishing Awake.");
    }

    private void Start()
    {
        // Start에서는 R1 패널을 켜고 초기 메시지를 설정만 함.
        Debug.Log("[TutoPanelManager: Start] 1. Setting initial UI for Region_07.");
        SetRegionText("Region_07", "> 배치 모드로 진입하세요.");
        SetPanelActive(panel1, true, "Start(): force Panel1 ON");
        SetPanelActive(panel2, false, "Start(): Panel2 OFF");
        SetPanelActive(panel3, false, "Start(): Panel3 OFF");
    }

    private void OnDisable() => HideSubPanel();

    private void Update()
    {
        string currentId = LevelManager.Instance ? LevelManager.Instance.CurrentRegionId : "N/A";

        Debug.Log($"-----------------------------------------------------");
        Debug.Log($"[TutoPanelManager: Update] 1. Current Region: '{currentId}'");

        // =========================================================================
        // **[강화된 시작 로직]**: 씬 시작 후 첫 번째 유효한 Update 루프에서 R1을 강제 시작시킴
        // =========================================================================
        if (forceR1StartOnce)
        {
            // R1이 이미 영구 완료된 경우를 제외하고 R1 강제 시작
            if (currentId == "Region_07" && !r1AllDone)
            {
                Debug.LogWarning("[TutoPanelManager: Update] FORCE R1 START LOGIC ACTIVATED. Bypassing global check.");
                // 상태 초기화 및 R1 텍스트 설정은 Start와 ResetProgress에서 이미 처리됨
            }
            // 첫 프레임 이후에는 플래그 해제
            forceR1StartOnce = false;
        }

        // 1) 허용 리전이 아니거나 글로벌 완료 시 숨김 (R1 완료 여부는 여기서 체크하지 않음)
        if (!IsSupportedRegion(currentId))
        {
            Debug.Log("[TutoPanelManager: Update] Region Unsupported, returning.");
            ShowOnlyRegion(null);
            HideSubPanel();
            prevRegionId = currentId;
            return;
        }

        // **[디버그]** 글로벌 완료 체크 (R1은 제외, R2, R3 완료 여부만 체크)
        bool globallyDone = IsRegionGloballyDone(currentId) && currentId != "Region_07";
        Debug.Log($"[TutoPanelManager: Update] 2. Checking Global Done. Result for '{currentId}': {globallyDone} (R1 check excluded)");

        if (globallyDone)
        {
            Debug.Log("[TutoPanelManager: Update] 3. Global Done detected, showing completion message.");
            ShowOnlyRegion(currentId);
            UpdateOneLineAndSub(currentId);
            HideSubPanel();
            prevRegionId = currentId;
            return;
        }

        // 2) 리전 변경 처리
        if (currentId != prevRegionId && !string.IsNullOrEmpty(currentId))
        {
            Debug.Log($"[TutoPanelManager: Update] 4. Region changed from '{prevRegionId}' to '{currentId}'. Resetting progress.");
            // R1에서 R2로 이동 시 R1의 완료 상태는 이미 PP에 저장되어 있어야 함.
            ResetProgressForRegion(currentId);
            prevRegionId = currentId;
        }

        // 3) 진행 추적 & UI 갱신
        TrackEditMode();
        TrackPlacementAndDeletion();
        ShowOnlyRegion(currentId);

        // **[디버그]** 상세 플래그 로깅
        string flags = $"Edit:{editEnabledOnce}/{editDisabledOnce} | R1:{r1_Installed}/{r1_ButtonPressed} | R2:{r2_Installed}/{r2_ButtonPressed}/{r2_DoorExited} | R3:{r3_XInstalled}/{r3_ThornDeleted}";
        Debug.Log($"[TutoPanelManager: Update] 5. Current Progress Flags: {flags}");

        UpdateOneLineAndSub(currentId);

        // 4) 완료 저장
        bool regionJustDone = IsRegionDone(currentId);
        Debug.Log($"[TutoPanelManager: Update] 6. Checking Current Region Done. Result: {regionJustDone}");

        if (!string.IsNullOrEmpty(currentId) && regionJustDone)
        {
            // PlayerPrefs에 완료 상태 저장
            if (currentId == "Region_07" && !r1AllDone) { r1AllDone = true; PlayerPrefs.SetInt(PP_R1_DONE, 1); PlayerPrefs.Save(); Debug.LogWarning("[TutoPanelManager: Update] R1 Completed and SAVED."); }
            if (currentId == "Region_08" && !r2AllDone) { r2AllDone = true; PlayerPrefs.SetInt(PP_R2_DONE, 1); PlayerPrefs.Save(); Debug.LogWarning("[TutoPanelManager: Update] R2 Completed and SAVED."); }
            if (currentId == "Region_09" && !r3AllDone) { r3AllDone = true; PlayerPrefs.SetInt(PP_R3_DONE, 1); PlayerPrefs.Save(); Debug.LogWarning("[TutoPanelManager: Update] R3 Completed and SAVED."); }
        }

        if (r1AllDone && r2AllDone && r3AllDone)
        {
            Debug.LogError("[TutoPanelManager: Update] 7. ALL TUTORIALS FINALLY DONE. DESTROYING.");
            DestroyTutorialObjects();
        }
    }

    // ===== 진행 상태/카운트 =====
    private void ResetProgressForRegion(string regionId)
    {
        // 리전 변경 시, 해당 리전의 진행 플래그 초기화
        editEnabledOnce = editDisabledOnce = false;
        lastMode = GameManager.mode;

        lastCount_R1 = 0;
        lastCount_R2 = 0;
        lastCount_R3 = 0;

        r1_Installed = r1_ButtonPressed = false;
        r2_Installed = r2_ButtonPressed = r2_DoorExited = false;
        r3_XInstalled = r3_ThornDeleted = false;

        Debug.Log($"[TutoPanelManager] ResetProgressForRegion('{regionId}') - All flags set to false/0.");
    }

    private void TrackEditMode()
    {
        if (GameManager.mode != lastMode)
        {
            Debug.Log($"[TutoPanelManager: TrackEditMode] Mode changed: {lastMode} -> {GameManager.mode}");
            if (GameManager.mode == Mode.Editing)
            {
                editEnabledOnce = true;
            }
            if (lastMode == Mode.Editing)
            {
                editDisabledOnce = true;
            }
            lastMode = GameManager.mode;
        }
    }

    private void TrackPlacementAndDeletion()
    {
        if (sliderManager_R1)
        {
            int c = sliderManager_R1.PlacedInstance != null ? sliderManager_R1.PlacedInstance.Count : 0;
            if (c > 0) r1_Installed = true;
            lastCount_R1 = c;
        }

        if (toggleManager_R2)
        {
            int c = toggleManager_R2.PlacedInstance != null ? toggleManager_R2.PlacedInstance.Count : 0;
            if (c > 0) r2_Installed = true;
            lastCount_R2 = c;
        }

        if (xManager_R3)
        {
            int c = xManager_R3.PlacedInstance != null ? xManager_R3.PlacedInstance.Count : 0;
            if (c > 0) r3_XInstalled = true;
            lastCount_R3 = c;
        }
    }

    public void NotifyDoorExited()
    {
        r2_DoorExited = true;
        Debug.LogWarning("[TutoPanelManager] NOTIFY: Door Exited (r2_DoorExited = true)");
    }
    public void NotifyThornDeleted()
    {
        r3_ThornDeleted = true;
        Debug.LogWarning("[TutoPanelManager] NOTIFY: Thorn Deleted (r3_ThornDeleted = true)");
    }

    // ===== 표시/문구 =====
    private void ShowOnlyRegion(string currentIdOrNull)
    {
        bool p1 = currentIdOrNull == "Region_07";
        bool p2 = currentIdOrNull == "Region_08" && !r2AllDone; // R2가 영구 완료되지 않은 경우에만 표시
        bool p3 = currentIdOrNull == "Region_09" && !r3AllDone; // R3가 영구 완료되지 않은 경우에만 표시

        SetPanelActive(panel1, p1, $"ShowOnlyRegion('{currentIdOrNull}'): Panel1 {(p1 ? "ON" : "OFF")}");
        SetPanelActive(panel2, p2, $"ShowOnlyRegion('{currentIdOrNull}'): Panel2 {(p2 ? "ON" : "OFF")}");
        SetPanelActive(panel3, p3, $"ShowOnlyRegion('{currentIdOrNull}'): Panel3 {(p3 ? "ON" : "OFF")}");
    }

    private void UpdateOneLineAndSub(string currentRegionId)
    {
        if (!IsSupportedRegion(currentRegionId))
        {
            ShowOnlyRegion(null);
            HideSubPanel();
            return;
        }

        // 1. 완료 상태 체크 (글로벌 완료 상태는 Update 상단에서 이미 처리됨)
        if (IsRegionDone(currentRegionId))
        {
            SetRegionText(currentRegionId, "> 튜토리얼 완료");
            HideSubPanel();
            return;
        }

        // 1-A. R2, R3가 영구 완료된 경우 완료 메시지를 띄워야 함 (GlobalDone에서 처리 안된 경우)
        if (currentRegionId == "Region_08" && r2AllDone || currentRegionId == "Region_09" && r3AllDone)
        {
            SetRegionText(currentRegionId, "> 튜토리얼 완료");
            HideSubPanel();
            return;
        }

        // 2. 진행 단계 메시지 결정 및 표시
        var (line, sub) = DetermineOneLineAndSub(currentRegionId);
        SetRegionText(currentRegionId, $"> {line}");

        // 3. SubPanel 표시/숨김
        if (subPanel && subPanelText)
        {
            if (!string.IsNullOrWhiteSpace(sub))
            {
                if (!subPanel.activeSelf)
                {
                    subPanel.SetActive(true);
                    Debug.Log("[TutoPanelManager] SubPanel ON");
                }
                subPanelText.text = sub;
            }
            else HideSubPanel();
        }
    }

    private void SetRegionText(string regionId, string text)
    {
        if (regionId == "Region_07") { if (panel1Text) panel1Text.text = text; }
        else if (regionId == "Region_08") { if (panel2Text) panel2Text.text = text; }
        else if (regionId == "Region_09") { if (panel3Text) panel3Text.text = text; }
    }

    private (string line, string sub) DetermineOneLineAndSub(string regionId)
    {
        if (!editEnabledOnce)
            return ("배치 모드로 진입하세요.", "<b>Tap</b> : 모드 전환");

        if (regionId == "Region_07")
        {
            if (!r1_Installed)
                return ("슬라이더 가젯을 설치하세요.", "<b>가젯 이동</b>: 드래그\n<b>가젯 삭제</b>: 우클릭");

            if (!editDisabledOnce)
                return ("배치 모드에서 퇴장하세요.", "<b>Tap</b> : 모드 전환");

            if (!r1_ButtonPressed)
                return ("버튼을 눌러 문을 여세요.", "");
        }

        if (regionId == "Region_08")
        {
            if (!r2_Installed)
                return ("토글 가젯을 설치하세요.", "<b>가젯 이동</b>: 드래그\n<b>가젯 삭제</b>: 우클릭");

            if (!editDisabledOnce)
                return ("배치 모드에서 퇴장하세요.", "<b>Tap</b> : 모드 전환");

            if (!r2_ButtonPressed)
                return ("토글을 눌러 문을 여세요.", "");

            if (!r2_DoorExited)
                return ("문을 통해 나가세요.", "");
        }

        if (regionId == "Region_09")
        {
            if (!r3_XInstalled)
                return ("X 가젯을 설치하세요.", "<b>가젯 이동</b>: 드래그");

            if (!editDisabledOnce)
                return ("배치 모드에서 퇴장하세요.", "<b>Tap</b> : 모드 전환");

            if (!r3_ThornDeleted)
                return ("문을 통해 나가세요.", "");
        }

        return ("튜토리얼 진행 중…", "");
    }

    private bool IsRegionDone(string regionId)
    {
        if (regionId == "Region_07")
            return editEnabledOnce && r1_Installed && editDisabledOnce && r1_ButtonPressed;

        if (regionId == "Region_08")
            return editEnabledOnce && r2_Installed && editDisabledOnce && r2_ButtonPressed && r2_DoorExited;

        if (regionId == "Region_09")
            return editEnabledOnce && r3_XInstalled && editDisabledOnce && r3_ThornDeleted;

        return false;
    }

    private bool IsRegionGloballyDone(string regionId)
    {
        // R1이 이미 완료되었더라도, R2나 R3에서 튜토리얼을 진행해야 할 수 있으므로
        // R2, R3에 대해서만 글로벌 완료 여부를 엄격하게 체크
        if (regionId == "Region_07") return false; // R1은 항상 진행 (r1AllDone이 true라도)

        return (regionId == "Region_08" && r2AllDone) ||
               (regionId == "Region_09" && r3AllDone);
    }

    private bool IsSupportedRegion(string regionId)
    {
        return regionId == "Region_07" || regionId == "Region_08" || regionId == "Region_09";
    }

    // --- (HideSubPanel, Destroy... 메소드는 동일) ---

    private void HideSubPanel()
    {
        if (subPanelText) subPanelText.text = "";
        if (subPanel && subPanel.activeSelf)
        {
            subPanel.SetActive(false);
            Debug.Log("[TutoPanelManager] SubPanel OFF");
        }
    }

    private void DestroyTutorialObjects()
    {
        HideSubPanel();
        if (subPanel) Destroy(subPanel);
        if (tutorialCanvasRoot) Destroy(tutorialCanvasRoot);
        else Destroy(gameObject);
        enabled = false;
        Debug.Log("[TutoPanelManager] DestroyTutorialObjects()");
    }

    private void DestroyTutorialObjectsImmediate()
    {
        HideSubPanel();
        if (subPanel) DestroyImmediate(subPanel);
        if (tutorialCanvasRoot) DestroyImmediate(tutorialCanvasRoot);
        else DestroyImmediate(gameObject);
        enabled = false;
        Debug.Log("[TutoPanelManager] DestroyTutorialObjectsImmediate()");
    }

    public void NotifyDoorExitedExtern() => NotifyDoorExited();
    public void NotifyThornDeletedExtern() => NotifyThornDeleted();

    private void SetPanelActive(GameObject go, bool active, string reason)
    {
        if (!go) return;
        if (go.activeSelf == active) return;
        go.SetActive(active);
        Debug.Log($"[TutoPanelManager] {(active ? "ON " : "OFF")} -> {go.name} | {reason}");
    }
}