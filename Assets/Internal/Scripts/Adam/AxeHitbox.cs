using UnityEngine;
using System.Collections.Generic;

public class AxeHitbox : MonoBehaviour
{
    public AttackData AttackData;
    private float LastHitTime;
    //private HashSet<EnemyHealth> _hitEnemies = new HashSet<EnemyHealth>();

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
            Debug.Log("Dealt" + AttackData.Damage + "damage to enemy!");

            LastHitTime = Time.time;
        }
    }
}