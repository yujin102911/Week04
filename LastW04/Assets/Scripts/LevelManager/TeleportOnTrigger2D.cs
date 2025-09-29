using System;
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

    [Header("End Game")]
    [Tooltip("엔딩 시 보여줄 패널(루트 오브젝트)")]
    [SerializeField] private GameObject endPanel;
    [SerializeField] private bool pauseTimeScaleWhileOpen = true;

    private void Awake()
    {
        TryBindLevelManager();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded_Rebind;
        if (!levelManager) TryBindLevelManager();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded_Rebind;
    }

    private void OnSceneLoaded_Rebind(Scene s, LoadSceneMode mode)
    {
        TryBindLevelManager();
    }

    private void TryBindLevelManager()
    {
        if (levelManager != null) return;

        levelManager = LevelManager.Instance;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(requiredTag)) return;

        if (!levelManager) TryBindLevelManager();
        if (!levelManager)
        {
            Debug.LogWarning("[TeleportTrigger] LevelManager not found. Teleport aborted.", this);
            return;
        }

        if (levelManager.IsTeleportImmune) return;

        // ★ 엔딩 처리 분기
        if (destinationRegionId == "Region_End")
        {
            if (endPanel) endPanel.SetActive(true);
            if (pauseTimeScaleWhileOpen) Time.timeScale = 0f;
            Debug.Log("[TeleportTrigger] Game End reached!");
            return;
        }

        // 일반 텔레포트
        levelManager.TeleportToRegion(destinationRegionId, affectCamera: true);

    }
}
