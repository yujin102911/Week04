using TMPro;
using UnityEngine;

public class TutoPanelManager : MonoBehaviour
{
    [Header("튜토리얼 패널 (리전별로 하나만 표시)")]
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
    [Tooltip("씬 시작 시, 저장된 튜토리얼 완료 플래그를 전부 초기화합니다.")]
    [SerializeField] private bool resetProgressOnStart = false;

    // 내부 상태
    private Mode lastMode;

    // R1: 슬라이더 설치 → (OFF) → 버튼
    private bool r1_Installed, r1_ButtonPressed; private int lastCount_R1 = -1;

    // R2: 토글 설치 → (OFF) → 버튼 → 문 나가기
    private bool r2_Installed, r2_ButtonPressed, r2_DoorExited; private int lastCount_R2 = -1;

    // R3: X 설치 → (OFF) → 가시 삭제
    private bool r3_XInstalled, r3_ThornDeleted; private int lastCount_R3 = -1;

    // 편집 on/off (리전 단위)
    private bool editEnabledOnce, editDisabledOnce;

    // 완료 영구 저장
    private bool r1AllDone, r2AllDone, r3AllDone;
    private const string PP_R1_DONE = "Tut_R1_Done";
    private const string PP_R2_DONE = "Tut_R2_Done";
    private const string PP_R3_DONE = "Tut_R3_Done";

    // 리전 전환 감지
    private string prevRegionId = "";

    // 부팅 1회: Panel1 강제 노출용
    private bool forcePanel1Once = true;

    private void Awake()
    {
        if (resetProgressOnStart)
        {
            PlayerPrefs.DeleteKey(PP_R1_DONE);
            PlayerPrefs.DeleteKey(PP_R2_DONE);
            PlayerPrefs.DeleteKey(PP_R3_DONE);
            PlayerPrefs.Save();
        }

        // 버튼 완료 구독
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

        // 모든 튜토리얼 완료 시 제거
        if (r1AllDone && r2AllDone && r3AllDone)
        {
            DestroyTutorialObjectsImmediate();
            return;
        }
    }

    private void Start()
    {
        // 부팅 직후엔 리전/완료 여부 무시하고 Panel1만 켠다
        SetPanelActive(panel1, true, "Start(): force Panel1 ON");
        SetPanelActive(panel2, false, "Start(): Panel2 OFF");
        SetPanelActive(panel3, false, "Start(): Panel3 OFF");

        // 첫 문구(Region_07 기준) 바로 표기
        SetRegionText("Region_07", "> 배치 모드로 진입하세요.");
    }

    private void OnDisable() => HideSubPanel();

    private void Update()
    {
        string currentId = LevelManager.Instance ? LevelManager.Instance.CurrentRegionId : "";
        Debug.Log($"[TutoPanelManager] Current Region detected: '{currentId}'");

        // 1) 부팅 1프레임: Panel1 유지 후 바로 반환(ShowOnlyRegion으로 끄지 않도록)
        if (forcePanel1Once)
        {
            SetPanelActive(panel1, true, "forcePanel1Once: keep Panel1 regardless of region/completion");
            SetPanelActive(panel2, false, "forcePanel1Once: Panel2 OFF");
            SetPanelActive(panel3, false, "forcePanel1Once: Panel3 OFF");

            // 시작 리전 스냅샷 및 진행 리셋(있다면)
            if (!string.IsNullOrEmpty(currentId))
            {
                ResetProgressForRegion(currentId);
                prevRegionId = currentId;
            }
            forcePanel1Once = false;
            return; // 다음 프레임부터 일반 규칙
        }

        // 2) 허용 리전이 아니면 모든 튜토리얼 UI 숨김
        if (!IsSupportedRegion(currentId))
        {
            ShowOnlyRegion(null);
            HideSubPanel();
            prevRegionId = currentId;
            Debug.Log("[TutoPanelManager] Unsupported region -> hide all panels");
            return;
        }

        // 3) 리전 변경 처리
        if (currentId != prevRegionId && !string.IsNullOrEmpty(currentId))
        {
            if (IsRegionGloballyDone(currentId))
            {
                ShowOnlyRegion(currentId);
                HideSubPanel();
                prevRegionId = currentId;
                return;
            }
            ResetProgressForRegion(currentId);
            prevRegionId = currentId;
        }

        // 4) 진행 추적 & UI 갱신
        TrackEditMode();
        TrackPlacementAndDeletion();

        ShowOnlyRegion(currentId);
        UpdateOneLineAndSub(currentId);

        // 5) 완료 저장
        if (!string.IsNullOrEmpty(currentId) && IsRegionDone(currentId))
        {
            if (currentId == "Region_07" && !r1AllDone) { r1AllDone = true; PlayerPrefs.SetInt(PP_R1_DONE, 1); PlayerPrefs.Save(); }
            if (currentId == "Region_08" && !r2AllDone) { r2AllDone = true; PlayerPrefs.SetInt(PP_R2_DONE, 1); PlayerPrefs.Save(); }
            if (currentId == "Region_09" && !r3AllDone) { r3AllDone = true; PlayerPrefs.SetInt(PP_R3_DONE, 1); PlayerPrefs.Save(); }
            HideSubPanel();
        }

        if (r1AllDone && r2AllDone && r3AllDone)
            DestroyTutorialObjects();
    }

    // ===== 진행 상태/카운트 =====
    private void ResetProgressForRegion(string regionId)
    {
        editEnabledOnce = editDisabledOnce = false;
        lastMode = GameManager.mode;

        lastCount_R1 = lastCount_R2 = lastCount_R3 = -1;

        r1_Installed = r1_ButtonPressed = false;
        r2_Installed = r2_ButtonPressed = r2_DoorExited = false;
        r3_XInstalled = r3_ThornDeleted = false;

        Debug.Log($"[TutoPanelManager] ResetProgressForRegion('{regionId}')");
    }

    private void TrackEditMode()
    {
        if (GameManager.mode != lastMode)
        {
            if (GameManager.mode == Mode.Editing) editEnabledOnce = true;
            if (lastMode == Mode.Editing) editDisabledOnce = true;
            lastMode = GameManager.mode;
        }
    }

    private void TrackPlacementAndDeletion()
    {
        if (sliderManager_R1)
        {
            int c = sliderManager_R1.PlacedInstance != null ? sliderManager_R1.PlacedInstance.Count : 0;
            if (lastCount_R1 < 0) lastCount_R1 = c;
            else { if (c > lastCount_R1) r1_Installed = true; lastCount_R1 = c; }
        }

        if (toggleManager_R2)
        {
            int c = toggleManager_R2.PlacedInstance != null ? toggleManager_R2.PlacedInstance.Count : 0;
            if (lastCount_R2 < 0) lastCount_R2 = c;
            else { if (c > lastCount_R2) r2_Installed = true; lastCount_R2 = c; }
        }

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

    public void NotifyDoorExited() => r2_DoorExited = true;
    public void NotifyThornDeleted() => r3_ThornDeleted = true;

    // ===== 표시/문구 =====
    private void ShowOnlyRegion(string currentIdOrNull)
    {
        bool p1 = currentIdOrNull == "Region_07";
        bool p2 = currentIdOrNull == "Region_08" && !r2AllDone;
        bool p3 = currentIdOrNull == "Region_09" && !r3AllDone;

        SetPanelActive(panel1, p1, $"ShowOnlyRegion('{currentIdOrNull}'): Panel1 {(p1 ? "ON" : "OFF")}");
        SetPanelActive(panel2, p2, $"ShowOnlyRegion('{currentIdOrNull}'): Panel2 {(p2 ? "ON" : "OFF")}");
        SetPanelActive(panel3, p3, $"ShowOnlyRegion('{currentIdOrNull}'): Panel3 {(p3 ? "ON" : "OFF")}");

        if (!IsSupportedRegion(currentIdOrNull))
        {
            if (panel1Text) panel1Text.text = "";
            if (panel2Text) panel2Text.text = "";
            if (panel3Text) panel3Text.text = "";
        }
    }

    private void UpdateOneLineAndSub(string currentRegionId)
    {
        if (!IsSupportedRegion(currentRegionId))
        {
            ShowOnlyRegion(null);
            HideSubPanel();
            return;
        }

        if (IsRegionGloballyDone(currentRegionId) || IsRegionDone(currentRegionId))
        {
            SetRegionText(currentRegionId, "> 튜토리얼 완료");
            HideSubPanel();
            return;
        }

        var (line, sub) = DetermineOneLineAndSub(currentRegionId);
        SetRegionText(currentRegionId, $"> {line}");

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
        if (regionId == "Region_07")
        {
            if (panel1Text) panel1Text.text = text;
            if (panel2Text) panel2Text.text = "";
            if (panel3Text) panel3Text.text = "";
        }
        else if (regionId == "Region_08")
        {
            if (panel1Text) panel1Text.text = "";
            if (panel2Text) panel2Text.text = text;
            if (panel3Text) panel3Text.text = "";
        }
        else if (regionId == "Region_09")
        {
            if (panel1Text) panel1Text.text = "";
            if (panel2Text) panel2Text.text = "";
            if (panel3Text) panel3Text.text = text;
        }
        else
        {
            if (panel1Text) panel1Text.text = "";
            if (panel2Text) panel2Text.text = "";
            if (panel3Text) panel3Text.text = "";
        }
    }

    private (string line, string sub) DetermineOneLineAndSub(string regionId)
    {
        if (!editEnabledOnce)
            return ("배치 모드로 진입하세요.", "<b>Tap</b> : 모드 전환");

        if (regionId == "Region_07" && !r1_Installed)
            return ("슬라이더 가젯을 설치하세요.", "<b>가젯 이동</b>: 드래그\n<b>가젯 삭제</b>: 우클릭");

        if (regionId == "Region_08" && !r2_Installed)
            return ("토글 가젯을 설치하세요.", "<b>가젯 이동</b>: 드래그\n<b>가젯 삭제</b>: 우클릭");

        if (regionId == "Region_09" && !r3_XInstalled)
            return ("X 가젯을 설치하세요.", "<b>가젯 이동</b>: 드래그");

        if (!editDisabledOnce)
            return ("배치 모드에서 퇴장하세요.", "<b>Tap</b> : 모드 전환");

        if (regionId == "Region_07" && !r1_ButtonPressed)
            return ("버튼을 눌러 문을 여세요.", "");

        if (regionId == "Region_08")
        {
            if (!r2_ButtonPressed) return ("토글을 눌러 문을 여세요.", "");
            if (!r2_DoorExited) return ("문을 통해 나가세요.", "");
        }

        if (regionId == "Region_09" && !r3_ThornDeleted)
            return ("문을 통해 나가세요.", "");

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
        return (regionId == "Region_07" && r1AllDone) ||
               (regionId == "Region_08" && r2AllDone) ||
               (regionId == "Region_09" && r3AllDone);
    }

    private bool IsSupportedRegion(string regionId)
    {
        return regionId == "Region_07" || regionId == "Region_08" || regionId == "Region_09";
    }

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

    // 외부 이벤트 바인딩용 public 메소드(필요시)
    public void NotifyDoorExitedExtern() => NotifyDoorExited();
    public void NotifyThornDeletedExtern() => NotifyThornDeleted();

    // 공통: 패널 ON/OFF + 로그
    private void SetPanelActive(GameObject go, bool active, string reason)
    {
        if (!go) return;
        if (go.activeSelf == active) return;
        go.SetActive(active);
        Debug.Log($"[TutoPanelManager] {(active ? "ON " : "OFF")} -> {go.name} | {reason}");
    }
}
