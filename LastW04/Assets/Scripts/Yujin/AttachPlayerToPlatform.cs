using UnityEngine;
using System.Collections.Generic; // Dictionary�� ����ϱ� ���� �߰�

public class AttachPlayerToPlatform : MonoBehaviour
{
    private int lotusLayer; // 'Lotus' ���̾��� ��ȣ�� ������ ����

    // ���� ��ü(�÷��̾�, �ڽ� ��)�� ���� ���̾ �����ϱ� ���� Dictionary
    // Key: Ʈ���ſ� ���� ��ü�� Collider2D
    // Value: �ش� ��ü�� ���� ���̾� ��ȣ
    private Dictionary<Collider2D, int> originalLayers = new Dictionary<Collider2D, int>();

    void Awake()
    {
        // �̸����� "Lotus" ���̾� ��ȣ�� ã�� �����մϴ�.
        // �� ���̾�� Physics 2D �������� 'Water'�� �浹���� �ʵ��� �����Ǿ�� �մϴ�.
        lotusLayer = LayerMask.NameToLayer("Lotus");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // �÷��̾� �Ǵ� �ڽ��� ������ ��
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            if (originalLayers.ContainsKey(other)) return;

            originalLayers.Add(other, other.gameObject.layer);
            other.gameObject.layer = lotusLayer;
            other.transform.SetParent(this.transform);

            // ���� �߰��� �κ� ����
            // ���� ���� ���� �÷��̾���, PlayerMove ��ũ��Ʈ���� �÷����� �ö����ٰ� �˷��ݴϴ�.
            if (other.CompareTag("Player"))
            {
                PlayerMove playerMove = other.GetComponent<PlayerMove>();
                if (playerMove != null)
                {
                    playerMove.IsOnPlatform = true;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // �÷��̾� �Ǵ� �ڽ��� ������ ��
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            if (originalLayers.ContainsKey(other))
            {
                // ���� �߰��� �κ� ����
                // ���� ������ ���� �÷��̾���, PlayerMove ��ũ��Ʈ���� �÷������� ���ȴٰ� �˷��ݴϴ�.
                if (other.CompareTag("Player"))
                {
                    PlayerMove playerMove = other.GetComponent<PlayerMove>();
                    if (playerMove != null)
                    {
                        playerMove.IsOnPlatform = false;
                    }
                }

                other.gameObject.layer = originalLayers[other];
                originalLayers.Remove(other);
                other.transform.SetParent(null);
            }
        }
    }
}

