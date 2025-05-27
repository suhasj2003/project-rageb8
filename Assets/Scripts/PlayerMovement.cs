using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float m_Speed = 5f;
    public static float m_JumpForce = 15f;
    public float m_GravityScale = 5f;
    public float m_FallMultiplier = 2.5f;
    public float m_LowJumpMultiplier = 2f;
    public bool m_CanMove = true;
    public Transform m_GroundCheck;
    public LayerMask m_GroundLayer;
    public LayerMask m_WallLayer;

    public float m_WallSlideSpeed = 2f;  
    public float m_WallSlideGravity = 1f; 
    public float m_WallJumpHorizontalForce = 5f;
    public float m_WallJumpVerticalForce = m_JumpForce * 0.8f;

    private bool m_IsWallSliding = false; 
    private float GroundCheckRadius = 0.1f;
    private float MoveInput;
    private float WallJumpCooldown = 1.1f;
    float WallDirection;
    private float HorizontalInput;

    private Rigidbody2D Body;
    private Animator Anim;
    private BoxCollider2D BoxCollider;

    private void Awake()
    {
        Body = GetComponent<Rigidbody2D>();
        Body.constraints = RigidbodyConstraints2D.FreezeRotation;
        BoxCollider = GetComponent<BoxCollider2D>();
        Anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!m_CanMove) return;

        MoveInput = Input.GetAxis("Horizontal");
        m_IsWallSliding = OnWall() && !IsGrounded() && Body.linearVelocity.y < 0;

        if (IsGrounded())
        {
            WallJumpCooldown = 1.1f;
        }

        if (m_IsWallSliding)
        {
            Body.linearVelocity = new Vector2(0, Mathf.Max(Body.linearVelocity.y, -m_WallSlideSpeed));
            Body.gravityScale = m_WallSlideGravity;
        }
        else if (WallJumpCooldown > 1f)
        {
            Body.linearVelocity = new Vector2(MoveInput * m_Speed, Body.linearVelocity.y);
            Body.gravityScale = m_GravityScale;
        }

        UpdatePlayerFacing();

        if (Input.GetKey(KeyCode.Space))
        {
            Jump();
        }
        else
        {
            WallJumpCooldown += Time.deltaTime;
        }

        UpdateGravity();

        Anim.SetBool("Run", MoveInput != 0);
        Anim.SetBool("Grounded", IsGrounded());
    }

    private void UpdatePlayerFacing()
    {
        if (MoveInput > 0.01f)
        {
            transform.localScale = new Vector3(5, 5, 1);
        }
        else if (MoveInput < -0.01f)
        {
            transform.localScale = new Vector3(-5, 5, 1);
        }
    }

    private void UpdateGravity()
    {
        if (Body.linearVelocity.y < 0)
        {
            Body.gravityScale = m_GravityScale * m_FallMultiplier;
        }
        else if (Body.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            Body.gravityScale = m_GravityScale * m_LowJumpMultiplier;
        }
        else
        {
            Body.gravityScale = m_GravityScale;
        }
    }

    private void Jump()
    {
        if (IsGrounded())
        {
            HandleGroundJump();
        }
        else if (m_IsWallSliding && WallJumpCooldown > 1f)
        {
            HandleWallJump();
        }
    }

    private void HandleGroundJump()
    {
        Body.linearVelocity = new Vector2(Body.linearVelocity.x, m_JumpForce);
        Anim.SetTrigger("Jump");
    }

    private void HandleWallJump()
    {
        float pushDirection = -Mathf.Sign(transform.localScale.x);
        Body.linearVelocity = new Vector2(
            pushDirection * m_WallJumpHorizontalForce,
            m_WallJumpVerticalForce
        );

        FlipPlayerAwayFromWall(pushDirection);
        Anim.SetTrigger("Jump");
        WallJumpCooldown = 0;
    }

    private void FlipPlayerAwayFromWall(float pushDirection)
    {
        transform.localScale = new Vector3(-pushDirection * 5, 5, 1);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(m_GroundCheck.position, GroundCheckRadius, m_GroundLayer);
    }

    private bool OnWall()
    {
        float CastDistance = 0.1f;
        Vector2 CastDirection = new Vector2(transform.localScale.x, 0); 

        RaycastHit2D Hit = Physics2D.BoxCast(
            BoxCollider.bounds.center,
            BoxCollider.bounds.size,
            0f,
            CastDirection,
            CastDistance,
            m_WallLayer
        );
        return Hit.collider != null;
    }
}