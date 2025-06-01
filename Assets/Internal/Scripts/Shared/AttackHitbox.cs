using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public EnemyData EnemyData;
    private float LastHitTime;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Time.time - LastHitTime < EnemyData.AttackCooldown) return;

        if (collision.CompareTag("Player"))
        {
            
            LastHitTime = Time.time;
            PlayerHealth ph = collision.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                Debug.Log("hit player");
                ph.TakeDamage(EnemyData.AttackDamage);
                ph.ApplyKnockback(transform.position); 
            }
        }

    }
}