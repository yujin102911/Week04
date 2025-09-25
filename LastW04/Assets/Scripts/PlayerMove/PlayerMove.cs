using UnityEngine;
using UnityEngine.InputSystem; // New Input System

public class PlayerMove : MonoBehaviour
{
    [SerializeField, Min(0f)] float moveSpeed = 5f;
    [SerializeField] Animator anim;

    [Header("Filters")]
    [SerializeField, Range(0f, 0.3f)] float deadZone = 0.05f;
    [SerializeField, Range(0f, 0.5f)] float snapHysteresis = 0.12f;

    Vector2 input;                  // New Input System에서 받은 원본 입력(-1..1)
    Vector2 lastCardinal = Vector2.down;

    // PlayerInput(Behavior=Invoke Unity Events)에서 Move 이벤트에 연결
    public void OnMove(InputValue value)
    {
        input = value.Get<Vector2>();
        if (input.sqrMagnitude > 1f) input = input.normalized; // 패드 대각선 보정
    }

    void Update()
    {
        bool isMoving = input.magnitude > deadZone;

        // 4방향 스냅(지배축+히스테리시스)
        Vector2 cardinal = lastCardinal;
        if (isMoving)
        {
            float ax = Mathf.Abs(input.x);
            float ay = Mathf.Abs(input.y);
            if (ax >= ay + snapHysteresis) cardinal = new Vector2(Mathf.Sign(input.x), 0f);
            else if (ay >= ax + snapHysteresis) cardinal = new Vector2(0f, Mathf.Sign(input.y));
            lastCardinal = cardinal;

            Vector3 delta = new Vector3(input.x, input.y, 0f) * moveSpeed * Time.deltaTime;
            transform.Translate(delta, Space.World);
        }

        // 애니메이터
        anim.SetBool("isMoving", isMoving);
        anim.SetFloat("dirX", isMoving ? cardinal.x : 0f);
        anim.SetFloat("dirY", isMoving ? cardinal.y : 0f);
        anim.SetFloat("lastX", lastCardinal.x);
        anim.SetFloat("lastY", lastCardinal.y);
    }
}
