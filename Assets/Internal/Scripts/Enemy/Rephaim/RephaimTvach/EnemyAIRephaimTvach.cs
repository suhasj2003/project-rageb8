using UnityEngine;
using System.Collections;

public class RangedEnemyAI : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] private float DetectionRange = 8f;
    [SerializeField] private float OptimalRange = 5f;
    [SerializeField] private float AttackCooldown = 2f;
    [SerializeField] private float ProjectileSpeed = 8f;

    [Header("References")]
    [SerializeField] private Transform ProjectileSpawnPoint;

    private Transform Player;
    private float LastAttackTime;
    private Animator Anim;
    private Rigidbody2D RB;

    void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        Anim = GetComponent<Animator>();
        RB = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        RB.constraints = RigidbodyConstraints2D.FreezePositionY;
        Anim.SetBool("CanMove", true);
    }

    void Update()
    {
        if (Player == null) return;
        if (!Anim.GetBool("CanMove")) return;

        float distance = Vector2.Distance(transform.position, Player.position);
        UpdateFacingDirection();

        if (distance <= DetectionRange)
        {
            if (distance > OptimalRange)
            {
                Anim.SetBool("IsWalking", true);
                MoveTowardsPlayer();
            }
            else if (Time.time >= LastAttackTime + AttackCooldown)
            {
                StopMoving();
                Attack();
            }
        }
    }

    void UpdateFacingDirection()
    {
        if (Player.position.x > transform.position.x)
            LeanTween.rotateY(gameObject, 0, 0).setEaseInOutSine();
        else
            LeanTween.rotateY(gameObject, 180, 0).setEaseInOutSine();
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (Player.position - transform.position).normalized;
        transform.Translate(direction * Time.deltaTime * 3f);
        Anim.SetBool("CanMove", true);
    }

    void StopMoving()
    {
        Anim.SetBool("IsWalking", false);
        Anim.SetBool("CanMove", false);
    }

    void Attack()
    {
        Anim.SetTrigger("Attack");
        LastAttackTime = Time.time;
    }

    // Animation event
    void FireProjectile()
    {
        GameObject projectile = ProjectilePool.Instance.GetProjectile(
            ProjectileSpawnPoint.position,
            ProjectileSpawnPoint.rotation
        );

        Vector2 direction = (Player.position - ProjectileSpawnPoint.position).normalized;
        projectile.GetComponent<Rigidbody2D>().linearVelocity = direction * ProjectileSpeed;
        StartCoroutine(ReturnProjectileAfterDelay(projectile, 2f));
    }

    void EndAttack()
    {
        Anim.SetBool("CanMove", true);
    }

    IEnumerator ReturnProjectileAfterDelay(GameObject projectile, float delay)
    {
        yield return new WaitForSeconds(delay);
        ProjectilePool.Instance.ReturnProjectile(projectile);
    }

    bool HasLineOfSight()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Player.position - transform.position,
            DetectionRange,
            LayerMask.GetMask("Player", "Walls")
        );

        return hit.collider != null && hit.collider.CompareTag("Player");
    }
}
