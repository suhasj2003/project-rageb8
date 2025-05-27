using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public PlayerMovement PM;

    private bool isAttacking = false;
    private bool comboAvailable = false;
    private bool comboQueued = false;
    public float attackCooldown = 2f;

    private Animator anim;
    private BoxCollider2D hitbox;

    void Awake()
    {
        anim = GetComponent<Animator>();
        hitbox = transform.Find("axeHitbox").GetComponent<BoxCollider2D>();
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
        if (!isAttacking)
        {
            isAttacking = true;
            PM.canMove = false;
            comboAvailable = true;
            comboQueued = false;

            anim.SetTrigger("attack");
            Invoke("ActivateHitbox", 0.2f);
            Invoke("DeactivateHitbox", 0.4f);
        }
        else if (comboAvailable)
        {
            comboQueued = true;
            comboAvailable = false;
        }
    }

    public void OnAttack1Complete()
    {
        if (comboQueued)
        {
            anim.SetTrigger("attack2");
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

    private void EndAttack()
    {
        PM.canMove = true;
        isAttacking = false;
        comboAvailable = false;
        comboQueued = false;
    }

    void ActivateHitbox()
    {
        hitbox.gameObject.SetActive(true);
    }

    void DeactivateHitbox()
    {
        hitbox.gameObject.SetActive(false);
    }
}
