// WaterZone.cs
using UnityEngine;

public class WaterZone : MonoBehaviour
{
    // �ٸ� �ݶ��̴��� �� Ʈ���� �������� '������ ��' ȣ��˴ϴ�.
    private void OnTriggerExit2D(Collider2D other)
    {
        // ���� ������Ʈ�� �±װ� "Lotus" ���
        if (other.CompareTag("Lotus"))
        {
            // �ش� ���� ������Ʈ�� �ı��մϴ�.
            Destroy(other.gameObject);
        }
    }
}