using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    public int DamageAmount = 5;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(DamageAmount);
                player.ApplyKnockback(transform.position);
            }
        }
    }
}