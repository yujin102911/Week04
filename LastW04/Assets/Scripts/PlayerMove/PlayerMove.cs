using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField, Min(0f)] float moveSpeed = 5f;
    [SerializeField] Animator anim;

    [Header("Filters")]
    [SerializeField, Range(0f, 0.3f)] float deadZone = 0.05f;   // ���ϸ� Idle
    [SerializeField, Range(0f, 0.5f)] float snapHysteresis = 0.12f; // �밢������ ���� ���� ���� ��

    Vector2 lastCardinal = Vector2.down; // Idle ���� ������

    void Update()
    {
        // 1) �Է� �ռ� (�밢�� �ӵ� ����)
        float x = (Input.GetKey(KeyCode.RightArrow) ? 1f : 0f) - (Input.GetKey(KeyCode.LeftArrow) ? 1f : 0f);
        float y = (Input.GetKey(KeyCode.UpArrow) ? 1f : 0f) - (Input.GetKey(KeyCode.DownArrow) ? 1f : 0f);
        Vector2 input = new Vector2(x, y);
        if (input.sqrMagnitude > 1f) input.Normalize();

        bool isMoving = input.magnitude > deadZone;

        // 2) 4���� ����(������ + �����׸��ý�)
        Vector2 cardinal = lastCardinal;
        if (isMoving)
        {
            float ax = Mathf.Abs(input.x);
            float ay = Mathf.Abs(input.y);
            if (ax >= ay + snapHysteresis) cardinal = new Vector2(Mathf.Sign(input.x), 0f);
            else if (ay >= ax + snapHysteresis) cardinal = new Vector2(0f, Mathf.Sign(input.y));
            // ���� �밢���̸� ���� ���� ����

            lastCardinal = cardinal;

            // ���� �̵��� ���� �Է����� �ε巴��
            Vector3 delta = new Vector3(input.x, input.y, 0f) * moveSpeed * Time.deltaTime;
            transform.Translate(delta, Space.World);
        }

        // 3) �ִϸ����� �Ķ����(���� ����: ��1 �Ǵ� 0�� ��)
        anim.SetBool("isMoving", isMoving);
        anim.SetFloat("dirX", isMoving ? cardinal.x : 0f);
        anim.SetFloat("dirY", isMoving ? cardinal.y : 0f);
        anim.SetFloat("lastX", lastCardinal.x);
        anim.SetFloat("lastY", lastCardinal.y);
    }
}
