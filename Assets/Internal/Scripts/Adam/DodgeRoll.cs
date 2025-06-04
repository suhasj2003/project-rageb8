using UnityEngine;

public class DodgeRoll : MonoBehaviour
{
    [Header("Roll")]
    public bool m_RollIFrames = true;
    public float m_RollDuration = 0.4f;
    public float m_RollSpeed = 10f;
    public float m_PostRollCooldown = 1f;

    private float RollTimer = 0f;
    private Vector2 RollDirection;
    private float lastRollTime;

    private PlayerMovement PM;
    private Rigidbody2D Body;
    private BoxCollider2D Hitbox;
    private Animator Anim;
    private SpriteRenderer SpriteRenderer;

    void Awake()
    {
        Anim = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Hitbox = GetComponent<BoxCollider2D>();
        PM = FindFirstObjectByType<PlayerMovement>();
        Body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Anim.GetBool("IsAttacking")) return;

        HandleRollMovement();

        if (Anim.GetBool("IsRolling")) return;

        if (Input.GetKeyDown(KeyCode.LeftControl) && (Time.time - lastRollTime > m_PostRollCooldown))
        {
            Roll();
        }
    }

    public void Roll()
    {
        if (!Anim.GetBool("IsRolling") && PM.IsGrounded())
        {
            RollTimer = m_RollDuration;
            RollDirection = new Vector2(GetFacingDirection(), 0);
            Anim.SetTrigger("Roll");
            Anim.SetBool("IsRolling", true);
            Anim.SetBool("CanMove", false);
        }
    }

    public void RollBurstMove()
    {
        float Direction = Mathf.Sign(transform.localScale.x);
        Body.linearVelocity = new Vector2(Direction * m_RollSpeed, Body.linearVelocity.y);
    }

    private void EndRoll()
    {
        Anim.SetBool("IsRolling", false);
        Body.linearVelocity = Vector2.zero;
        Body.gravityScale = PM.m_GravityScale;
        ResetControl();
        lastRollTime = Time.time;
    }

    private void ResetControl()
    {
        Anim.SetBool("CanMove", true);
    }

    private void HandleRollMovement()
    {
        if (!Anim.GetBool("IsRolling")) return;

        RollTimer -= Time.deltaTime;

        if (RollTimer > 0)
        {
            Body.linearVelocity = RollDirection * m_RollSpeed;
            Body.gravityScale = 0;
        }
    }
    private int GetFacingDirection()
    {
        return Mathf.Abs(transform.eulerAngles.y - 180) < 0.1f ? -1 : 1;
    }
    
    private void DisableHitbox()
    {
        if (!m_RollIFrames) return;
        Hitbox.enabled = false;
    }

    private void EnableHitbox()
    {
        if (!m_RollIFrames) return;
        Hitbox.enabled = true;
    }
}
