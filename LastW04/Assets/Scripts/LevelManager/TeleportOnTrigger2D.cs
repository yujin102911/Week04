using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class TeleportOnTrigger2D : MonoBehaviour
{

    [Header("Refs")]
    [SerializeField] private LevelManager levelManager;   // �ν����� ����Ǹ� ���, �ƴϸ� �ڵ� Ž��

    [Header("Logic")]
    [SerializeField] private string destinationRegionId = "Region_01";
    [SerializeField] private string requiredTag = "Player";

    [Header("End Game")]
    [Tooltip("���� �� ������ �г�(��Ʈ ������Ʈ)")]
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

        // �� ���� ó�� �б�
        if (destinationRegionId == "Region_End")
        {
            if (endPanel) endPanel.SetActive(true);
            if (pauseTimeScaleWhileOpen) Time.timeScale = 0f;
            Debug.Log("[TeleportTrigger] Game End reached!");
            return;
        }

        // �Ϲ� �ڷ���Ʈ
        levelManager.TeleportToRegion(destinationRegionId, affectCamera: true);

    }
}
