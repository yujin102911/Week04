using UnityEngine;

// 이 스크립트는 Rigidbody 2D 컴포넌트가 반드시 필요합니다.
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    // [SerializeField]를 사용하면 private 변수여도 인스펙터 창에서 값을 수정할 수 있습니다.
    [SerializeField]
    private float moveSpeed = 5f; // 플레이어의 이동 속도

    private Rigidbody2D rb; // Rigidbody 2D 컴포넌트를 담을 변수
    private Vector2 moveInput; // 사용자의 입력(WASD) 값을 저장할 변수

    // 게임이 시작될 때 한 번 호출됩니다.
    void Start()
    {
        // 이 스크립트가 붙어있는 게임 오브젝트의 Rigidbody 2D 컴포넌트를 가져와서 rb 변수에 할당합니다.
        rb = GetComponent<Rigidbody2D>();
    }

    // 매 프레임마다 호출됩니다. 입력 값을 받기에 적합합니다.
    void Update()
    {
        // 수평(A, D, ←, →) 및 수직(W, S, ↑, ↓) 입력 값을 받습니다.
        // 입력 값은 -1.0f에서 1.0f 사이의 값을 가집니다.
        float moveX = Input.GetAxisRaw("Horizontal"); // Horizontal 축
        float moveY = Input.GetAxisRaw("Vertical"); // Vertical 축

        // 입력 값을 Vector2 형태로 저장하고, 정규화(Normalize)하여 대각선 이동 시 속도가 빨라지는 것을 방지합니다.
        moveInput = new Vector2(moveX, moveY).normalized;
    }

    // 고정된 시간 간격으로 호출됩니다. 물리 계산을 하기에 적합합니다.
    void FixedUpdate()
    {
        // Rigidbody의 속도(velocity)를 변경하여 플레이어를 움직입니다.
        // moveInput 값에 이동 속도(moveSpeed)와 물리 시간(Time.fixedDeltaTime)을 곱해줍니다.
        // Time.fixedDeltaTime을 곱해주면 프레임 속도에 관계없이 일정한 속도로 움직입니다.
        rb.linearVelocity = moveInput * moveSpeed;
    }
}