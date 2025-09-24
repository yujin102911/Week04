using UnityEngine;

public class WorldSpaceSlider : MonoBehaviour
{
    [Header("오브젝트 연결")]
    [SerializeField] private Transform handle;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private Transform body;

    [Header("슬라이더 값")]
    [Range(0, 1)]
    [SerializeField] private float value = 0.5f; // 0.0 ~ 1.0 사이의 값

    void OnValidate()
    {
        // 인스펙터에서 value 값을 조절할 때 실시간으로 핸들 위치를 업데이트합니다.
        UpdateHandlePosition();
    }

    // 핸들의 위치를 기반으로 value 값을 업데이트하는 함수 (SliderHandle이 호출)
    public void UpdateValueFromHandlePosition(Vector3 handleWorldPos)
    {
        // 벡터 프로젝션(Vector Projection)을 사용하여 마우스 위치가
        // 슬라이더 트랙 위의 어느 지점에 해당하는지 계산합니다.
        Vector3 trackDirection = endPoint.position - startPoint.position;
        Vector3 handleDirection = handleWorldPos - startPoint.position;

        // 투영된 벡터 계산
        Vector3 projectedVector = Vector3.Project(handleDirection, trackDirection);

        // value 값 계산 (투영된 벡터의 길이 / 전체 트랙의 길이)
        float newValue = projectedVector.magnitude / trackDirection.magnitude;

        // 투영된 벡터의 방향이 트랙 방향과 반대이면 value는 0이 되어야 합니다.
        if (Vector3.Dot(projectedVector, trackDirection) < 0)
        {
            newValue = 0;
        }

        // 계산된 값으로 value를 설정합니다.
        SetValue(newValue);
    }

    // 외부에서 value 값을 설정하는 함수
    public void SetValue(float newValue)
    {
        // 값을 0과 1 사이로 제한합니다.
        value = Mathf.Clamp01(newValue);
        UpdateHandlePosition();
    }

    // 현재 value 값에 맞춰 핸들의 위치를 업데이트합니다.
    private void UpdateHandlePosition()
    {
        if (handle != null && startPoint != null && endPoint != null)
        {
            // Lerp 함수를 사용하여 시작점과 끝점 사이의 정확한 위치를 계산합니다.
            handle.position = Vector3.Lerp(startPoint.position, endPoint.position, value);
        }
    }

    // ObjectPlacer가 호출할 설치 함수 (기존 PowerConduit.cs의 Setup 역할)
    public void Setup(Vector3 startPos, Vector3 endPos)
    {
        Vector3 direction = endPos - startPos;
        float distance = direction.magnitude;

        // 전체 슬라이더의 위치와 방향 설정
        transform.position = startPos;
        transform.right = direction.normalized;

        // Body(막대)의 길이를 조절
        body.localScale = new Vector3(distance, body.localScale.y, body.localScale.z);

        // EndPoint의 로컬 위치를 막대의 끝으로 이동
        endPoint.localPosition = new Vector3(distance, 0, 0);

        // 초기 값에 따라 핸들 위치 업데이트
        //UpdateHandlePosition();

        SetValue(0f);
    }
}