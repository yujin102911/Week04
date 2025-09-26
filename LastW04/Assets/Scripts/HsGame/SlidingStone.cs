using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class SlidingStone2D : MonoBehaviour
{
    [Header("Identity")]
    public int stoneId = 0; // 석상 매칭용

    [Header("Grid & Move")]
    [SerializeField] float cellSize = 1f;
    [SerializeField] Vector2 gridOrigin = Vector2.zero; // ✅ (0,0) 셀의 "중심" 월드좌표
    [SerializeField, Min(0.1f)] float slideSpeed = 8f;
    [SerializeField] Vector2 overlapBoxSize = new Vector2(0.8f, 0.8f);

    [Header("Blocking")]
    [SerializeField] LayerMask blockingMask;   // 벽/다른 돌 등 '막아야 하는' 레이어
    [SerializeField, Min(1)] int maxSlideCells = 64; // 안전장치: 최대 이동 칸수

    public bool IsSliding { get; private set; }

    // ===== 좌표 변환 유틸(간단/일관 스냅) =====
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

    public void Push(Vector2 dir)
    {
        if (IsSliding) return;

        // 4방향 스냅
        Vector2Int d = Vector2Int.zero;
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y)) d = new Vector2Int((int)Mathf.Sign(dir.x), 0);
        else d = new Vector2Int(0, (int)Mathf.Sign(dir.y));
        if (d == Vector2Int.zero) return;

        // 시작 위치를 셀 중심으로 스냅(원점이 이미 중심이면 영향 없음)
        Vector2 start = new Vector2(
            Mathf.Round(transform.position.x / cellSize) * cellSize,
            Mathf.Round(transform.position.y / cellSize) * cellSize
        );

        Vector2 lastFree = start;
        for (int i = 1; i <= maxSlideCells; i++)
        {
            Vector2 nextCenter = start + (Vector2)d * (i * cellSize);

            // 다음 칸이 막히면 현재(lastFree)에서 정지
            var hit = Physics2D.OverlapBox(nextCenter, overlapBoxSize, 0f, blockingMask);
            if (hit != null)
                break;

            lastFree = nextCenter;
        }

        // 이동할 칸이 없으면 리턴
        if ((lastFree - start).sqrMagnitude < 1e-6f) return;

        // 바로 이동 시작
        StartCoroutine(SlideRoutine(lastFree));
    }

    IEnumerator SlideRoutine(Vector2 targetCenter)
    {
        IsSliding = true;

        while ((targetCenter - (Vector2)transform.position).sqrMagnitude > 0.0001f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetCenter, slideSpeed * Time.deltaTime);
            yield return null;
        }

        // 최종 스냅: 반드시 "셀 중심"으로
        Vector2Int finalCell = WorldToCell(targetCenter);
        transform.position = CellToWorldCenter(finalCell);

        IsSliding = false;

        // 필요 시 클리어 판정 갱신
        // Statue.ReevaluateAll();
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, (Vector3)overlapBoxSize);

        // 그리드 원점 시각화(0,0 셀 중심)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(gridOrigin, 0.06f);
    }
#endif
}
