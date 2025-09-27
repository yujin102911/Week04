// LevelClearChecker.cs
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class LevelClearChecker : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private Statue[] statues;            // 직접 연결할 석상들
    [SerializeField] private ToggleTarget[] doorsToOpen;  // 모두 완료되면 여는 문들

    [Header("Options")]
    [SerializeField] private bool autoLogClear = true;    // 완료 로그 출력 여부
    [SerializeField] private bool openOnlyOnce = true;    // 한 번 열었으면 이후 무시

    bool _clearedOnce = false;

    void OnEnable()
    {
        // 씬 전체 검색은 하지 않되, 석상 상태 변화 알림은 전역 이벤트로 수신
        Statue.OnAnyStatueStateChanged += CheckClear;
    }

    void OnDisable()
    {
        Statue.OnAnyStatueStateChanged -= CheckClear;
    }

    void Start()
    {
        // 초기 1회 판정(현재 씬의 석상 상태 재평가)
        Statue.ReevaluateAll();
        CheckClear();
    }

    void CheckClear()
    {
        if (statues == null || statues.Length == 0) return;

        bool allOk = statues.All(s => s != null && s.IsSatisfied);
        if (!allOk) return;

        if (openOnlyOnce && _clearedOnce) return; // 이미 열었으면 다시 실행하지 않음
        _clearedOnce = true;

        // 문 열기: isOn=true
        if (doorsToOpen != null)
        {
            foreach (var door in doorsToOpen)
            {
                if (door != null) door.SetState(true);
            }
        }

        if (autoLogClear)
            Debug.Log("[MiniPuzzle] CLEAR: 연결된 모든 석상이 만족 상태입니다. 문을 엽니다 (isOn=true).");
    }
}
