using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class TeleportOnTrigger2D : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private LevelManager levelManager;   // 인스펙터 연결되면 사용, 아니면 자동 탐색

    [Header("Logic")]
    [SerializeField] private string destinationRegionId = "Region_01";
    [SerializeField] private string requiredTag = "Player";

    private void Awake()
    {
        TryBindLevelManager();
    }

    private void OnEnable()
    {
        // 씬 리로드 대응: 새 씬 로드 후 다시 바인딩
        SceneManager.sceneLoaded += OnSceneLoaded_Rebind;
        if (!levelManager) TryBindLevelManager();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded_Rebind;
    }

    private void OnSceneLoaded_Rebind(Scene s, LoadSceneMode mode)
    {
        // 씬이 바뀌면 영속 싱글톤은 살아있고, 이 스크립트는 새로 로드됨 → 다시 잡기
        TryBindLevelManager();
    }

    private void TryBindLevelManager()
    {
        if (levelManager != null) return;

        // 1순위: 싱글톤
        levelManager = LevelManager.Instance;
        if (levelManager) return;

        // 2순위: 씬에서 검색(혹시 싱글톤 off거나 특수 케이스)
        // levelManager = FindObjectOfType<LevelManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(requiredTag)) return;

        // 혹시 모를 누락 방지: 진입 순간에도 한 번 더 보장
        if (!levelManager) TryBindLevelManager();
        if (!levelManager)
        {
            Debug.LogWarning("[TeleportTrigger] LevelManager not found. Teleport aborted.", this);
            return;
        }

        if (levelManager.IsTeleportImmune)
        {
            // 리스폰 직후 무적 등에 막혔을 때
            // Debug.Log("[TeleportTrigger] Blocked by immunity.");
            return;
        }

        levelManager.TeleportToRegion(destinationRegionId, affectCamera: true);
    }
}
