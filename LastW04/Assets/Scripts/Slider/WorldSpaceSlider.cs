using UnityEngine;

public class WorldSpaceSlider : MonoBehaviour
{
    [Header("������Ʈ ����")]
    [SerializeField] private Transform handle;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private Transform body;

    [Header("�����̴� ��")]
    [Range(0, 1)]
    [SerializeField] private float value = 0.5f; // 0.0 ~ 1.0 ������ ��

    public bool IsInstalled { get; set; } = false;

    void OnValidate()
    {
        // �ν����Ϳ��� value ���� ������ �� �ǽð����� �ڵ� ��ġ�� ������Ʈ�մϴ�.
        UpdateHandlePosition();
    }

    // �ڵ��� ��ġ�� ������� value ���� ������Ʈ�ϴ� �Լ� (SliderHandle�� ȣ��)
    public void UpdateValueFromHandlePosition(Vector3 handleWorldPos)
    {
        Vector3 trackDirection = endPoint.position - startPoint.position;

        // ���� �� ���� ó�� �ڵ带 �߰��ϼ���! ����
        // �����̴��� ���̰� 0�� ������ ������ �����ϱ� ���� ����� �ߴ��մϴ�.
        if (trackDirection.magnitude < 0.01f)
        {
            SetValue(0); // ���̸� 0���� �����ϰų� �׳� return �ص� �˴ϴ�.
            return;
        }
        // ���� ������� �߰� ����

        Vector3 handleDirection = handleWorldPos - startPoint.position;
        Vector3 projectedVector = Vector3.Project(handleDirection, trackDirection);
        float newValue = projectedVector.magnitude / trackDirection.magnitude;

        if (Vector3.Dot(projectedVector, trackDirection) < 0)
        {
            newValue = 0;
        }

        SetValue(newValue);
    }

    // �ܺο��� value ���� �����ϴ� �Լ�
    public void SetValue(float newValue)
    {
        // ���� 0�� 1 ���̷� �����մϴ�.
        value = Mathf.Clamp01(newValue);
        UpdateHandlePosition();
    }

    // ���� value ���� ���� �ڵ��� ��ġ�� ������Ʈ�մϴ�.
    private void UpdateHandlePosition()
    {
        if (handle != null && startPoint != null && endPoint != null)
        {
            // Lerp �Լ��� ����Ͽ� �������� ���� ������ ��Ȯ�� ��ġ�� ����մϴ�.
            handle.position = Vector3.Lerp(startPoint.position, endPoint.position, value);
        }
    }

    // ObjectPlacer�� ȣ���� ��ġ �Լ� (���� PowerConduit.cs�� Setup ����)
    public void Setup(Vector3 startPos, Vector3 endPos)
    {
        Vector3 direction = endPos - startPos;
        float distance = direction.magnitude;

        transform.position = startPos;

        // ���� �� �κ��� �����ϼ���! ����
        // �Ÿ��� 0�� ������(�̸����� ����) ������ �⺻������ �����Ͽ� ���� ����
        if (distance < 0.01f)
        {
            transform.right = Vector3.right; // �⺻������ �������� ������ ����
        }
        else
        {
            transform.right = direction.normalized;
        }
        // ���� ������� ���� ����

        body.localScale = new Vector3(distance, body.localScale.y, body.localScale.z);
        endPoint.localPosition = new Vector3(distance, 0, 0);
        SetValue(1.0f);
    }
}