using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField, Min(0f)] float moveSpeed = 5f;
    [SerializeField] Animator anim;

    [Header("Filters")]
    [SerializeField, Range(0f, 0.3f)] float deadZone = 0.05f;   // 이하면 Idle
    [SerializeField, Range(0f, 0.5f)] float snapHysteresis = 0.12f; // 대각선에서 기존 방향 유지 폭

    Vector2 lastCardinal = Vector2.down; // Idle 방향 유지용

    void Update()
    {
        // 1) 입력 합성 (대각선 속도 보정)
        float x = (Input.GetKey(KeyCode.RightArrow) ? 1f : 0f) - (Input.GetKey(KeyCode.LeftArrow) ? 1f : 0f);
        float y = (Input.GetKey(KeyCode.UpArrow) ? 1f : 0f) - (Input.GetKey(KeyCode.DownArrow) ? 1f : 0f);
        Vector2 input = new Vector2(x, y);
        if (input.sqrMagnitude > 1f) input.Normalize();

        bool isMoving = input.magnitude > deadZone;

        // 2) 4방향 스냅(지배축 + 히스테리시스)
        Vector2 cardinal = lastCardinal;
        if (isMoving)
        {
            float ax = Mathf.Abs(input.x);
            float ay = Mathf.Abs(input.y);
            if (ax >= ay + snapHysteresis) cardinal = new Vector2(Mathf.Sign(input.x), 0f);
            else if (ay >= ax + snapHysteresis) cardinal = new Vector2(0f, Mathf.Sign(input.y));
            // 거의 대각선이면 이전 방향 유지

            lastCardinal = cardinal;

            // 실제 이동은 원본 입력으로 부드럽게
            Vector3 delta = new Vector3(input.x, input.y, 0f) * moveSpeed * Time.deltaTime;
            transform.Translate(delta, Space.World);
        }

        // 3) 애니메이터 파라미터(전이 전용: ±1 또는 0만 들어감)
        anim.SetBool("isMoving", isMoving);
        anim.SetFloat("dirX", isMoving ? cardinal.x : 0f);
        anim.SetFloat("dirY", isMoving ? cardinal.y : 0f);
        anim.SetFloat("lastX", lastCardinal.x);
        anim.SetFloat("lastY", lastCardinal.y);
    }
}
