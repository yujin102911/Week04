using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private Color previewColor = new Color(0, 1, 0, 0.5f);

    private bool isInstallMode = false;
    private bool isPlacing = false; // 현재 시작점이 지정되어 끝점을 기다리는 중인지?

    private Vector3 startPosition;
    private WorldSpaceSlider previewInstance;

    void Update()
    {
        // 'B' 키로 설치 모드 ON/OFF
        if (Input.GetKeyDown(KeyCode.B))
        {
            isInstallMode = !isInstallMode;
            Debug.Log("설치 모드: " + isInstallMode);

            // 설치 모드를 나갈 때 미리보기가 남아있으면 취소시킴
            if (!isInstallMode && isPlacing)
            {
                CancelPlacement();
            }
        }

        if (!isInstallMode) return;

        HandlePlacement();
    }

    private void HandlePlacement()
    {
        // 타일맵 그리드에 스냅된 마우스 위치 계산 (원하는 방식으로 선택)
        Vector3 mouseWorldPos = GetSnappedPosition(GetMouseWorldPosition());

        // 만약 설치 중(시작점 지정 후)이라면
        if (isPlacing)
        {
            // 미리보기 업데이트: 시작점은 고정, 끝점은 마우스를 따라다님
            Vector3 constrainedMousePos = GetConstrainedMousePosition(startPosition, mouseWorldPos);
            previewInstance.Setup(startPosition, constrainedMousePos);

            // 마우스 왼쪽 버튼을 두 번째로 클릭했을 때 (설치 완료)
            if (Input.GetMouseButtonDown(0))
            {
                FinalizePlacement();
            }
            // 마우스 오른쪽 버튼을 눌렀을 때 (설치 취소)
            else if (Input.GetMouseButtonDown(1))
            {
                CancelPlacement();
            }
        }
        // 아직 설치 시작 전이라면
        else
        {
            // 마우스 왼쪽 버튼을 처음 클릭했을 때 (설치 시작)
            if (Input.GetMouseButtonDown(0))
            {
                StartPlacement(mouseWorldPos);
            }
        }
    }

    // 설치 시작
    private void StartPlacement(Vector3 position)
    {
        isPlacing = true;
        startPosition = position;

        // 미리보기 객체 생성
        GameObject newObject = Instantiate(objectPrefab, startPosition, Quaternion.identity);
        previewInstance = newObject.GetComponent<WorldSpaceSlider>();
        previewInstance.GetComponentInChildren<SpriteRenderer>().color = previewColor;
    }

    // 설치 확정
    private void FinalizePlacement()
    {
        isPlacing = false;
        previewInstance.GetComponentInChildren<SpriteRenderer>().color = Color.white;
        previewInstance = null; // 다음 설치를 위해 참조를 비움
    }

    // 설치 취소
    private void CancelPlacement()
    {
        isPlacing = false;
        Destroy(previewInstance.gameObject);
        previewInstance = null;
    }

    // 마우스 월드 좌표 계산
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane + 10;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    // 타일 모서리에 맞춤
    private Vector3 GetSnappedPosition(Vector3 originalPosition)
    {
        float snappedX = Mathf.Round(originalPosition.x);
        float snappedY = Mathf.Round(originalPosition.y);
        return new Vector3(snappedX, snappedY, 0);
    }

    // 0도 또는 90도로 각도 보정
    private Vector3 GetConstrainedMousePosition(Vector3 startPos, Vector3 currentPos)
    {
        Vector3 direction = currentPos - startPos;

        // X축 변화량의 절댓값이 Y축 변화량의 절댓값보다 크면 수평으로 고정
        // (시작점-끝점 각도가 -45도 ~ 45도 사이일 때)
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            return new Vector3(currentPos.x, startPos.y, startPos.z);
        }
        // 그렇지 않으면 수직으로 고정
        // (각도가 45~90도 또는 -45~-90도 사이일 때)
        else
        {
            return new Vector3(startPos.x, currentPos.y, startPos.z);
        }
    }
}