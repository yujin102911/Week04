using UnityEngine;

public class AttachPlayerToPlatform : MonoBehaviour
{
    private int playerLayer;
    private int lotusLayer;

    void Awake()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        lotusLayer = LayerMask.NameToLayer("LotusPad");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ���� �� ������ �����մϴ�! ����
        // ���� ������Ʈ�� �±װ� "Player" �̰ų� "Box" ���,
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            // ������Ʈ�� �� �÷���(����)�� �ڽ����� ����ϴ�.
            other.transform.SetParent(this.transform);

            // ���� �߰��� ���ǹ�! ����
            // ���� ���� ���� "Player"�� ��쿡�� ���̾ �����մϴ�.
            // (�ڽ��� ���̾ �ٲ� �ʿ䰡 �����Ƿ�)
            if (other.CompareTag("Player"))
            {
                other.gameObject.layer = lotusLayer;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // ���� ���⵵ �Ȱ��� �����մϴ�! ����
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            // ������Ʈ�� �θ�-�ڽ� ���踦 �����մϴ�.
            other.transform.SetParent(null);

            // ���� �߰��� ���ǹ�! ����
            // ������ ���� "Player"�� ��쿡�� ���̾ ������� �ǵ����ϴ�.
            if (other.CompareTag("Player"))
            {
                other.gameObject.layer = playerLayer;
            }
        }
    }
}