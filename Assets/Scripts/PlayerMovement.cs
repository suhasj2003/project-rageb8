using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 15f;
    public float gravityScale = 5f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    private Rigidbody2D body;
    private Animator anim;
    private bool grounded;

    private void Awake()
    {
        // grab refs
        body = GetComponent<Rigidbody2D>();
        body.constraints = RigidbodyConstraints2D.FreezeRotation;

        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // default movement
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

        if (Input.GetKey(KeyCode.Space) && grounded)
        {
            Jump();
        }

        // set anim
        anim.SetBool("run", moveInput != 0);
        anim.SetBool("grounded", grounded);

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
    private void Jump()
    {
        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
        anim.SetTrigger("jump");
        grounded = false;
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
            grounded = true;
    }
}
