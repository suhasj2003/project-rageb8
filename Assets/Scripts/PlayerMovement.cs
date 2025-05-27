using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float m_Speed = 5f;
    public float m_JumpForce = 15f;
    public float m_GravityScale = 5f;
    public float m_FallMultiplier = 2.5f;
    public float m_LowJumpMultiplier = 2f;
    public bool m_CanMove = true;
    public Transform m_GroundCheck;
    public LayerMask m_GroundLayer;

    private float GroundCheckRadius = 0.1f;
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
        if (m_CanMove)
        {
            float MoveInput = Input.GetAxis("Horizontal");
            Body.linearVelocity = new Vector2(MoveInput * m_Speed, Body.linearVelocity.y);

            // flip player
            if (MoveInput > 0.01f)
            {
                transform.localScale = new Vector3(5, 5, 1);
            }
            else if (MoveInput < -0.01f)
            {
                transform.localScale = new Vector3(-5, 5, 1);
            }

            if (Input.GetKey(KeyCode.Space) && IsGrounded())
            {
                Jump();
            }

            // set anim parameters
            Anim.SetBool("Run", MoveInput != 0 && m_CanMove);
            Anim.SetBool("Grounded", IsGrounded());

            // decr rise and incr fall
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
    }

    private void Jump()
    {
        Body.linearVelocity = new Vector2(Body.linearVelocity.x, m_JumpForce);
        Anim.SetTrigger("Jump");
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(m_GroundCheck.position, GroundCheckRadius, m_GroundLayer);
    }
}
