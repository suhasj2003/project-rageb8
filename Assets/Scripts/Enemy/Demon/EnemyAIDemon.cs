using UnityEngine;

public class EnemyAIDemon : MonoBehaviour
{
    public EnemyData EnemyData;
    public Transform Player;
    public BoxCollider2D AttackHitbox;

    public Rigidbody2D Body;
    private Animator Anim;
    private float LastAttackTime;
    private bool IsAttacking = false;

    void Awake()
    {
        Body = GetComponent<Rigidbody2D>();
        Body.constraints = RigidbodyConstraints2D.FreezeRotation;
        Anim = GetComponent<Animator>();
        AttackHitbox.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Player == null) return;

        float Distance = Vector2.Distance(transform.position, Player.position);

        // Chase if not in attack range
        if (Distance > EnemyData.AttackRange && !IsAttacking)
        {
            Anim.SetBool("IsAttacking", false);
            Anim.SetBool("IsWalking", true);
            Vector2 dir = (Player.position - transform.position).normalized;
            transform.position += (Vector3)dir * EnemyData.MoveSpeed * Time.deltaTime;

            if (dir.x != 0)
                transform.localScale = new Vector3(Mathf.Sign(dir.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        }
        else
        {
            Anim.SetBool("IsWalking", false);

            if (Time.time >= LastAttackTime + EnemyData.AttackCooldown && !Anim.GetBool("IsAttacking"))
            {
                StartAttack();
            }
        }
    }

    void StartAttack()
    {
        Anim.SetBool("IsAttacking", true);
        Anim.SetTrigger("Attack");
        LastAttackTime = Time.time;
    }

    void ActivateAttackHitbox()
    {
        AttackHitbox.gameObject.SetActive(true);
    }

    void DeactivateAttackHitbox()
    {
        AttackHitbox.gameObject.SetActive(false);
    }

    void EndAttack()
    {
        Anim.SetBool("IsAttacking", false);
    }
}