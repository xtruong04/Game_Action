using UnityEngine;

public class Player : MonoBehaviour
{
    // Thêm tốc độ chạy
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f; // Tốc độ khi giữ Shift

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        MovePlayer();
        HandleAttackInput(); // <--- GỌI HÀM XỬ LÝ TẤN CÔNG Ở ĐÂY
    }

    void MovePlayer()
    {
        Vector2 playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // --- 1. Xử lý Input và Tốc độ ---
        bool isMoving = playerInput.magnitude > 0.1f; // Dùng 0.1f để tránh lỗi float nhỏ
        bool isRunningInput = Input.GetKey(KeyCode.LeftShift);

        float currentMoveSpeed = walkSpeed;
        bool isCurrentlyRunning = false;

        // Xác định Tốc độ
        if (isRunningInput && isMoving)
        {
            // Nếu nhấn Shift VÀ đang di chuyển
            currentMoveSpeed = runSpeed;
            isCurrentlyRunning = true;
        }
        else if (isMoving)
        {
            // Chỉ đi bộ
            currentMoveSpeed = walkSpeed;
        }

        // --- 2. Di chuyển nhân vật ---
        rb.linearVelocity = playerInput.normalized * currentMoveSpeed;

        // --- 3. Lật nhân vật (Flip) ---
        if (playerInput.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (playerInput.x > 0)
        {
            spriteRenderer.flipX = false;
        }

        // --- 4. Cập nhật Animator ---
        animator.SetBool("isWalk", isMoving);
        animator.SetBool("isRun", isCurrentlyRunning);
    }

    // Hàm mới để xử lý Input Tấn công
    void HandleAttackInput()
    {
        // Kiểm tra nếu người chơi nhấn nút Tấn công (Ví dụ: Chuột trái)
        if (Input.GetMouseButtonDown(0))
        {
            // Kích hoạt duy nhất một Trigger "Attack".
            // Animator sẽ tự động chọn Transition (Idle->Attack, Walk->Walk Attack, Run->Run Attack)
            // dựa trên trạng thái hiện tại của nhân vật.
            animator.SetTrigger("Attack");
        }
    }
}