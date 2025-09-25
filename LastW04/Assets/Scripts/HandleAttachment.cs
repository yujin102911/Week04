using UnityEngine;

public class HandleAttachment : MonoBehaviour
{
    // �ڽ��� Ʈ���� ������ ������ ��
    private void OnTriggerEnter2D(Collider2D other)
    {
        // ���� ���� 'Box' �±׸� �����ٸ�
        if (other.CompareTag("Box"))
        {
            // ������ �ڵ��� �ڽ����� ����ϴ�.
            other.transform.SetParent(this.transform);
        }
    }

    // �ڽ��� Ʈ���� �������� ������ ��
    private void OnTriggerExit2D(Collider2D other)
    {
        // ���� ���� 'Box' �±׸� �����ٸ�
        if (other.CompareTag("Box"))
        {
            // ������ �θ�-�ڽ� ���踦 �����մϴ�.
            other.transform.SetParent(null);
        }
    }
}