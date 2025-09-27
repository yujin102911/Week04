// LevelClearChecker.cs
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class LevelClearChecker : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private Statue[] statues;            // ���� ������ �����
    [SerializeField] private ToggleTarget[] doorsToOpen;  // ��� �Ϸ�Ǹ� ���� ����

    [Header("Options")]
    [SerializeField] private bool autoLogClear = true;    // �Ϸ� �α� ��� ����
    [SerializeField] private bool openOnlyOnce = true;    // �� �� �������� ���� ����

    bool _clearedOnce = false;

    void OnEnable()
    {
        // �� ��ü �˻��� ���� �ʵ�, ���� ���� ��ȭ �˸��� ���� �̺�Ʈ�� ����
        Statue.OnAnyStatueStateChanged += CheckClear;
    }

    void OnDisable()
    {
        Statue.OnAnyStatueStateChanged -= CheckClear;
    }

    void Start()
    {
        // �ʱ� 1ȸ ����(���� ���� ���� ���� ����)
        Statue.ReevaluateAll();
        CheckClear();
    }

    void CheckClear()
    {
        if (statues == null || statues.Length == 0) return;

        bool allOk = statues.All(s => s != null && s.IsSatisfied);
        if (!allOk) return;

        if (openOnlyOnce && _clearedOnce) return; // �̹� �������� �ٽ� �������� ����
        _clearedOnce = true;

        // �� ����: isOn=true
        if (doorsToOpen != null)
        {
            foreach (var door in doorsToOpen)
            {
                if (door != null) door.SetState(true);
            }
        }

        if (autoLogClear)
            Debug.Log("[MiniPuzzle] CLEAR: ����� ��� ������ ���� �����Դϴ�. ���� ���ϴ� (isOn=true).");
    }
}
