using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 15f;
    public float gravityScale = 5f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public bool canMove = true;

    private bool isAttacking = false;
    private bool comboAvailable = false;
    private bool comboQueued = false;

    public float attackCooldown = 2f;
    private float comboTimer = 2f;

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
        if (Input.GetMouseButtonDown(0))
        {
            Attack(); // this now runs even during an attack to detect combo clicks
        }

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

            if (Input.GetKey(KeyCode.Space) && grounded)
            {
                Jump();
            }

            // set anim
            anim.SetBool("run", moveInput != 0 && canMove);
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

    private void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            canMove = false;
            comboAvailable = true;
            comboQueued = false;

            anim.SetTrigger("attack");
            Debug.Log("Attack 1 started");
        }
        else if (comboAvailable)
        {
            comboQueued = true;
            comboAvailable = false;
            Debug.Log("Combo queued");
        }
    }

    public void OnAttack1Complete()
    {
        if (comboQueued)
        {
            anim.SetTrigger("attack2");
            Debug.Log("Attack 2 started");
        }
        else
        {
            EndAttack();
            Debug.Log("Attack 1 ended");
        }
    }

    public void OnAttack2Complete()
    {
        EndAttack();
        Debug.Log("Attack 2 ended");
    }

    private void EndAttack()
    {
        canMove = true;
        isAttacking = false;
        comboAvailable = false;
        comboQueued = false; 
    }
}
