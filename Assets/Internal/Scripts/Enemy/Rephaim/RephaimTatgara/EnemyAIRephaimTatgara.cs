using UnityEngine;

public class EnemyAIRephaimTatgara : MonoBehaviour
{
    public EnemyData EnemyData;

    private float LastAttackTime;

    private BoxCollider2D AttackHitbox;
    private Rigidbody2D Body;
    private Animator Anim;
    private Transform Player;

    void Awake()
    {
        Body = GetComponent<Rigidbody2D>();
        Body.constraints = RigidbodyConstraints2D.FreezeRotation | 
            RigidbodyConstraints2D.FreezePositionX;
        Anim = GetComponent<Animator>();
        
        AttackHitbox = transform.Find("AttackHitbox").GetComponent<BoxCollider2D>();
        Player = GameObject.FindWithTag("Player").transform;
    }

    void Start()
    {
        AttackHitbox.gameObject.SetActive(false);
        Anim.SetBool("CanMove", true);
    }

    void Update()
    {
        if (Player == null || !Anim.GetBool("CanMove") || Anim.GetBool("IsAttacking")) return;

        float Distance = Vector2.Distance(transform.position, Player.position);

        if (Distance > EnemyData.AttackRange)
        {
            float dirX = Player.position.x - transform.position.x;
            Vector3 move = new Vector3(Mathf.Sign(dirX) * EnemyData.MoveSpeed * Time.deltaTime, 0, 0);
            transform.position += move;

            if (dirX != 0)
                transform.localScale = new Vector3(Mathf.Sign(dirX) * Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        }
        else if (Time.time >= LastAttackTime + EnemyData.AttackCooldown)
        {
            StartAttack();
        }
    }

    void StartAttack()
    {
        print("here");
        Anim.SetTrigger("Attack");
        Anim.SetBool("IsAttacking", true);
        Anim.SetBool("IsWalking", false);
        Anim.SetBool("CanMove", false);
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
        Anim.SetBool("CanMove", true);
    }
}