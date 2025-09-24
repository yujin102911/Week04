using UnityEngine;

public class HandleAttachment : MonoBehaviour
{

    // �ٸ� �ݶ��̴�(Trigger)�� �� ������Ʈ�� Ʈ���� �������� ������ �� �� �� ȣ��˴ϴ�.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // ���� ������Ʈ�� 'Player' �±׸� ������ �ִ��� Ȯ���մϴ�.
        if (other.CompareTag("Player"))
        {
            Debug.Log("�÷��̾ �ڵ� ������ ����. �θ�� �����մϴ�.");
            // �÷��̾�(other)�� �θ� �� �ڵ�(this.transform)�� �����մϴ�.
            other.transform.SetParent(this.transform);
        }
    }

    // �ٸ� �ݶ��̴�(Trigger)�� �� ������Ʈ�� Ʈ���� �������� ������ �� �� �� ȣ��˴ϴ�.
    private void OnTriggerExit2D(Collider2D other)
    {
        // ���� ������Ʈ�� 'Player' �±׸� ������ �ִ��� Ȯ���մϴ�.
        if (other.CompareTag("Player"))
        {
            Debug.Log("�÷��̾ �ڵ� ������ ��Ż. �θ�-�ڽ� ���踦 �����մϴ�.");
            // �÷��̾��� �θ�-�ڽ� ���踦 �����Ͽ� ������ �ֻ��� �������� ���������ϴ�.
            other.transform.SetParent(null);
        }
    }
}