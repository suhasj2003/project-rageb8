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

    [Header("Float")]
    public float m_FloatGravityScale = 1.5f;
    public float m_MaxFloatTime = 0.5f;

    [Header("Wall Sliding")]
    public float m_WallSlideSpeed = 2f;
    public float m_WallSlideGravity = 1f;
    public float m_WallJumpHorizontalForce = 8f;
    public float m_WallJumpVerticalForce = 15f;

    public float m_MinJumpHeightMultiplier = 0.5f; // Minimum jump height (quick tap)
    public float m_MaxJumpHoldTime = 0.2f; // Max time holding space to reach full jump height

    private float GroundCheckRadius = 0.1f;
    private float MoveInput;
    private float WallJumpCooldown = 0f;
    private float HorizontalInput;
    private float WallDirection;

    private bool JustWallJumped = false;
    private bool WasGroundedLastFrame = false;
    private bool WasWallSlidingLastFrame = false;

    private float FloatTimer;
    private bool IsFloating;

    private float _fallSpeedYDampingChangeThreshold;

    private Rigidbody2D RB;
    private SpriteRenderer SpriteRenderer;
    private GameObject PlayerObject;
    private Transform GroundCheck;
    private Animator Anim;
    private BoxCollider2D BoxCollider;

    void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        BoxCollider = GetComponent<BoxCollider2D>();
        Anim = GetComponent<Animator>();
        PlayerObject = GameObject.Find("Player");
        //GroundCheck = gameObject.transform.Find("GroundCheck");

        GroundCheck = GetOrCreateGroundCheck("GroundCheck", new Vector3(0f, 0f, 0f), Vector3.one);
    }

    private Transform GetOrCreateGroundCheck(string GroundCheckName, Vector3 LocalPosition, Vector3 LocalScale)
    {
        Transform GroundCheck = gameObject.transform.Find(GroundCheckName);
        if (GroundCheck == null)
        {
            GameObject GroundCheckObject = new GameObject(GroundCheckName);
            GroundCheckObject.transform.SetParent(transform);
            GroundCheckObject.transform.localPosition = LocalPosition;
            GroundCheckObject.transform.localRotation = Quaternion.identity;
            GroundCheckObject.transform.localScale = LocalScale;
            return GroundCheckObject.transform;
        }
        else
        {
            return GroundCheck;
        }
    }

    void Start()
    {
        RB.constraints = RigidbodyConstraints2D.FreezeRotation;
        Anim.SetBool("CanMove", true);
        //Time.timeScale = 0.5f;

        //_fallSpeedYDampingChangeThreshold = CameraManager.Instance.FallSpeedYDampingChangeThreshold;
    }

    void Update()
    {
        Anim.SetBool("IsGrounded", IsGrounded());


        if (!Anim.GetBool("CanMove") ||
            Anim.GetBool("IsAttacking") ||
            Anim.GetBool("IsRolling")) return;

        MoveInput = Input.GetAxis("Horizontal");

        Anim.SetBool("IsWallSliding", OnWall() && !IsGrounded() && RB.linearVelocity.y < 0);


        if (IsGrounded())
        {
            WallJumpCooldown = 1.1f;
            JustWallJumped = false;
        }

        if (Anim.GetBool("IsWallSliding"))
        {
            RB.linearVelocity = new Vector2(0, Mathf.Max(RB.linearVelocity.y, -m_WallSlideSpeed));
            RB.gravityScale = m_WallSlideGravity;
        }
        else if (WallJumpCooldown > 1f)
        {
            Anim.SetBool("IsRunning", Anim.GetBool("IsGrounded") && MoveInput != 0);
            RB.linearVelocity = new Vector2(MoveInput * m_Speed, RB.linearVelocity.y);
            RB.gravityScale = m_GravityScale;
        }

        UpdatePlayerFacing();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        else
        {
            WallJumpCooldown += Time.deltaTime;
            IsFloating = false;
        }

        UpdateGravity();

        WasGroundedLastFrame = IsGrounded();
        WasWallSlidingLastFrame = Anim.GetBool("IsWallSliding");

        //if (Body.linearVelocity.y < _fallSpeedYDampingChangeThreshold &&
        //    !CameraManager.Instance.IsLerpingYDamping &&
        //    !CameraManager.Instance.LerpredFromPlayerFalling)
        //{
        //    CameraManager.Instance.LerpYDamping(true);
        //}

        //if (Body.linearVelocity.y >= 0f &&
        //    !CameraManager.Instance.IsLerpingYDamping &&
        //    CameraManager.Instance.LerpredFromPlayerFalling)
        //{
        //    CameraManager.Instance.LerpredFromPlayerFalling = false;
        //    CameraManager.Instance.LerpYDamping(false);
        //}
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
        if (Anim.GetBool("IsWallSliding") || JustWallJumped) { return; }

        Flip(MoveInput);

        if (IsGrounded() && !WasGroundedLastFrame && WasWallSlidingLastFrame && !JustWallJumped)
        {
            Flip(-GetFacingDirection());
        }
    }

    private void UpdateGravity()
    {
        if (IsFloating)
        {
            RB.gravityScale = m_FloatGravityScale;
        }
        else if (RB.linearVelocity.y < 0.1f)
        {
            RB.gravityScale = m_GravityScale * m_FallMultiplier;
        }
        else if (RB.linearVelocity.y > 0.1f && !Input.GetKey(KeyCode.Space))
        {
            RB.gravityScale = m_GravityScale * m_LowJumpMultiplier;
        }
        else
        {
            RB.gravityScale = m_GravityScale;
        }
    }

    private void HandleGroundJump()
    {
        RB.linearVelocity = new Vector2(RB.linearVelocity.x, m_JumpForce);
        Anim.SetTrigger("Jump");
    }

    private void HandleWallJump()
    { 

        Anim.SetBool("IsWallSliding", false);        
        Anim.SetTrigger("WallJump");
        //Invoke("OnWallJumpFlip", 0.2f);
    }

    private void HandleWallJumpPhysics()
    {
        float wallDirection = GetFacingDirection();
        print(wallDirection);

        print(RB.linearVelocity);
        RB.linearVelocity = new Vector2(-wallDirection * m_WallJumpHorizontalForce, m_WallJumpVerticalForce);
        print(RB.linearVelocity);
        Flip(-wallDirection);

        WallJumpCooldown = 0;
        JustWallJumped = true;
    }

    private void Jump()
    {
        if (IsGrounded())
        {
            HandleGroundJump();
        }
        else if (Anim.GetBool("IsWallSliding"))
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
        Vector2 CastDirection = new Vector2(GetFacingDirection(), 0);

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

    private int GetFacingDirection()
    {
        return Mathf.Abs(transform.eulerAngles.y - 180) < 0.1f ? -1 : 1;
    }
}