using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }
    public string respawnRegionId;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log($"[GameSession] Awake (persist). respawn='{respawnRegionId}'");
    }
}
