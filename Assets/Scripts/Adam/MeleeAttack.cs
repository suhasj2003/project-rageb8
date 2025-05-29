using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public PlayerMovement PM;

    private bool IsAttacking = false;
    private bool ComboAvailable = false;
    private bool ComboQueued = false;
    public float m_AttackCooldown = 2f;

    private Animator Anim;
    private BoxCollider2D AxeHitbox;
    private BoxCollider2D KickHitbox;

    void Awake()
    {
        Anim = GetComponent<Animator>();
        AxeHitbox = transform.Find("AxeHitbox").GetComponent<BoxCollider2D>();
        KickHitbox = transform.Find("KickHitbox").GetComponent<BoxCollider2D>();
        Anim.SetBool("IsAttacking", false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !Anim.GetBool("IsRolling") && !Anim.GetBool("IsWallSliding"))
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (!IsAttacking || ComboAvailable)
        {
            Anim.SetBool("IsAttacking", true);

            if (PM.IsGrounded())
            {
                if (!IsAttacking) 
                {

                    ResetCombo();

                    IsAttacking = true;
                    PM.m_CanMove = false;
                    Anim.SetTrigger("Attack1");
                    AxeHitboxToggle();
                }
                else if (ComboAvailable) 
                {
                    ComboQueued = true;
                }
            }
            else if (!PM.IsWallSliding) 
            {
                ResetCombo();
                IsAttacking = true;
                Anim.SetTrigger("AttackJump");
                KickHitboxToggle();
            }
        }
    }

    public void EnableComboWindow()
    {
        ComboAvailable = true;
    }

    private void ResetCombo()
    {
        ComboAvailable = false;
        ComboQueued = false;
    }

    public void DisableComboWindow()
    {
        ComboAvailable = false;
    }

    public void CheckForCombo()
    {
        if (ComboQueued)
        {
            Anim.SetTrigger("Attack2");
            AxeHitboxToggle();
        }
    }

    private void AxeHitboxToggle()
    {
        Invoke("ActivateAxeHitbox", 0.1f);
        Invoke("DeactivateAxeHitbox", 0.25f);
    }

    private void KickHitboxToggle()
    {
        Invoke("ActivateKickHitbox", 0.1f);
        Invoke("DeactivateKickHitbox", 0.25f);
    }

    private void EndAttack()
    {
        Anim.SetBool("IsAttacking", false);
        PM.m_CanMove = true;
        IsAttacking = false;
        ComboAvailable = PM.IsGrounded() && ComboQueued;
        ComboQueued = false;
    }

    void ActivateAxeHitbox()
    {
        AxeHitbox.gameObject.SetActive(true);
    }

    void DeactivateAxeHitbox()
    {
        AxeHitbox.gameObject.SetActive(false);
    }

    void ActivateKickHitbox()
    {
        KickHitbox.gameObject.SetActive(true);
    }

    void DeactivateKickHitbox()
    {
        KickHitbox.gameObject.SetActive(false);
    }
}
