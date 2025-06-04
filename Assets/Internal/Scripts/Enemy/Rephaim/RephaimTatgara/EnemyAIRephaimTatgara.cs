using UnityEngine;

public class EnemyAITatgara : MonoBehaviour
{
    public EnemyData EnemyData;

    private float LastAttackTime;
    private float NextMoveDecisionTime;
    private bool IsRetreating;
    private bool IsStrafing;
    private int StrafeDirection; // -1 = Left, 1 = Right

    private BoxCollider2D AttackHitbox;
    private Rigidbody2D Body;
    private Animator Anim;
    private Transform Player;

    void Awake()
    {
        Body = GetComponent<Rigidbody2D>();
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

        if (Time.time > NextMoveDecisionTime)
        {
            DecideMovementPattern();
            NextMoveDecisionTime = Time.time + Random.Range(1.5f, 3f);
        }

        if (Distance > EnemyData.AttackRange)
        {
            MoveToPlayer();
        }
        else if (Time.time >= LastAttackTime + EnemyData.AttackCooldown + Random.Range(0f, 0.3f))
        {
            StartAttack();
        }
    }

    void DecideMovementPattern()
    {
        float RandVal = Random.value;
        IsRetreating = RandVal < 0.15f;
        IsStrafing = !IsRetreating && RandVal < 0.4f;
        StrafeDirection = Random.value > 0.5f ? 1 : -1;
    }

    void MoveToPlayer()
    {
        Vector3 Move = Vector3.zero;
        float DirX = Player.position.x - transform.position.x;

        if (IsRetreating)
        {
            Move = new Vector3(-Mathf.Sign(DirX) * EnemyData.MoveSpeed * 0.7f * Time.deltaTime, 0, 0);
        }
        else if (IsStrafing)
        {
            Move = new Vector3(StrafeDirection * EnemyData.MoveSpeed * 0.5f * Time.deltaTime, 0, 0);
        }
        else
        {
            Move = new Vector3(Mathf.Sign(DirX) * EnemyData.MoveSpeed * Time.deltaTime, 0, 0);
        }

        transform.position += Move;

        if (DirX != 0)
            transform.localScale = new Vector3(Mathf.Sign(DirX) * Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
    }

    void StartAttack()
    {
        Anim.SetTrigger("Attack");
        Anim.SetBool("IsAttacking", true);
        Anim.SetBool("IsWalking", false);
        Anim.SetBool("CanMove", false);
        LastAttackTime = Time.time;
    }

    // Animation Events
    void ActivateAttackHitbox() => AttackHitbox.gameObject.SetActive(true);
    void DeactivateAttackHitbox() => AttackHitbox.gameObject.SetActive(false);
    void EndAttack()
    {
        Anim.SetBool("IsAttacking", false);
        Anim.SetBool("CanMove", true);
    }
}