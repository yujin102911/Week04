using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class LevelManager : MonoBehaviour
{
    // ── Singleton (영속)
    public static LevelManager Instance { get; private set; }

    [Header("Scene Refs")]
    [SerializeField] private Transform player; // 리로드 후 재바인딩됨

    private CameraRegion2D[] regions;
    public string CurrentRegionId { get; [SerializeField] private set; }//현재 레벨
    public string CurrentRegionIdCheck;    
    public int levelBefore;//이전레벨
    public int levelCurrent;
    public bool levelChanged;
    public UIDragManager SliderUI;
    public int[] levelUISlider;
    public UIDragManager toggleUI;
    public int[] levelUIToggle;
    public UIDragManager deleteUI;
    public int[] levelUIDelete;



    [Header("Teleport Safety")]
    [SerializeField, Min(0f)] private float postTeleportImmunity = 0.3f;
    private float teleportImmunityUntil = -1f;
    public bool IsTeleportImmune => Time.time < teleportImmunityUntil;

    private void Awake()
    {
        // 싱글톤 보장 + 영속
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 씬 로드 콜백 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // 첫 부팅(부트 씬 포함)에서도 1회 세팅
        CacheSceneObjects();
        // CurrentRegionId가 비었으면 폴백(예: Region_01)
        if (string.IsNullOrEmpty(CurrentRegionId))
        {
            var first = FindRegion("Region_01");
            if (first)
            {
                CurrentRegionId = first.regionId;
                ApplyCameraFrame(first, instant: true);
            }
        }
        else
        {
            // 부트씬에서 넘어온 경우, 이미 가지고 있던 Region으로 즉시 워프
            var region = FindRegion(CurrentRegionId);
            if (region) TeleportToRegion(CurrentRegionId, true, instantCamera: true);
        }
    }

    private void Update()//레벨 변경 확인
    {
        CurrentRegionIdCheck = CurrentRegionId;
        // 새 Input System
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            HardResetScene();

        levelCurrent = int.Parse(new string(CurrentRegionId.Where(char.IsDigit).ToArray()));//텍스트에서 레벨 추출
        if (levelBefore == levelCurrent)//이전 렙이랑 레벨같음?
        {
            levelChanged=false;//렙 안바낌
        }
        else
        {
            levelChanged = true;//렙 바낌
        }                
        if (levelChanged)//렙 바뀌면
        {
            Debug.Log("레벨바뀜");
            SliderUI.limit = levelUISlider[levelCurrent];//제한 수 갱신
            toggleUI.limit = levelUIToggle[levelCurrent];
            deleteUI.limit = levelUIDelete[levelCurrent];
            SliderUI.PlacedInstance.RemoveAll(obj => true);//이전렙 배치된거 제거
            toggleUI.PlacedInstance.RemoveAll(obj => true);
            deleteUI.PlacedInstance.RemoveAll(obj => true);
        }
        levelBefore= levelCurrent;
    }

    // ── 씬 로드가 끝난 후, 모든 참조 재바인딩 + 현재 지역으로 복구
    private void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {
        CacheSceneObjects();

        if (!string.IsNullOrEmpty(CurrentRegionId))
        {
            var region = FindRegion(CurrentRegionId);
            if (region)
            {
                // 로딩 후 첫 프레임은 즉시 스냅 권장
                TeleportToRegion(CurrentRegionId, affectCamera: true, instantCamera: true);
                return;
            }
        }

        // 현재 지역을 못 찾으면 폴백
        var first = FindRegion("Region_01");
        if (first)
        {
            CurrentRegionId = first.regionId;
            ApplyCameraFrame(first, instant: true);
        }
    }

    private void CacheSceneObjects()
    {
        // Region 재수집(비활성 포함)
        regions = UnityEngine.Object.FindObjectsByType<CameraRegion2D>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);

        // 플레이어 재바인딩(인스펙터 설정이 씬 리로드로 끊어질 수 있음)
        if (!player)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go) player = go.transform;
            // 태그가 없다면 여기서 다른 방식으로 찾아도 됨(이름/컴포넌트 등)
        }
    }

    private CameraRegion2D FindRegion(string regionId)
    {
        if (string.IsNullOrEmpty(regionId) || regions == null) return null;
        return regions.FirstOrDefault(r => r && r.regionId == regionId);
    }

    public void TeleportToRegion(string regionId, bool affectCamera = true, bool instantCamera = false)
    {
        var region = FindRegion(regionId);
        if (!region || !player) return;

        var spawn = region.defaultSpawn ? region.defaultSpawn : region.transform;

        // 이동 준비
        var rb = player.GetComponent<Rigidbody2D>();
        if (rb)
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = Vector2.zero;
#else
            rb.velocity = Vector2.zero;
#endif
            rb.angularVelocity = 0f;
        }

        // 위치 스냅
        player.position = spawn.position;
        Physics2D.SyncTransforms();
        if (rb) rb.WakeUp();

        // 상태 갱신 + 카메라 적용
        CurrentRegionId = regionId;
        ApplyCameraFrame(region, instant: instantCamera);

        // 도착 직후 텔레포트 무적
        teleportImmunityUntil = Time.time + postTeleportImmunity;
    }

    public void SetCurrentRegion(string regionId, bool affectCamera = true, bool instantCamera = false)
    {
        var region = FindRegion(regionId);
        if (!region) return;

        CurrentRegionId = regionId;
        if (affectCamera) ApplyCameraFrame(region, instant: instantCamera);
    }

    public void HardResetScene()
    {
        // 싱글톤이 영속이므로 CurrentRegionId는 유지됨.
        if (Time.timeScale != 1f) Time.timeScale = 1f;

        var active = SceneManager.GetActiveScene();
        SceneManager.LoadScene(active.buildIndex, LoadSceneMode.Single);
        // 로드 완료 후 OnSceneLoaded에서 CurrentRegionId로 복구됨.
    }

    private void ApplyCameraFrame(CameraRegion2D region, bool instant = false)
    {
        if (!region) return;

        if (CameraDirector.Instance)
        {
            CameraDirector.Instance.WarpToRegion(region.regionId, instant: instant);
        }
        else
        {
            var cam = Camera.main;
            if (!cam) return;
            var pivot = region.pivot ? region.pivot : region.transform;
            var fromPos = cam.transform.position;
            cam.transform.position = new Vector3(pivot.position.x, pivot.position.y, fromPos.z);
            cam.orthographicSize = region.orthoSize;
        }
    }
}
