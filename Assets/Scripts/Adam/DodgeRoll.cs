using UnityEngine;

public class DodgeRoll : MonoBehaviour
{
    public PlayerMovement PM;

    private float RollTimer = 0f;
    public float RollDuration = 0.4f;
    public float RollSpeed = 10f;
    private Vector2 RollDirection;

    public float m_PostRollCooldown = 1f;

    private Animator Anim;

    void Awake()
    {
        Anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Anim.GetBool("IsAttacking")) return;

        HandleRollMovement();

        if (PM.m_IsRolling) return;

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Roll();
        }
    }

    public void Roll()
    {
        if (!PM.m_IsRolling && PM.IsGrounded())
        {
            PM.m_IsRolling = true;
            RollTimer = RollDuration;
            RollDirection = new Vector2(Mathf.Sign(transform.localScale.x), 0);
            Anim.SetTrigger("Roll");
            Anim.SetBool("IsRolling", true);
            PM.m_CanMove = false;
        }
    }

    public void RollBurstMove()
    {
        float direction = Mathf.Sign(transform.localScale.x);
        PM.m_Body.linearVelocity = new Vector2(direction * RollSpeed, PM.m_Body.linearVelocity.y);
    }

    private void EndRoll()
    {
        PM.m_IsRolling = false;
        Anim.SetBool("IsRolling", false);
        PM.m_Body.linearVelocity = Vector2.zero;
        PM.m_Body.gravityScale = PM.m_GravityScale;
        Invoke(nameof(ResetControl), m_PostRollCooldown);
    }

    private void ResetControl()
    {
        PM.m_CanMove = true;
    }

    private void HandleRollMovement()
    {
        if (!PM.m_IsRolling) return;

        RollTimer -= Time.deltaTime;

        if (RollTimer > 0)
        {
            PM.m_Body.linearVelocity = RollDirection * RollSpeed;
            PM.m_Body.gravityScale = 0;
        }
    }
}
