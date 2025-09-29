using UnityEngine;

// �� ��ũ��Ʈ�� ����� �뵵�θ� ����ϰ�, ������ �ذ�Ǹ� �����ص� �˴ϴ�.
public class AlphaWatcher : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color lastColor;

    void Awake()
    {
        // �� ������Ʈ�� SpriteRenderer�� �����ɴϴ�.
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            lastColor = sr.color;
        }
    }

    void Update()
    {
        // ���� ���� ������ ���� �������� ����� �ٸ��ٸ� (������ ���� �ٲ�ٸ�)
        if (sr != null && sr.color != lastColor)
        {
            // Ư�� ���� ���� 0���� �ٲ���ٸ�
            if (sr.color.a < 1.0f)
            {
                Debug.LogError("���� ���� " + sr.color.a + "�� ����Ǿ����ϴ�! ������ ã�� ���� �����͸� �Ͻ������մϴ�.", this.gameObject);

                // ���� �̰� �ٽ��Դϴ�! ����
                // �����͸� ��� �Ͻ��������Ѽ�, � �ڵ� ������ ����Ǿ����� Ȯ���� �ð��� �ݴϴ�.
                Debug.Break();
            }

            // ���� �����Ӱ� ���ϱ� ���� ���� ������ �����մϴ�.
            lastColor = sr.color;
        }
    }
}