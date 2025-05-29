using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Running")]
    public float m_Speed = 5f;

    [Header("Jumping")]
    public float m_JumpForce = 15f;
    public float m_GravityScale = 5f;
    public float m_FallMultiplier = 2.5f;
    public float m_LowJumpMultiplier = 2f;
    public bool m_CanMove = true;
    public Transform m_GroundCheck;
    public LayerMask m_GroundLayer;
    public LayerMask m_WallLayer;

    [Header("Wall Sliding")]
    public float m_WallSlideSpeed = 2f;  
    public float m_WallSlideGravity = 1f; 
    public float m_WallJumpHorizontalForce = 8f;
    public float m_WallJumpVerticalForce = 15f;
    public bool IsWallSliding = false;

    [Header("Roll")]
    public bool m_IsRolling = false;

    [Header("Components")]
    public Rigidbody2D m_Body;

    private float GroundCheckRadius = 0.1f;
    private float MoveInput;
    private float WallJumpCooldown = 1.1f;
    private float HorizontalInput;
    private float WallDirection;

    private bool JustWallJumped = false;
    private bool WasGroundedLastFrame = false;
    private bool WasWallSlidingLastFrame = false;

    private Animator Anim;
    private BoxCollider2D BoxCollider;

    private void Awake()
    {
        m_Body.constraints = RigidbodyConstraints2D.FreezeRotation;
        BoxCollider = GetComponent<BoxCollider2D>();
        Anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!m_CanMove || Anim.GetBool("IsAttacking")) return;

        if (m_IsRolling) return;

        MoveInput = Input.GetAxis("Horizontal");
        IsWallSliding = OnWall() && !IsGrounded() && m_Body.linearVelocity.y < 0;
        
        Anim.SetBool("IsWallSliding", IsWallSliding);
        Anim.SetBool("IsGrounded", IsGrounded());
        Anim.SetBool("IsRunning", MoveInput != 0);

        if (IsGrounded())
        {
            WallJumpCooldown = 1.1f;
            JustWallJumped = false;
        }

        if (IsWallSliding)
        {
            m_Body.linearVelocity = new Vector2(0, Mathf.Max(m_Body.linearVelocity.y, -m_WallSlideSpeed));
            m_Body.gravityScale = m_WallSlideGravity;
        }
        else if (WallJumpCooldown > 1f)
        {
            m_Body.linearVelocity = new Vector2(MoveInput * m_Speed, m_Body.linearVelocity.y);
            m_Body.gravityScale = m_GravityScale;
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

        WasGroundedLastFrame = IsGrounded();
        WasWallSlidingLastFrame = IsWallSliding;
    }

    private void UpdatePlayerFacing() 
    {
        if (IsWallSliding || JustWallJumped){ return; }

        if (MoveInput > 0.01f)
        {
            transform.localScale = new Vector3(5, 5, 1);
        }
        else if (MoveInput < -0.01f)
        {
            transform.localScale = new Vector3(-5, 5, 1);
        }

        if (IsGrounded() && !WasGroundedLastFrame && WasWallSlidingLastFrame && !JustWallJumped)
        {
            float currentDirection = Mathf.Sign(transform.localScale.x);
            transform.localScale = new Vector3(-currentDirection * 5, 5, 1);
        }
    }

    private void UpdateGravity()
    {
        if (m_Body.linearVelocity.y < 0)
        {
            m_Body.gravityScale = m_GravityScale * m_FallMultiplier;
        }
        else if (m_Body.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            m_Body.gravityScale = m_GravityScale * m_LowJumpMultiplier;
        }
        else
        {
            m_Body.gravityScale = m_GravityScale;
        }
    }

    private void HandleGroundJump()
    {
        m_Body.linearVelocity = new Vector2(m_Body.linearVelocity.x, m_JumpForce);
        Anim.SetTrigger("Jump");
    }

    private void HandleWallJump()
    {
        Anim.SetTrigger("WallJump");
    }

    private void OnWallJumpFlip()
    {
        float pushDirection = -Mathf.Sign(transform.localScale.x);
        m_Body.linearVelocity = new Vector2(
            pushDirection * m_WallJumpHorizontalForce,
            m_WallJumpVerticalForce
        );
        transform.localScale = new Vector3(pushDirection * 5, 5, 1);

        Anim.SetBool("IsWallSliding", false);
        WallJumpCooldown = 0;
        JustWallJumped = true;
    }

    private void Jump()
    {
        if (IsGrounded())
        {
            HandleGroundJump();
        }
        else if (IsWallSliding && WallJumpCooldown > 1f)
        {
            HandleWallJump();
        }
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(m_GroundCheck.position, GroundCheckRadius, m_GroundLayer);
    }

    public bool OnWall()
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