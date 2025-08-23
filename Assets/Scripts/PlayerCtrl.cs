using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody2D))]
public class Player2DController : MonoBehaviour
{
    [Header("References")]
    public AbilityManager abilities;        // 해금 여부 조회
    [SerializeField] Transform groundCheck; // 발밑 체크 포인트
    [SerializeField] LayerMask groundMask;  // Ground 레이어

    [Header("Tuning")]
    [SerializeField] float speed = 6f;            // set speed=n 으로 조절
    [SerializeField] float moveDuration = 0.6f;   // move 유지 시간
    [SerializeField] float jumpForce = 5f;        // 점프 임펄스
    [SerializeField] float airControl = 0.5f;     // 공중 수평 제어 비율
    [SerializeField] float coyoteTime = 0.15f;    // 지상 이탈 후 관용 시간
    [SerializeField] Vector2 groundBox = new(0.4f, 0.1f); // 바닥 판정 박스

    Rigidbody2D rb;
    float dirX;         // -1/0/1
    float moveTime;     // 남은 이동 시간
    bool isGrounded;
    float lastGroundedTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!groundCheck) Debug.LogWarning("groundCheck가 비어있음. 발밑 빈 오브젝트를 연결하세요.");
    }

    public void SetSpeed(float s) => speed = Mathf.Clamp(s, 2f, 12f);

    void FixedUpdate()
    {
        // 콘솔에 포커스가 있으면 게임 입력을 무시(타이핑이 이동으로 먹히는 것 방지)
        bool uiFocused = EventSystem.current && EventSystem.current.currentSelectedGameObject != null;

        // 1) 지상 체크(짧은 Overlap 박스)
        if (groundCheck)
        {
            isGrounded = Physics2D.OverlapBox(groundCheck.position, groundBox, 0f, groundMask);
            if (isGrounded) lastGroundedTime = Time.time;
        }

        // 2) 입력 읽기(항상 읽되, 적용은 해금 여부에 따름)
        float inputX = uiFocused ? 0f : Input.GetAxisRaw("Horizontal"); // -1,0,1
        bool jumpPressed = uiFocused ? false : Input.GetKeyDown(KeyCode.Space);

        // 3) move 상태 갱신(해금된 경우에만)
        if (abilities && abilities.MoveHorizontal)
        {
            if (Mathf.Abs(inputX) > 0.01f)
            {
                dirX = Mathf.Sign(inputX);
                moveTime = moveDuration; // 새 입력으로 갱신(덮어쓰기)
            }
        }
        else
        {
            dirX = 0f;
            moveTime = 0f;
        }

        // 4) 수평 속도 결정(여러 프레임에 걸쳐 유지)
        float vx = rb.velocity.x;
        if (moveTime > 0f && Mathf.Abs(dirX) > 0.01f)
        {
            float factor = isGrounded ? 1f : airControl;
            vx = dirX * speed * factor;
            moveTime -= Time.fixedDeltaTime;
        }
        else
        {
            vx = 0f; // 단순 감속(필요하면 Mathf.MoveTowards로 부드럽게)
        }

        // 5) 점프(해금 + 지상/코요테 타임)
        bool canJump = abilities && abilities.Jump &&
                       (isGrounded || (Time.time - lastGroundedTime) <= coyoteTime);

        float vy = rb.velocity.y;
        if (canJump && jumpPressed)
        {
            // 왜: 직전 수직 속도 잔여값을 제거해 일관된 점프 높이 보장
            vy = 0f;
            rb.velocity = new Vector2(vx, vy);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        else
        {
            rb.velocity = new Vector2(vx, vy);
        }
    }

    // 에디터에서 바닥 판정 박스 확인용
    void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheck.position, groundBox);
    }
}
