using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class LevelManager : MonoBehaviour
{
    // ���� Singleton (����)
    public static LevelManager Instance { get; private set; }

    [Header("Scene Refs")]
    [SerializeField] private Transform player; // ���ε� �� ����ε���

    private CameraRegion2D[] regions;
    public string CurrentRegionId { get; private set; }

    [Header("Teleport Safety")]
    [SerializeField, Min(0f)] private float postTeleportImmunity = 0.3f;
    private float teleportImmunityUntil = -1f;
    public bool IsTeleportImmune => Time.time < teleportImmunityUntil;

    private void Awake()
    {
        // �̱��� ���� + ����
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // �� �ε� �ݹ� ���
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // ù ����(��Ʈ �� ����)������ 1ȸ ����
        CacheSceneObjects();
        // CurrentRegionId�� ������� ����(��: Region_01)
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
            // ��Ʈ������ �Ѿ�� ���, �̹� ������ �ִ� Region���� ��� ����
            var region = FindRegion(CurrentRegionId);
            if (region) TeleportToRegion(CurrentRegionId, true, instantCamera: true);
        }
    }

    private void Update()
    {
        // �� Input System
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            HardResetScene();
    }

    // ���� �� �ε尡 ���� ��, ��� ���� ����ε� + ���� �������� ����
    private void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {
        CacheSceneObjects();

        if (!string.IsNullOrEmpty(CurrentRegionId))
        {
            var region = FindRegion(CurrentRegionId);
            if (region)
            {
                // �ε� �� ù �������� ��� ���� ����
                TeleportToRegion(CurrentRegionId, affectCamera: true, instantCamera: true);
                return;
            }
        }

        // ���� ������ �� ã���� ����
        var first = FindRegion("Region_01");
        if (first)
        {
            CurrentRegionId = first.regionId;
            ApplyCameraFrame(first, instant: true);
        }
    }

    private void CacheSceneObjects()
    {
        // Region �����(��Ȱ�� ����)
        regions = Object.FindObjectsByType<CameraRegion2D>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);

        // �÷��̾� ����ε�(�ν����� ������ �� ���ε�� ������ �� ����)
        if (!player)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go) player = go.transform;
            // �±װ� ���ٸ� ���⼭ �ٸ� ������� ã�Ƶ� ��(�̸�/������Ʈ ��)
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

        // �̵� �غ�
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

        // ��ġ ����
        player.position = spawn.position;
        Physics2D.SyncTransforms();
        if (rb) rb.WakeUp();

        // ���� ���� + ī�޶� ����
        CurrentRegionId = regionId;
        ApplyCameraFrame(region, instant: instantCamera);

        // ���� ���� �ڷ���Ʈ ����
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
        // �̱����� �����̹Ƿ� CurrentRegionId�� ������.
        if (Time.timeScale != 1f) Time.timeScale = 1f;

        var active = SceneManager.GetActiveScene();
        SceneManager.LoadScene(active.buildIndex, LoadSceneMode.Single);
        // �ε� �Ϸ� �� OnSceneLoaded���� CurrentRegionId�� ������.
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
