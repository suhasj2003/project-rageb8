using UnityEngine;

public class Hitbox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Breakable"))
        {
            Destroy(collision.gameObject);
        }

        EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(20);
            Debug.Log("Dealt 20 damage to enemy!");
        }

        
    }
}