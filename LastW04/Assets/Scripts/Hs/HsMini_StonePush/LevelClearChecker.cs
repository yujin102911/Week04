using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class LevelClearChecker : MonoBehaviour
{
    public bool autoLogClear = true; // 전부 만족 시 Debug.Log 출력

    void OnEnable()
    {
        Statue.OnAnyStatueStateChanged += CheckClear;
    }
    void OnDisable()
    {
        Statue.OnAnyStatueStateChanged -= CheckClear;
    }

    void Start()
    {
        // 초기 진입 시 1회 판정
        Statue.ReevaluateAll();
        CheckClear();
    }

    void CheckClear()
    {
        // 씬 내 모든 Statue가 만족이면 클리어
        var all = Object.FindObjectsByType<Statue>(FindObjectsSortMode.None); // ← 변경
        bool allOk = all.Length > 0 && all.All(s => s.IsSatisfied);

        if (allOk && autoLogClear)
        {
            Debug.Log("[MiniPuzzle] CLEAR: 모든 석상이 만족 상태입니다.");
            // 여기서 UI 토글/문 열기 등 원하는 액션 수행
        }
    }
}
