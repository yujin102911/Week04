using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private Color previewColor = new Color(0, 1, 0, 0.5f);

    private bool isInstallMode = false;
    private bool isPlacing = false; // ���� �������� �����Ǿ� ������ ��ٸ��� ������?

    private Vector3 startPosition;
    private WorldSpaceSlider previewInstance;

    void Update()
    {
        // 'B' Ű�� ��ġ ��� ON/OFF
        if (Input.GetKeyDown(KeyCode.B))
        {
            isInstallMode = !isInstallMode;
            Debug.Log("��ġ ���: " + isInstallMode);

            // ��ġ ��带 ���� �� �̸����Ⱑ ���������� ��ҽ�Ŵ
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
        // Ÿ�ϸ� �׸��忡 ������ ���콺 ��ġ ��� (���ϴ� ������� ����)
        Vector3 mouseWorldPos = GetSnappedPosition(GetMouseWorldPosition());

        // ���� ��ġ ��(������ ���� ��)�̶��
        if (isPlacing)
        {
            // �̸����� ������Ʈ: �������� ����, ������ ���콺�� ����ٴ�
            Vector3 constrainedMousePos = GetConstrainedMousePosition(startPosition, mouseWorldPos);
            previewInstance.Setup(startPosition, constrainedMousePos);

            // ���콺 ���� ��ư�� �� ��°�� Ŭ������ �� (��ġ �Ϸ�)
            if (Input.GetMouseButtonDown(0))
            {
                FinalizePlacement();
            }
            // ���콺 ������ ��ư�� ������ �� (��ġ ���)
            else if (Input.GetMouseButtonDown(1))
            {
                CancelPlacement();
            }
        }
        // ���� ��ġ ���� ���̶��
        else
        {
            // ���콺 ���� ��ư�� ó�� Ŭ������ �� (��ġ ����)
            if (Input.GetMouseButtonDown(0))
            {
                StartPlacement(mouseWorldPos);
            }
        }
    }

    // ��ġ ����
    private void StartPlacement(Vector3 position)
    {
        isPlacing = true;
        startPosition = position;

        // �̸����� ��ü ����
        GameObject newObject = Instantiate(objectPrefab, startPosition, Quaternion.identity);
        previewInstance = newObject.GetComponent<WorldSpaceSlider>();
        previewInstance.GetComponentInChildren<SpriteRenderer>().color = previewColor;
    }

    // ��ġ Ȯ��
    private void FinalizePlacement()
    {
        isPlacing = false;
        previewInstance.GetComponentInChildren<SpriteRenderer>().color = Color.white;
        previewInstance = null; // ���� ��ġ�� ���� ������ ���
    }

    // ��ġ ���
    private void CancelPlacement()
    {
        isPlacing = false;
        Destroy(previewInstance.gameObject);
        previewInstance = null;
    }

    // ���콺 ���� ��ǥ ���
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane + 10;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    // Ÿ�� �𼭸��� ����
    private Vector3 GetSnappedPosition(Vector3 originalPosition)
    {
        float snappedX = Mathf.Round(originalPosition.x);
        float snappedY = Mathf.Round(originalPosition.y);
        return new Vector3(snappedX, snappedY, 0);
    }

    // 0�� �Ǵ� 90���� ���� ����
    private Vector3 GetConstrainedMousePosition(Vector3 startPos, Vector3 currentPos)
    {
        Vector3 direction = currentPos - startPos;

        // X�� ��ȭ���� ������ Y�� ��ȭ���� ���񰪺��� ũ�� �������� ����
        // (������-���� ������ -45�� ~ 45�� ������ ��)
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            return new Vector3(currentPos.x, startPos.y, startPos.z);
        }
        // �׷��� ������ �������� ����
        // (������ 45~90�� �Ǵ� -45~-90�� ������ ��)
        else
        {
            return new Vector3(startPos.x, currentPos.y, startPos.z);
        }
    }
}