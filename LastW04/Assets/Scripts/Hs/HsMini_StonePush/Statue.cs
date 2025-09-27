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

    [Tooltip("���� ���� ���̾�(��)�� �����ϼ���.")]
    [SerializeField] LayerMask stoneMask;

    [Tooltip("�� �±�(�������� ���� �� �±׸� ����). ���� �±� �˻縦 �����մϴ�.")]
    [SerializeField] string stoneTag = "Stone";

    public bool IsSatisfied { get; private set; }

    // === �̺�Ʈ: ��ĭ �� ID ��ȭ �˸� ===
    // C# �̺�Ʈ(��ũ��Ʈ ������)
    public event Action<int> OnFrontStoneIdChanged;
    // �ν����Ϳ��� ���ε� ������ UnityEvent
    [SerializeField] UnityEvent<int> onFrontStoneIdChanged;

    // ���� ����: ���� ������ �� ID (-1�̸� ����)
    int _currentFrontStoneId = -1;

    // === ���� ���� ===
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

    /// <summary>���� ���� ������ + ��ĭ �� ID ��ȭ �� �ݹ� ����</summary>
    bool Reevaluate()
    {
        Vector2 frontCenter = GetFrontCellCenter();

        // ����ũ + �±� + ������Ʈ(SlidingStone2D)�� �� ����
        int detectedId = DetectFrontStoneId(frontCenter);

        // ��ĭ �� ID ��ȭ �˸�
        if (detectedId != _currentFrontStoneId)
        {
            _currentFrontStoneId = detectedId;
            if (detectedId >= 0)
            {
                // ����/�����: id ����
                OnFrontStoneIdChanged?.Invoke(detectedId);
                onFrontStoneIdChanged?.Invoke(detectedId);
            }
            else
            {
                // ��������(���ϸ� ���� �̺�Ʈ�� �и� ����)
            }
        }

        // Ŭ���� ����(acceptStoneId ��Ī)
        bool newSatisfied = (detectedId >= 0) && (detectedId == acceptStoneId);

        if (newSatisfied != IsSatisfied)
        {
            IsSatisfied = newSatisfied;
            return true;
        }
        return false;
    }

    /// <summary>��ĭ �߽� �������� '��'�� ã�� stoneId ��ȯ. ������ -1.</summary>
    int DetectFrontStoneId(Vector2 frontCenter)
    {
        // OverlapBoxAll: ���� ��ġ�� ���� �ݶ��̴��� ���� �� ������ ���� Ȯ��
        var hits = Physics2D.OverlapBoxAll(frontCenter, goalBoxSize, 0f, stoneMask);
        if (hits == null || hits.Length == 0) return -1;

        for (int i = 0; i < hits.Length; i++)
        {
            var col = hits[i];
            if (col == null) continue;

            // �±� �˻�(�ɼ�)
            if (!string.IsNullOrEmpty(stoneTag) && !col.CompareTag(stoneTag))
                continue;

            // ������Ʈ���� ID ����
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
