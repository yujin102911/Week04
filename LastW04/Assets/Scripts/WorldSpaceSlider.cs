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

    public bool IsInstalled { get; set; } = false;

    void OnValidate()
    {
        // 인스펙터에서 value 값을 조절할 때 실시간으로 핸들 위치를 업데이트합니다.
        UpdateHandlePosition();
    }

    // 핸들의 위치를 기반으로 value 값을 업데이트하는 함수 (SliderHandle이 호출)
    public void UpdateValueFromHandlePosition(Vector3 handleWorldPos)
    {
        Vector3 trackDirection = endPoint.position - startPoint.position;

        // ▼▼▼ 이 예외 처리 코드를 추가하세요! ▼▼▼
        // 슬라이더의 길이가 0에 가까우면 오류를 방지하기 위해 계산을 중단합니다.
        if (trackDirection.magnitude < 0.01f)
        {
            SetValue(0); // 길이를 0으로 설정하거나 그냥 return 해도 됩니다.
            return;
        }
        // ▲▲▲ 여기까지 추가 ▲▲▲

        Vector3 handleDirection = handleWorldPos - startPoint.position;
        Vector3 projectedVector = Vector3.Project(handleDirection, trackDirection);
        float newValue = projectedVector.magnitude / trackDirection.magnitude;

        if (Vector3.Dot(projectedVector, trackDirection) < 0)
        {
            newValue = 0;
        }

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

        transform.position = startPos;

        // ▼▼▼ 이 부분을 수정하세요! ▼▼▼
        // 거리가 0에 가까우면(미리보기 상태) 방향을 기본값으로 설정하여 오류 방지
        if (distance < 0.01f)
        {
            transform.right = Vector3.right; // 기본적으로 오른쪽을 보도록 설정
        }
        else
        {
            transform.right = direction.normalized;
        }
        // ▲▲▲ 여기까지 수정 ▲▲▲

        body.localScale = new Vector3(distance, body.localScale.y, body.localScale.z);
        endPoint.localPosition = new Vector3(distance, 0, 0);
        SetValue(1.0f);
    }
}