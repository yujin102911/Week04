using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    // �÷��̾ ���� ���� �ö���� �� ȣ��˴ϴ�.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �浹�� ������Ʈ�� 'Player' �±׸� ������ �ִ��� Ȯ���մϴ�.
        if (collision.gameObject.CompareTag("Player"))
        {
            // �÷��̾ ������ '����'���� �浹�ߴ��� Ȯ���մϴ�.
            // �浹 ������ ����(normal) ������ y���� �����̸� ������ �浹�� ���Դϴ�.
            ContactPoint2D contact = collision.GetContact(0);
            if (contact.normal.y < -0.5f)
            {
                // �÷��̾ �� ����(transform)�� �ڽ����� ����ϴ�.
                collision.transform.SetParent(this.transform);
            }
        }
    }

    // �÷��̾ ���ǿ��� �������� �� ȣ��˴ϴ�.
    private void OnCollisionExit2D(Collision2D collision)
    {
        // �浹�� ���� ������Ʈ�� 'Player' �±׸� ������ �ִ��� Ȯ���մϴ�.
        if (collision.gameObject.CompareTag("Player"))
        {
            // �÷��̾��� �θ�-�ڽ� ���踦 �����Ͽ� �ٽ� ������ �ֻ������ �ű�ϴ�.
            collision.transform.SetParent(null);
        }
    }
}