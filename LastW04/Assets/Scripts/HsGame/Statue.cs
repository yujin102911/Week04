// Statue.cs
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] LayerMask stoneMask; // 돌(Stone) 레이어

    public bool IsSatisfied { get; private set; }

    static readonly List<Statue> s_all = new();
    public static System.Action OnAnyStatueStateChanged;

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

    bool Reevaluate()
    {
        Vector2 frontCenter = GetFrontCellCenter();
        var hit = Physics2D.OverlapBox(frontCenter, goalBoxSize, 0f, stoneMask);

        bool newSatisfied = false;
        if (hit != null && hit.TryGetComponent(out SlidingStone2D stone))
        {
            newSatisfied = (stone.stoneId == acceptStoneId);
        }

        if (newSatisfied != IsSatisfied)
        {
            IsSatisfied = newSatisfied;
            return true;
        }
        return false;
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
