using UnityEngine;

public class AxeAttack : MonoBehaviour
{
    public float m_AttackCooldown = 2f;

    private bool ComboQueued = false;

    private Animator Anim;
    private BoxCollider2D AxeLightHitbox;
    private BoxCollider2D AxeHeavyHitbox;
    private BoxCollider2D AxeKickHitbox;

    void Awake()
    {
        Anim = GetComponent<Animator>();
        AxeLightHitbox = transform.Find("AxeLightHitbox").GetComponent<BoxCollider2D>();
        AxeHeavyHitbox = transform.Find("AxeHeavyHitbox").GetComponent<BoxCollider2D>();
        AxeKickHitbox = transform.Find("AxeKickHitbox").GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        Anim.SetBool("IsAttacking", false);
    }

    void Update()
    {
        if (!Anim.GetBool("IsRolling") &&
            !Anim.GetBool("IsWallSliding") &&
            !Anim.GetBool("IsAttacking"))
        {
            if (Anim.GetBool("IsGrounded"))
            { 
                if (Input.GetMouseButtonDown(0))
                {
                    AxeLight();
                }
                else if (Input.GetMouseButtonDown(1))
                {
                   AxeHeavy();
                }
            }
            else 
            {
                if (Input.GetMouseButtonDown(0))
                {  
                    AxeJump();
                }
            }
        }
        else if (Anim.GetCurrentAnimatorStateInfo(0).IsName("AxeLight"))
        {
            if (Input.GetMouseButtonDown(1))
            {
                ComboQueued = true;
            }
        }
    }

    private void AxeLight()
    {
        Anim.SetBool("IsAttacking", true);
        Anim.SetBool("CanMove", false);
        Anim.SetTrigger("AxeLight");
        AxeLightHitboxToggle();
    }

    private void AxeHeavy()
    {
        Anim.SetBool("IsAttacking", true);
        Anim.SetBool("CanMove", false);
        Anim.SetTrigger("AxeHeavy");
        AxeHeavyHitboxToggle();
    }

    private void AxeJump()
    {
        Anim.SetBool("IsAttacking", true);
        Anim.SetTrigger("AxeJump");
        AxeKickHitboxToggle();
    }

    private void EndAttack()
    {
        if (!ComboQueued)
        {
            ComboQueued = false;
            Anim.SetBool("IsAttacking", false);
            Anim.SetBool("CanMove", true);
            
        }
        else 
        {
            ComboQueued = false;
            AxeHeavy();
        }
    }

    private void AxeLightHitboxToggle()
    {
        Invoke("ActivateAxeLightHitbox", 0.1f);
        Invoke("DeactivateAxeLightHitbox", 0.25f);
    }

    private void AxeHeavyHitboxToggle()
    {
        Invoke("ActivateAxeHeavyHitbox", 0.1f);
        Invoke("DeactivateAxeHeavyHitbox", 0.25f);
    }

    private void AxeKickHitboxToggle()
    {
        Invoke("ActivateAxeKickHitbox", 0.1f);
        Invoke("DeactivateAxeKickHitbox", 0.25f);
    }

    void ActivateAxeLightHitbox()
    {
        AxeLightHitbox.gameObject.SetActive(true);
    }

    void DeactivateAxeLightHitbox()
    {
        AxeLightHitbox.gameObject.SetActive(false);
    }

    void ActivateAxeHeavyHitbox()
    {
        AxeHeavyHitbox.gameObject.SetActive(true);
    }

    void DeactivateAxeHeavyHitbox()
    {
        AxeHeavyHitbox.gameObject.SetActive(false);
    }

    void ActivateAxeKickHitbox()
    {
        AxeKickHitbox.gameObject.SetActive(true);
    }

    void DeactivateAxeKickHitbox()
    {
        AxeKickHitbox.gameObject.SetActive(false);
    }
}
