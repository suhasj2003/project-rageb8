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
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (!PM.IsGrounded())
        {
            Anim.SetTrigger("AttackJump");
            KickHitboxToggle();
        }
        else if (!IsAttacking)
        {
            IsAttacking = true;
            PM.m_CanMove = false;
            ComboAvailable = true;
            ComboQueued = false;

            Anim.SetTrigger("Attack1");
            AxeHitboxToggle();
        }
        else if (ComboAvailable)
        {
            ComboQueued = true;
            ComboAvailable = false;
        }
    }

    public void Attack1()
    {
        if (ComboQueued)
        {
            Anim.SetTrigger("Attack2");
            AxeHitboxToggle();
        }
        else
        {
            EndAttack();
        }
    }

    public void Attack2()
    {
        EndAttack();
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
        PM.m_CanMove = true;
        IsAttacking = false;
        ComboAvailable = false;
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
