// SlidingStone2D.cs
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class SlidingStone2D : MonoBehaviour
{
    [Header("Identity")]
    public int stoneId = 0;                          // 석상 매칭용

    [Header("Grid & Move")]
    [SerializeField] float cellSize = 1f;
    [SerializeField] bool originIsIntersection = true; // gridOrigin이 교차점 기준이면 true
    [SerializeField] Vector2 gridOrigin = Vector2.zero; // (0,0) 셀의 교차점 또는 중심(위 플래그에 따라 보정)
    [SerializeField, Min(0.1f)] float slideSpeed = 8f;

    [Header("Blocking")]
    [SerializeField] LayerMask blockingMask;         // 벽 등 막히는 레이어
    [SerializeField, Min(1)] int maxSlideCells = 64; // 최대 이동 칸(안전장치)
    [SerializeField] string stoneTag = "Stone";      // 이 태그도 벽처럼 취급(돌끼리 겹침 방지)

    public bool IsSliding { get; private set; }

    // 내부: 항상 "셀 중심 원점"으로 사용
    Vector2 OriginCenter => originIsIntersection
        ? gridOrigin + new Vector2(cellSize * 0.5f, cellSize * 0.5f)
        : gridOrigin;

    Vector2Int WorldToCell(Vector2 world)
    {
        Vector2 local = world - OriginCenter;
        return new Vector2Int(
            Mathf.RoundToInt(local.x / cellSize),
            Mathf.RoundToInt(local.y / cellSize)
        );
    }

    Vector2 CellToWorldCenter(Vector2Int cell)
    {
        return OriginCenter + new Vector2(cell.x * cellSize, cell.y * cellSize);
    }

    /// <summary>지정 방향으로 막히는 칸 직전까지 미끄러집니다.</summary>
    public void Push(Vector2 dir)
    {
        if (IsSliding) return;

        // 4방향 스냅
        Vector2Int d = Vector2Int.zero;
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y)) d = new Vector2Int((int)Mathf.Sign(dir.x), 0);
        else d = new Vector2Int(0, (int)Mathf.Sign(dir.y));
        if (d == Vector2Int.zero) return;

        // 시작 셀 중심 정렬
        Vector2Int startCell = WorldToCell(transform.position);
        Vector2 startCenter = CellToWorldCenter(startCell);
        if (((Vector2)transform.position - startCenter).sqrMagnitude > 0.0001f)
            transform.position = startCenter;

        // 한 칸씩 전진하면서 다음 칸 '중심 한 점'이 막혔는지 검사
        Vector2Int lastFreeCell = startCell;
        float bias = cellSize * 0.001f; // 모서리 경계 오검출 방지용 미세 바이어스
        for (int i = 1; i <= maxSlideCells; i++)
        {
            Vector2Int nextCell = startCell + d * i;
            Vector2 nextCenter = CellToWorldCenter(nextCell) + (Vector2)d * bias;

            // 모든 레이어를 받아온 후, (블로킹 마스크) 또는 (Stone 태그) 이면 막힘
            var hits = Physics2D.OverlapPointAll(nextCenter, ~0);
            bool blocked = false;
            for (int h = 0; h < hits.Length; h++)
            {
                var col = hits[h];
                if (!col || col.transform == transform) continue; // 자기 자신 제외

                bool inBlockingMask = (blockingMask.value & (1 << col.gameObject.layer)) != 0;
                bool isStone = !string.IsNullOrEmpty(stoneTag) && col.CompareTag(stoneTag);

                if (inBlockingMask || isStone)
                {
                    blocked = true;
                    break;
                }
            }

            if (blocked) break;       // 다음 칸이 막힘 → 직전 칸에서 정지
            lastFreeCell = nextCell;  // 비어있음 → 계속 진행
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

        // 최종 스냅
        Vector2Int finalCell = WorldToCell(targetCenter);
        transform.position = CellToWorldCenter(finalCell);

        IsSliding = false;

        // 클리어 판정 갱신
        Statue.ReevaluateAll();
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // 입력된 gridOrigin(마젠타) / 보정된 중심 원점(시안)
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(gridOrigin, 0.06f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(OriginCenter, 0.06f);
    }
#endif
}
