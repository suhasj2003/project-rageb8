using UnityEngine;

public class AxeHitbox : MonoBehaviour
{
    public AttackData AttackData;
    private float LastHitTime;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Time.time - LastHitTime < AttackData.AttackCooldown) return;

        if (collision.CompareTag("Breakable"))
        {
            Destroy(collision.gameObject);
        }

        EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(AttackData.Damage);
            Debug.Log("Dealt" + AttackData.Damage + "damage to enemy!");
            LastHitTime = Time.time;
        }
    }
}