using UnityEngine;

public class PowerConduit : MonoBehaviour
{
    [SerializeField] private Transform body; // �ν����Ϳ��� Body �ڽ� ������Ʈ�� ����

    /// <summary>
    /// �������� ������ �������� ���� ������ �����մϴ�.
    /// </summary>
    public void Setup(Vector3 startPosition, Vector3 endPosition)
    {
        // �� �� ������ ����� �Ÿ��� ���
        Vector3 direction = endPosition - startPosition;
        float distance = direction.magnitude;

        // ��ġ�� �� ���� �߰����� ����
        transform.position = startPosition;

        // Body�� �������� �Ÿ���ŭ �÷� ���̸� ǥ�� (Y, Z �������� 1�� ����)
        body.localScale = new Vector3(distance, body.localScale.y, body.localScale.z);

        // ���������� ������ �ٶ󺸵��� ������ ����
        transform.right = direction.normalized;
    }
}