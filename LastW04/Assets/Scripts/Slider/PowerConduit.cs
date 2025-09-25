using UnityEngine;

public class PowerConduit : MonoBehaviour
{
    [SerializeField] private Transform body; // 인스펙터에서 Body 자식 오브젝트를 연결

    /// <summary>
    /// 시작점과 끝점을 기준으로 모양과 방향을 설정합니다.
    /// </summary>
    public void Setup(Vector3 startPosition, Vector3 endPosition)
    {
        // 두 점 사이의 방향과 거리를 계산
        Vector3 direction = endPosition - startPosition;
        float distance = direction.magnitude;

        // 위치를 두 점의 중간으로 설정
        transform.position = startPosition;

        // Body의 스케일을 거리만큼 늘려 길이를 표현 (Y, Z 스케일은 1로 유지)
        body.localScale = new Vector3(distance, body.localScale.y, body.localScale.z);

        // 시작점에서 끝점을 바라보도록 각도를 설정
        transform.right = direction.normalized;
    }
}