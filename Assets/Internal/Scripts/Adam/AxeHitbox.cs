using UnityEngine;
using System.Collections.Generic;

public class AxeHitbox : MonoBehaviour
{
    public AttackData AttackData;
    [SerializeField] private float jumpHeavyUpwardForce = 2f;

   private float LastHitTime;

    private Rigidbody2D RB;
    //private HashSet<EnemyHealth> _hitEnemies = new HashSet<EnemyHealth>();
    void Awake()
    {
        RB = GetComponentInParent<Rigidbody2D>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        EnemyHealth enemy = collision.GetComponent<EnemyHealth>();

        if ((Time.time - LastHitTime < AttackData.AttackCooldown)) return;

        if (collision.CompareTag("Breakable"))
        {
            Destroy(collision.gameObject);
        }

        if (enemy != null)
        {
            enemy.TakeDamage(AttackData.Damage);
            Debug.Log("Dealt " + AttackData.Damage + " damage to enemy!");

            LastHitTime = Time.time;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (AttackData.AttackName == "AxeJumpHeavy")
        {
            RB.linearVelocity = new Vector2(RB.linearVelocity.x, 0f);
            RB.AddForce(Vector2.up * jumpHeavyUpwardForce, ForceMode2D.Impulse);
        }
    }
}