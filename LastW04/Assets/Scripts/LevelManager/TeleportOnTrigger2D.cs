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

    private void Awake()
    {
        TryBindLevelManager();
    }

    private void OnEnable()
    {
        // �� ���ε� ����: �� �� �ε� �� �ٽ� ���ε�
        SceneManager.sceneLoaded += OnSceneLoaded_Rebind;
        if (!levelManager) TryBindLevelManager();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded_Rebind;
    }

    private void OnSceneLoaded_Rebind(Scene s, LoadSceneMode mode)
    {
        // ���� �ٲ�� ���� �̱����� ����ְ�, �� ��ũ��Ʈ�� ���� �ε�� �� �ٽ� ���
        TryBindLevelManager();
    }

    private void TryBindLevelManager()
    {
        if (levelManager != null) return;

        // 1����: �̱���
        levelManager = LevelManager.Instance;
        if (levelManager) return;

        // 2����: ������ �˻�(Ȥ�� �̱��� off�ų� Ư�� ���̽�)
        // levelManager = FindObjectOfType<LevelManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(requiredTag)) return;

        // Ȥ�� �� ���� ����: ���� �������� �� �� �� ����
        if (!levelManager) TryBindLevelManager();
        if (!levelManager)
        {
            Debug.LogWarning("[TeleportTrigger] LevelManager not found. Teleport aborted.", this);
            return;
        }

        if (levelManager.IsTeleportImmune)
        {
            // ������ ���� ���� � ������ ��
            // Debug.Log("[TeleportTrigger] Blocked by immunity.");
            return;
        }

        levelManager.TeleportToRegion(destinationRegionId, affectCamera: true);
    }
}
