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
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            other.transform.SetParent(this.transform);

            if (other.CompareTag("Player"))
            {
                other.gameObject.layer = lotusLayer;
            }
            // ���� [�߰���] �ڽ��� ������, �ڽ��� ���¸� '���� ��'�� ���� ����
            else if (other.CompareTag("Box"))
            {
                if (other.TryGetComponent(out PushableBox box))
                {
                    box.SetOnLotus(true);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            other.transform.SetParent(null);

            if (other.CompareTag("Player"))
            {
                other.gameObject.layer = playerLayer;
            }
            // ���� [�߰���] �ڽ��� ������, �ڽ��� ���¸� '���� �� �ƴ�'���� ���� ����
            else if (other.CompareTag("Box"))
            {
                if (other.TryGetComponent(out PushableBox box))
                {
                    box.SetOnLotus(false);
                }
            }
        }
    }
}