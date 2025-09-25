using UnityEngine;

public class DestroyOnRightClick : MonoBehaviour
{

    // ���콺 Ŀ���� �� ������Ʈ�� �ݶ��̴� ���� �ִ� ���� �� ������ ȣ��˴ϴ�.
    private void OnMouseOver()
    {
        // ���� ���콺 ������ ��ư�� "������ ����"�̶��
        if (Input.GetMouseButtonDown(1))
        {
            // �� ������Ʈ�� �θ� �������� WorldSpaceSlider ������Ʈ�� ã���ϴ�.
            WorldSpaceSlider rootSlider = GetComponentInParent<WorldSpaceSlider>();

            // ã�Ҵٸ�, �� ������Ʈ�� �پ��ִ� �ֻ��� ���� ������Ʈ�� �ı��մϴ�.
            if (rootSlider != null)
            {
                Destroy(rootSlider.gameObject);
            }
        }
    }
}