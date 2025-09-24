using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private Color previewColor = new Color(0, 1, 0, 0.5f);

    private bool isInstallMode = false;
    private bool isPlacing = false;

    private Vector3 startPosition;
    private WorldSpaceSlider previewInstance;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            isInstallMode = !isInstallMode;
            Debug.Log("설치 모드: " + isInstallMode);

            if (!isInstallMode && previewInstance != null)
            {
                Destroy(previewInstance.gameObject);
            }
        }

        if (!isInstallMode) return;

        HandlePlacement();
    }

    private void HandlePlacement()
    {
        // ▼▼▼ 변경된 부분 ▼▼▼
        // 마우스 월드 좌표를 가져온 뒤, 그리드에 맞춰 스냅시킵니다.
        Vector3 mouseWorldPos = GetSnappedPosition(GetMouseWorldPosition());

        if (Input.GetMouseButtonDown(0) && !isPlacing)
        {
            isPlacing = true;
            // 시작 위치 또한 스냅된 위치를 사용하게 됩니다.
            startPosition = mouseWorldPos;

            GameObject newObject = Instantiate(objectPrefab, startPosition, Quaternion.identity);
            previewInstance = newObject.GetComponent<WorldSpaceSlider>();
            previewInstance.GetComponentInChildren<SpriteRenderer>().color = previewColor;
        }
        else if (Input.GetMouseButton(0) && isPlacing)
        {
            // 현재 마우스 위치도 스냅된 위치를 사용합니다.
            Vector3 currentMousePos = GetConstrainedMousePosition(startPosition, mouseWorldPos);
            previewInstance.Setup(startPosition, currentMousePos);
        }
        else if (Input.GetMouseButtonUp(0) && isPlacing)
        {
            isPlacing = false;
            previewInstance.GetComponentInChildren<SpriteRenderer>().color = Color.white;
            previewInstance = null; 
        }
        else if (Input.GetMouseButtonDown(1) && isPlacing)
        {
            isPlacing = false;
            Destroy(previewInstance.gameObject);
            previewInstance = null;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane + 10;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    // ▼▼▼ 추가된 함수 ▼▼▼
    /// <summary>
    /// 주어진 좌표를 가장 가까운 정수 좌표(그리드)로 반올림합니다.
    /// </summary>
    private Vector3 GetSnappedPosition(Vector3 originalPosition)
    {
        float snappedX = Mathf.Round(originalPosition.x);
        float snappedY = Mathf.Round(originalPosition.y);
        // Z좌표는 2D 게임에서 중요하지 않으므로 0으로 고정하는 것이 좋습니다.
        return new Vector3(snappedX, snappedY, 0);
    }

    private Vector3 GetConstrainedMousePosition(Vector3 startPos, Vector3 currentPos)
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
}