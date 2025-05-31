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
    
    public LayerMask m_GroundLayer;
    public LayerMask m_WallLayer;

    [Header("Wall Sliding")]
    public float m_WallSlideSpeed = 2f;  
    public float m_WallSlideGravity = 1f; 
    public float m_WallJumpHorizontalForce = 8f;
    public float m_WallJumpVerticalForce = 15f;

    private float GroundCheckRadius = 0.1f;
    private float MoveInput;
    private float WallJumpCooldown = 1.1f;
    private float HorizontalInput;
    private float WallDirection;

    private bool JustWallJumped = false;
    private bool WasGroundedLastFrame = false;
    private bool WasWallSlidingLastFrame = false;

    private float _fallSpeedYDampingChangeThreshold;

    private Rigidbody2D Body;
    private SpriteRenderer SpriteRenderer;
    private GameObject PlayerObject;
    private Transform GroundCheck;
    private Animator Anim;
    private BoxCollider2D BoxCollider;

    void Awake()
    {
        Body = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        BoxCollider = GetComponent<BoxCollider2D>();
        Anim = GetComponent<Animator>();
        PlayerObject = GameObject.Find("Player");
        GroundCheck = gameObject.transform.Find("GroundCheck");

        
    }
    void Start()
    {
        Body.constraints = RigidbodyConstraints2D.FreezeRotation;
        Anim.SetBool("CanMove", true);
        //Time.timeScale = 0.5f;

        _fallSpeedYDampingChangeThreshold = CameraManager.Instance.FallSpeedYDampingChangeThreshold;
    }

    void Update()
    {
        Anim.SetBool("IsGrounded", IsGrounded());
        

        if (!Anim.GetBool("CanMove") || 
            Anim.GetBool("IsAttacking") || 
            Anim.GetBool("IsRolling")) return;

        MoveInput = Input.GetAxis("Horizontal");
        
        Anim.SetBool("IsRunning", MoveInput != 0);
        Anim.SetBool("IsWallSliding", OnWall() && !IsGrounded() && Body.linearVelocity.y < 0);


        if (IsGrounded())
        {
            WallJumpCooldown = 1.1f;
            JustWallJumped = false;
        }

        if (Anim.GetBool("IsWallSliding"))
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
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        else
        {
            WallJumpCooldown += Time.deltaTime;
        }

        UpdateGravity();

        WasGroundedLastFrame = IsGrounded();
        WasWallSlidingLastFrame = Anim.GetBool("IsWallSliding");

        if (Body.linearVelocity.y < _fallSpeedYDampingChangeThreshold &&
        !CameraManager.Instance.IsLerpingYDamping &&
        !CameraManager.Instance.LerpredFromPlayerFalling)
        {
            CameraManager.Instance.LerpYDamping(true);
        }

        if (Body.linearVelocity.y >= 0f &&
            !CameraManager.Instance.IsLerpingYDamping &&
            CameraManager.Instance.LerpredFromPlayerFalling)
        {
            CameraManager.Instance.LerpredFromPlayerFalling = false;
            CameraManager.Instance.LerpYDamping(false);
        }
    }

    private void Flip(float Value)
    {
        if (Value > 0.01f)
        {
            LeanTween.rotateY(PlayerObject, 0, 0).setEaseInOutSine();

        }
        else if (Value < -0.01f)
        {
            LeanTween.rotateY(PlayerObject, 180, 0).setEaseInOutSine();
        }
    }

    private void UpdatePlayerFacing() 
    {
        if (Anim.GetBool("IsWallSliding") || JustWallJumped){ return; }

        Flip(MoveInput);

        if (IsGrounded() && !WasGroundedLastFrame && WasWallSlidingLastFrame && !JustWallJumped)
        {
            Flip(-GetFacingDirection());
        }
    }

    private void UpdateGravity()
    {
        if (Body.linearVelocity.y < -0.1f)
        {
            Body.gravityScale = m_GravityScale * m_FallMultiplier;
        }
        else if (Body.linearVelocity.y > 0.1f && !Input.GetKey(KeyCode.Space))
        {
            Body.gravityScale = m_GravityScale * m_LowJumpMultiplier;
        }
        else
        {
            Body.gravityScale = m_GravityScale;
        }
    }

    private void HandleGroundJump()
    {
        Body.linearVelocity = new Vector2(Body.linearVelocity.x, m_JumpForce);
        Anim.SetTrigger("Jump");
    }

    private void HandleWallJump()
    {
        Anim.SetTrigger("WallJump");
    }

    private void OnWallJumpFlip()
    {
        float pushDirection = -Mathf.Sign(transform.localScale.x);
        Body.linearVelocity = new Vector2(
            pushDirection * m_WallJumpHorizontalForce,
            m_WallJumpVerticalForce
        );

        Flip(pushDirection);

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
        else if (Anim.GetBool("IsWallSliding") && WallJumpCooldown > 1f)
        {
            HandleWallJump();
        }
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(GroundCheck.position, GroundCheckRadius, m_GroundLayer);
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

    public int GetFacingDirection()
    {
        return Mathf.Abs(transform.eulerAngles.y - 180) < 0.1f ? -1 : 1;
    }
}