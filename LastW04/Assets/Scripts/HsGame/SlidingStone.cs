using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class SlidingStone2D : MonoBehaviour
{
    [Header("Identity")]
    public int stoneId = 0; // 석상 매칭용

    [Header("Grid & Move")]
    [SerializeField] float cellSize = 1f;                         // 격자 단위
    [SerializeField, Min(0.1f)] float slideSpeed = 8f;            // 유닛/초
    [SerializeField] Vector2 overlapBoxSize = new Vector2(0.8f, 0.8f); // 칸 충돌 판정 크기

    [Header("Blocking")]
    [SerializeField] LayerMask blockingMask;                      // 벽/다른 돌 등이 들어간 레이어

    public bool IsSliding { get; private set; }

    /// <summary>지정 방향으로 “막힐 때까지” 미끄러뜨립니다.</summary>
    public void Push(Vector2 dir)
    {
        if (IsSliding) return;

        // 4방향 스냅
        Vector2Int d = Vector2Int.zero;
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y)) d = new Vector2Int((int)Mathf.Sign(dir.x), 0);
        else d = new Vector2Int(0, (int)Mathf.Sign(dir.y));
        if (d == Vector2Int.zero) return;

        Vector2 start = transform.position;
        Vector2 step = (Vector2)d * cellSize;
        Vector2 probe = start;

        // 다음 칸이 막히면 정지
        while (true)
        {
            Vector2 next = probe + step;
            var hit = Physics2D.OverlapBox(next, overlapBoxSize, 0f, blockingMask);
            if (hit != null) break;
            probe = next;
        }

        if ((probe - start).sqrMagnitude < 1e-6f) return; // 이동 없음
        StartCoroutine(SlideRoutine(probe));
    }

    IEnumerator SlideRoutine(Vector2 target)
    {
        IsSliding = true;

        while ((target - (Vector2)transform.position).sqrMagnitude > 0.0001f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, slideSpeed * Time.deltaTime);
            yield return null;
        }

        // 최종 스냅(격자 중심)
        transform.position = new Vector3(
            Mathf.Round(target.x / cellSize) * cellSize,
            Mathf.Round(target.y / cellSize) * cellSize,
            transform.position.z
        );

        IsSliding = false;

        // 석상 판정 갱신(선택) — 석상이 자동 감지하도록 하고 싶으면 주석 해제
        // Statue.ReevaluateAll();
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, (Vector3)overlapBoxSize);
    }
#endif
}
