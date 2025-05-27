using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 15f;
    public float gravityScale = 5f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    public bool canMove = true;

    public Transform groundCheck;
    private float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        // grab refs
        body = GetComponent<Rigidbody2D>();
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    private void Update() 
    {
        if (canMove)
        {
            float moveInput = Input.GetAxis("Horizontal");
            body.linearVelocity = new Vector2(moveInput * speed, body.linearVelocity.y);

            // flip player
            if (moveInput > 0.01f)
            {
                transform.localScale = new Vector3(5, 5, 1);
            }
            else if (moveInput < -0.01f)
            {
                transform.localScale = new Vector3(-5, 5, 1);
            }

            if (Input.GetKey(KeyCode.Space) && isGrounded())
            {
                Jump();
            }

            // set anim
            anim.SetBool("run", moveInput != 0 && canMove);
            anim.SetBool("grounded", isGrounded());

            // decr rise and incr fall
            if (body.linearVelocity.y < 0)
            {
                body.gravityScale = gravityScale * fallMultiplier;
            }
            else if (body.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
            {
                body.gravityScale = gravityScale * lowJumpMultiplier;
            }
            else
            {
                body.gravityScale = gravityScale;
            }
        }
    }

    private void Jump()
    {
        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
        anim.SetTrigger("jump");
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
}
