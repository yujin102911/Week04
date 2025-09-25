using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private Color previewColor = new Color(0, 1, 0, 0.5f);
    [SerializeField] private Color finalColor = Color.white;

    private bool isInstallMode = false;
    private bool isPlacing = false;

    private Vector3 startPosition;
    private WorldSpaceSlider previewInstance;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            isInstallMode = !isInstallMode;

            if (isInstallMode) StartInstallMode();
            else StopInstallMode();
        }

        if (!isInstallMode || previewInstance == null) return;

        HandlePlacement();
    }

    private void StartInstallMode()
    {
        if (objectPrefab == null)
        {
            Debug.LogError("ObjectPlacer의 Object Prefab 슬롯에 설치할 프리팹을 할당해야 합니다!");
            isInstallMode = false;
            return;
        }

        if (previewInstance == null)
        {
            GameObject newObject = Instantiate(objectPrefab);
            previewInstance = newObject.GetComponent<WorldSpaceSlider>();
            previewInstance.GetComponentInChildren<SpriteRenderer>().color = previewColor;
        }
    }

    private void StopInstallMode()
    {
        isPlacing = false;
        if (previewInstance != null)
        {
            Destroy(previewInstance.gameObject);
            previewInstance = null;
        }
    }

    private void HandlePlacement()
    {
        Vector3 mouseWorldPos = GetSnappedPosition(GetMouseWorldPosition());

        if (isPlacing)
        {
            previewInstance.Setup(startPosition, GetConstrainedMousePosition(startPosition, mouseWorldPos));

            if (Input.GetMouseButtonDown(0)) FinalizePlacement();
            else if (Input.GetMouseButtonDown(1)) CancelPlacement();
        }
        else
        {
            previewInstance.Setup(mouseWorldPos, mouseWorldPos);
            if (Input.GetMouseButtonDown(0)) StartPlacement(mouseWorldPos);
        }
    }

    private void StartPlacement(Vector3 position)
    {
        isPlacing = true;
        startPosition = position;
    }

    private void FinalizePlacement()
    {
        previewInstance.IsInstalled = true;
        previewInstance.GetComponentInChildren<SpriteRenderer>().color = finalColor;

        previewInstance = null; // 현재 미리보기 참조를 먼저 비워줍니다.
        StartInstallMode();     // 그 다음에 다음 설치를 위한 새 미리보기를 생성합니다.
        isPlacing = false;      // 마지막으로 설치 상태를 초기화합니다.
    }

    private void CancelPlacement()
    {
        isPlacing = false;
    }

    // --- Helper Functions ---
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane + 10;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    private Vector3 GetSnappedPosition(Vector3 originalPosition)
    {
        float snappedX = Mathf.Round(originalPosition.x);
        float snappedY = Mathf.Round(originalPosition.y);
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