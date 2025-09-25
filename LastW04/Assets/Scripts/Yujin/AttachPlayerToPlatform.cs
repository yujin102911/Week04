using UnityEngine;

public class AttachPlayerToPlatform : MonoBehaviour
{
    private int playerLayer; // 'Player' ���̾��� ��ȣ�� ������ ����
    private int lotusLayer;  // 'Lotus' ���̾��� ��ȣ�� ������ ����

    void Awake()
    {
        // �̸����� ���̾� ��ȣ�� ã�� �����صӴϴ�.
        // �̷��� �ϸ� ���߿� ���̾� ������ �ٲ� �ڵ带 ������ �ʿ䰡 �����ϴ�.
        playerLayer = LayerMask.NameToLayer("Player");
        lotusLayer = LayerMask.NameToLayer("Lotus");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ���� �� ���� �߰��ϼ���! ����
            // �÷��̾��� ���̾ 'Lotus' ���̾�� ��� �����մϴ�.
            other.gameObject.layer = lotusLayer;

            // �÷��̾ �� �÷���(����)�� �ڽ����� ����ϴ�.
            other.transform.SetParent(this.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ���� �� ���� �߰��ϼ���! ����
            // �÷��̾��� ���̾ ���� 'Player' ���̾�� �ǵ����ϴ�.
            other.gameObject.layer = playerLayer;

            // �÷��̾��� �θ�-�ڽ� ���踦 �����մϴ�.
            other.transform.SetParent(null);
        }
    }
}