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

    // ✅ gridOrigin을 "교차점(모서리)" 기준으로 입력했는지 여부
    //  - 타일맵 앵커가 0.5,0.5인 일반 케이스: true 로 두면 자동으로 중심 보정됩니다.
    [SerializeField] bool originIsIntersection = true;

    // ✅ gridOrigin은 씬의 (0,0) 셀 '교차점' 또는 '중심' 월드좌표 (위 플래그에 따라 보정)
    [SerializeField] Vector2 gridOrigin = Vector2.zero;

    [SerializeField, Min(0.1f)] float slideSpeed = 8f;

    [Header("Blocking")]
    [SerializeField] LayerMask blockingMask;               // 벽/다른 돌 등 막히는 대상 레이어
    [SerializeField, Min(1)] int maxSlideCells = 64;       // 안전장치: 최대 이동 칸수

    public bool IsSliding { get; private set; }

    // === 내부: 항상 "셀 중심 원점"으로 사용하도록 보정 ===
    Vector2 OriginCenter
        => originIsIntersection
           ? gridOrigin + new Vector2(cellSize * 0.5f, cellSize * 0.5f)  // 교차점 입력 시 중심 보정
           : gridOrigin;                                                // 이미 중심이면 그대로

    // 셀<->월드(중심) 변환
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

        // 한 칸씩 전진: "다음 칸의 중심 한 점"이 막혔는지 검사
        //  - 모서리 경계에 걸리는 오검출을 피하기 위해 '아주 조금' 이동 방향으로 바이어스
        //  - 자기 자신 콜라이더는 제외
        Vector2Int lastFreeCell = startCell;
        float bias = cellSize * 0.001f; // 0.1% 정도의 미세한 바이어스
        for (int i = 1; i <= maxSlideCells; i++)
        {
            Vector2Int nextCell = startCell + d * i;
            Vector2 nextCenter = CellToWorldCenter(nextCell) + (Vector2)d * bias;

            var hits = Physics2D.OverlapPointAll(nextCenter, blockingMask);
            bool blocked = false;
            for (int h = 0; h < hits.Length; h++)
            {
                if (hits[h] && hits[h].transform != transform) { blocked = true; break; } // 자기 자신 제외
            }

            if (blocked) break;       // nextCell이 막혔으므로 lastFreeCell에서 정지
            lastFreeCell = nextCell;  // 비어있으면 계속 진행
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

        // 최종 스냅(셀 중심 고정)
        Vector2Int finalCell = WorldToCell(targetCenter);
        transform.position = CellToWorldCenter(finalCell);

        IsSliding = false;

        Statue.ReevaluateAll();
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // 그리드 원점(교차점/중심) 및 보정된 중심 시각화
        Gizmos.color = Color.magenta;  // 입력된 gridOrigin 자체
        Gizmos.DrawWireSphere(gridOrigin, 0.06f);

        Gizmos.color = Color.cyan;     // 내부적으로 사용하는 '셀 중심 원점'
        var oc = OriginCenter;
        Gizmos.DrawWireSphere(oc, 0.06f);
    }
#endif
}
