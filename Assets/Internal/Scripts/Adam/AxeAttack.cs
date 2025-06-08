using UnityEngine;

public class AxeAttack : MonoBehaviour
{
    public float m_AttackCooldown = 2f;

    private bool ComboQueued = false;

    private Animator Anim;
    private BoxCollider2D AxeLightHitbox;
    private BoxCollider2D AxeHeavyHitbox;
    private BoxCollider2D AxeJumpLightHitbox;
    private BoxCollider2D AxeJumpHeavyHitbox;

    void Awake()
    {
        Anim = GetComponent<Animator>();

        AxeLightHitbox = transform.Find("AxeLightHitbox").GetComponent<BoxCollider2D>();
        AxeHeavyHitbox = transform.Find("AxeHeavyHitbox").GetComponent<BoxCollider2D>();
        AxeJumpLightHitbox = transform.Find("AxeJumpLightHitbox").GetComponent<BoxCollider2D>();
        AxeJumpHeavyHitbox = transform.Find("AxeJumpHeavyHitbox").GetComponent<BoxCollider2D>();
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
                    AxeJumpLight();
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    AxeJumpHeavy();
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
    }

    private void AxeHeavy()
    {
        Anim.SetBool("IsAttacking", true);
        Anim.SetBool("CanMove", false);
        Anim.SetTrigger("AxeHeavy");
    }

    private void AxeJumpLight()
    {
        Anim.SetBool("IsAttacking", true);
        Anim.SetTrigger("AxeJumpLight");
    }

    private void AxeJumpHeavy()
    {
        Anim.SetBool("IsAttacking", true);
        Anim.SetTrigger("AxeJumpHeavy");
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

    void ActivateHitbox(int Mux)
    {
        switch(Mux)
        {
            case 0:
                AxeLightHitbox.gameObject.SetActive(true);
                break;
            case 1:
                AxeHeavyHitbox.gameObject.SetActive(true);
                break;
            case 2:
                AxeJumpLightHitbox.gameObject.SetActive(true);
                break;
            case 3:
                AxeJumpHeavyHitbox.gameObject.SetActive(true);
                break;
        }
        
    }

    void DeactivateHitbox(int Mux)
    {
        switch (Mux)
        {
            case 0:
                AxeLightHitbox.gameObject.SetActive(false);
                break;
            case 1:
                AxeHeavyHitbox.gameObject.SetActive(false);
                break;
            case 2:
                AxeJumpLightHitbox.gameObject.SetActive(false);
                break;
            case 3:
                AxeJumpHeavyHitbox.gameObject.SetActive(false);
                break;
        }

    }
}
