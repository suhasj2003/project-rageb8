using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public PlayerMovement PM;

    private bool IsAttacking = false;
    private bool ComboAvailable = false;
    private bool ComboQueued = false;
    public float m_AttackCooldown = 2f;

    private Animator Anim;
    private BoxCollider2D Hitbox;

    void Awake()
    {
        Anim = GetComponent<Animator>();
        Hitbox = transform.Find("AxeHitbox").GetComponent<BoxCollider2D>();
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
        if (!IsAttacking)
        {
            IsAttacking = true;
            PM.m_CanMove = false;
            ComboAvailable = true;
            ComboQueued = false;

            Anim.SetTrigger("Attack1");
            HitboxToggle();
        }
        else if (ComboAvailable)
        {
            ComboQueued = true;
            ComboAvailable = false;
        }
    }

    public void OnAttack1Complete()
    {
        if (ComboQueued)
        {
            Anim.SetTrigger("Attack2");
            HitboxToggle();
        }
        else
        {
            EndAttack();
        }
    }

    public void OnAttack2Complete()
    {
        EndAttack();
    }

    private void HitboxToggle()
    {
        Invoke("ActivateHitbox", 0.1f);
        Invoke("DeactivateHitbox", 0.25f);
    }

    private void EndAttack()
    {
        PM.m_CanMove = true;
        IsAttacking = false;
        ComboAvailable = false;
        ComboQueued = false;
    }

    void ActivateHitbox()
    {
        Hitbox.gameObject.SetActive(true);
    }

    void DeactivateHitbox()
    {
        Hitbox.gameObject.SetActive(false);
    }
}
