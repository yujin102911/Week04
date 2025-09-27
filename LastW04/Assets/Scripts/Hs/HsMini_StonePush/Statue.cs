// Statue.cs
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Facing4 { Up, Down, Left, Right }

[DisallowMultipleComponent]
public class Statue : MonoBehaviour
{
    [Header("Goal Rule")]
    public int acceptStoneId = 0;
    public Facing4 facing = Facing4.Up;
    public float cellSize = 1f;

    [Header("Detection")]
    [SerializeField] Vector2 goalBoxSize = new Vector2(0.8f, 0.8f);

    [Tooltip("돌이 속한 레이어(들)만 포함하세요.")]
    [SerializeField] LayerMask stoneMask;

    [Tooltip("돌 태그(레벨에서 돌에 이 태그를 지정). 비우면 태그 검사를 생략합니다.")]
    [SerializeField] string stoneTag = "Stone";

    public bool IsSatisfied { get; private set; }

    // === 이벤트: 앞칸 돌 ID 변화 알림 ===
    // C# 이벤트(스크립트 구독용)
    public event Action<int> OnFrontStoneIdChanged;
    // 인스펙터에서 바인딩 가능한 UnityEvent
    [SerializeField] UnityEvent<int> onFrontStoneIdChanged;

    // 내부 상태: 직전 감지된 돌 ID (-1이면 없음)
    int _currentFrontStoneId = -1;

    // === 정적 관리 ===
    static readonly List<Statue> s_all = new();
    public static Action OnAnyStatueStateChanged;

    void OnEnable() { s_all.Add(this); }
    void OnDisable() { s_all.Remove(this); }

    public static void ReevaluateAll()
    {
        bool anyChanged = false;
        foreach (var st in s_all)
        {
            bool changed = st.Reevaluate();
            if (changed) anyChanged = true;
        }
        if (anyChanged) OnAnyStatueStateChanged?.Invoke();
    }

    /// <summary>현재 상태 재판정 + 앞칸 돌 ID 변화 시 콜백 발행</summary>
    bool Reevaluate()
    {
        Vector2 frontCenter = GetFrontCellCenter();

        // 마스크 + 태그 + 컴포넌트(SlidingStone2D)로 돌 판정
        int detectedId = DetectFrontStoneId(frontCenter);

        // 앞칸 돌 ID 변화 알림
        if (detectedId != _currentFrontStoneId)
        {
            _currentFrontStoneId = detectedId;
            if (detectedId >= 0)
            {
                // 들어옴/변경됨: id 통지
                OnFrontStoneIdChanged?.Invoke(detectedId);
                onFrontStoneIdChanged?.Invoke(detectedId);
            }
            else
            {
                // 빠져나감(원하면 별도 이벤트로 분리 가능)
            }
        }

        // 클리어 판정(acceptStoneId 매칭)
        bool newSatisfied = (detectedId >= 0) && (detectedId == acceptStoneId);

        if (newSatisfied != IsSatisfied)
        {
            IsSatisfied = newSatisfied;
            return true;
        }
        return false;
    }

    /// <summary>앞칸 중심 영역에서 '돌'을 찾고 stoneId 반환. 없으면 -1.</summary>
    int DetectFrontStoneId(Vector2 frontCenter)
    {
        // OverlapBoxAll: 동일 위치에 여러 콜라이더가 있을 수 있으니 전부 확인
        var hits = Physics2D.OverlapBoxAll(frontCenter, goalBoxSize, 0f, stoneMask);
        if (hits == null || hits.Length == 0) return -1;

        for (int i = 0; i < hits.Length; i++)
        {
            var col = hits[i];
            if (col == null) continue;

            // 태그 검사(옵션)
            if (!string.IsNullOrEmpty(stoneTag) && !col.CompareTag(stoneTag))
                continue;

            // 컴포넌트에서 ID 추출
            if (col.TryGetComponent(out SlidingStone2D stone))
                return stone.stoneId;
        }
        return -1;
    }

    Vector2 GetFrontDir()
    {
        return facing switch
        {
            Facing4.Up => Vector2.up,
            Facing4.Down => Vector2.down,
            Facing4.Left => Vector2.left,
            Facing4.Right => Vector2.right,
            _ => Vector2.up
        };
    }

    Vector2 GetFrontCellCenter()
    {
        return (Vector2)transform.position + GetFrontDir() * cellSize;
    }

    public class StatueIdLogger : UnityEngine.MonoBehaviour
    {
        [UnityEngine.SerializeField] private Statue statue;

        void Reset() { statue = GetComponent<Statue>(); }

        void OnEnable()
        {
            if (statue != null) statue.OnFrontStoneIdChanged += OnFrontId;
        }
        void OnDisable()
        {
            if (statue != null) statue.OnFrontStoneIdChanged -= OnFrontId;
        }
        void OnFrontId(int id)
        {
            UnityEngine.Debug.Log($"[Statue] front stone id = {id}");
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = IsSatisfied ? Color.green : Color.yellow;
        Gizmos.DrawWireCube(GetFrontCellCenter(), (Vector3)goalBoxSize);

        var p = transform.position;
        var d = (Vector3)GetFrontDir() * cellSize;
        Gizmos.DrawLine(p, p + d);
    }
#endif
}
