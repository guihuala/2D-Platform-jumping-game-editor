using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float jumpControl = 0.5f; // 空中控制力度
    public float fallMultiplier = 2.5f; // 下落时的重力倍数
    public float lowJumpMultiplier = 2f; // 轻微跳跃时的重力倍数

    private Rigidbody2D rb;
    private bool isGrounded;
    private int jumpCount;
    private float horizontalInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpCount = 0;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        // 检测跳跃输入（包括二段跳）
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && jumpCount < 2)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        Move();
        ApplyBetterJumpPhysics();
    }

    private void Move()
    {
        float targetSpeed = horizontalInput * moveSpeed;
        float smoothedSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, Time.fixedDeltaTime * 10);
        rb.velocity = new Vector2(smoothedSpeed, rb.velocity.y);

        if (isGrounded)
        {
            jumpCount = 0;
        }

        if (horizontalInput != 0)
        {
            transform.localScale = new Vector3(-Mathf.Sign(horizontalInput), 1, 1);
        }

        if (!isGrounded && jumpCount > 0)
        {
            rb.velocity += new Vector2(horizontalInput * jumpControl, 0);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0); // 清除当前垂直速度
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        jumpCount++;
    }

    private void ApplyBetterJumpPhysics()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.UpArrow))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 使用标签检测是否是地面
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // 使用标签检测是否离开地面
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}

