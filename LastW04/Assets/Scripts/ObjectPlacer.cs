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
            Debug.Log("��ġ ���: " + isInstallMode);

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
        // ���� ����� �κ� ����
        // ���콺 ���� ��ǥ�� ������ ��, �׸��忡 ���� ������ŵ�ϴ�.
        Vector3 mouseWorldPos = GetSnappedPosition(GetMouseWorldPosition());

        if (Input.GetMouseButtonDown(0) && !isPlacing)
        {
            isPlacing = true;
            // ���� ��ġ ���� ������ ��ġ�� ����ϰ� �˴ϴ�.
            startPosition = mouseWorldPos;

            GameObject newObject = Instantiate(objectPrefab, startPosition, Quaternion.identity);
            previewInstance = newObject.GetComponent<WorldSpaceSlider>();
            previewInstance.GetComponentInChildren<SpriteRenderer>().color = previewColor;
        }
        else if (Input.GetMouseButton(0) && isPlacing)
        {
            // ���� ���콺 ��ġ�� ������ ��ġ�� ����մϴ�.
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

    // ���� �߰��� �Լ� ����
    /// <summary>
    /// �־��� ��ǥ�� ���� ����� ���� ��ǥ(�׸���)�� �ݿø��մϴ�.
    /// </summary>
    private Vector3 GetSnappedPosition(Vector3 originalPosition)
    {
        float snappedX = Mathf.Round(originalPosition.x);
        float snappedY = Mathf.Round(originalPosition.y);
        // Z��ǥ�� 2D ���ӿ��� �߿����� �����Ƿ� 0���� �����ϴ� ���� �����ϴ�.
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