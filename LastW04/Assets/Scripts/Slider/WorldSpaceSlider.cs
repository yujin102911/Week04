using UnityEngine;
using UnityEngine.UIElements;

public class WorldSpaceSlider : MonoBehaviour
{
    [Header("오브젝트 연결")]
    [SerializeField] private Transform handle;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private Transform body;
    //플레이서에서 복제 코드~
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private Color previewColor = new Color(0, 1, 0, 0.5f);
    [SerializeField] private Color errorColor = new Color(1, 0, 0, 0.5f);
    [SerializeField] private Color finalColor = Color.gray;

    private SpriteRenderer bodyColor;
    private Vector3 startPosition;
    //~플레이서에서 복제코드
    [Header("슬라이더 값")]
    [Range(0, 1)]
    [SerializeField] private float value = 0.5f; // 0.0 ~ 1.0 사이의 값

    public bool IsInstalled { get; set; } = false;
    public bool placed = false;
    //플레이서에서 복제 및 수정 코드~
    void Start()
    {
        Debug.Log(body.name);
        startPosition=transform.position;//시작지점 설정
        bodyColor = body.GetComponent<SpriteRenderer>();//색 설정
        bodyColor.color = previewColor;    
    }
    void Update()
    {
        if (GameManager.mode==Mode.Editing)
        {
            if (!IsInstalled)//설치 상태가 아니면
            {
                HandlePlacement();
            }
        }
        else
        {
            Destroy(gameObject);
        }

    }
    private void HandlePlacement()
    {
        Vector3 mouseWorldPos = GetSnappedPosition(GetMouseWorldPosition());//마우스 위치에 그리드 계산 결과를 저장

        Setup(startPosition, GetConstrainedMousePosition(startPosition, mouseWorldPos));//각도에 따라 90도 회전
        Collider2D[] colliders = Physics2D.OverlapAreaAll(startPoint.position, endPoint.position);//사이에 뭐가 있는지 확인
        foreach(Collider2D collider in colliders)
        {
            Debug.DrawLine(startPoint.position, endPoint.position, Color.cyan);
            if (collider.CompareTag("EditorbleUI") && collider != gameObject && collider != collider.transform.IsChildOf(transform))
            {
                bodyColor.color = errorColor;
                break;
            }
            else 
            {
                bodyColor.color = previewColor;
            }
        }

        if (Input.GetMouseButtonDown(0)&& bodyColor.color != errorColor) FinalizePlacement();//한번 더 누르면 위치 고정
        else if (Input.GetMouseButtonDown(1)) Destroy(gameObject);//취소시 나 제거
    }

    private void FinalizePlacement()
    {
        bodyColor.color = finalColor;
        IsInstalled = true;   // 마지막으로 설치 상태를 초기화합니다.
        placed = true;
    }

    // --- Helper Functions ---
    private Vector3 GetMouseWorldPosition()//마우스 좌표 받기
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane + 10;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    private Vector3 GetSnappedPosition(Vector3 originalPosition)//스냅 시키기
    {
        float snappedX = Mathf.Floor(originalPosition.x)+0.5f;
        float snappedY = Mathf.Floor(originalPosition.y) + 0.5f;
        return new Vector3(snappedX, snappedY, 0);
    }

    private Vector3 GetConstrainedMousePosition(Vector3 startPos, Vector3 currentPos)//마우스 각도따라 90도 전환
    {
        Vector3 direction = currentPos - startPos;
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            return new Vector3(currentPos.x, startPos.y, startPos.z);
        }
        else
        {
            return new Vector3(startPos.x, currentPos.y, startPos.z);
        }
    }

    //~플레이서에서 복제 및 수정 코드

    void OnValidate()
    {
        // 인스펙터에서 value 값을 조절할 때 실시간으로 핸들 위치를 업데이트합니다.
        //if (placed = true)
        {
            UpdateHandlePosition();
        }
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

        if (placed)
        {
            UpdateHandlePosition();
        }
        
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