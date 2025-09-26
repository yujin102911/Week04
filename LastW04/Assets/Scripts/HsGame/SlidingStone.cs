// SlidingStone2D.cs
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class SlidingStone2D : MonoBehaviour
{
    [Header("Identity")]
    public int stoneId = 0; // 석상 매칭용

    [Header("Grid & Move")]
    [SerializeField] float cellSize = 1f;
    [SerializeField] Vector2 gridOrigin = Vector2.zero;   // (0,0) 셀의 "중심" 월드좌표
    [SerializeField, Min(0.1f)] float slideSpeed = 8f;

    [Header("Blocking")]
    [SerializeField] LayerMask blockingMask;               // 벽/다른 돌 등 막히는 대상 레이어
    [SerializeField, Min(1)] int maxSlideCells = 64;       // 안전장치: 최대 이동 칸수

    public bool IsSliding { get; private set; }

    // 셀<->월드(중심) 변환
    Vector2Int WorldToCell(Vector2 world)
    {
        Vector2 local = world - gridOrigin;
        return new Vector2Int(
            Mathf.RoundToInt(local.x / cellSize),
            Mathf.RoundToInt(local.y / cellSize)
        );
    }
    Vector2 CellToWorldCenter(Vector2Int cell)
    {
        return gridOrigin + new Vector2(cell.x * cellSize, cell.y * cellSize);
    }

    /// <summary>지정 방향으로 "막히는 칸 직전"까지 미끄러집니다.</summary>
    public void Push(Vector2 dir)
    {
        if (IsSliding) return;

        // 4방향 스냅
        Vector2Int d = Vector2Int.zero;
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y)) d = new Vector2Int((int)Mathf.Sign(dir.x), 0);
        else d = new Vector2Int(0, (int)Mathf.Sign(dir.y));
        if (d == Vector2Int.zero) return;

        // 시작 셀 중심으로 정렬
        Vector2Int startCell = WorldToCell(transform.position);
        Vector2 startCenter = CellToWorldCenter(startCell);
        if (((Vector2)transform.position - startCenter).sqrMagnitude > 0.0001f)
            transform.position = startCenter;

        // 한 칸씩 전진: "다음 칸의 중심 한 점"이 막혔는지 검사 → 정확히 앞칸까지 이동
        Vector2Int lastFreeCell = startCell;
        for (int i = 1; i <= maxSlideCells; i++)
        {
            Vector2Int nextCell = startCell + d * i;
            Vector2 nextCenter = CellToWorldCenter(nextCell);

            // 칸 중심 한 점만 검사(모서리 간섭 방지)
            Collider2D hit = Physics2D.OverlapPoint(nextCenter, blockingMask);
            if (hit != null) break;

            lastFreeCell = nextCell;
        }

        if (lastFreeCell == startCell) return;

        Vector2 targetCenter = CellToWorldCenter(lastFreeCell);
        StartCoroutine(SlideRoutine(targetCenter));
    }

    IEnumerator SlideRoutine(Vector2 targetCenter)
    {
        IsSliding = true;

        while ((targetCenter - (Vector2)transform.position).sqrMagnitude > 0.0001f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetCenter, slideSpeed * Time.deltaTime);
            yield return null;
        }

        // 최종 스냅: 셀 중심 고정
        Vector2Int finalCell = WorldToCell(targetCenter);
        transform.position = CellToWorldCenter(finalCell);

        IsSliding = false;

        // 석상 판정 갱신이 필요하면 주석 해제
        // Statue.ReevaluateAll();
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // 그리드 원점(0,0 셀 중심) 시각화
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(gridOrigin, 0.06f);
    }
#endif
}
