using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public EnemyData EnemyData;
    private int CurrentHealth;

    public Flash Flash;
    public HitStop HitStop;

    void Start()
    {
        CurrentHealth = EnemyData.MaxHP;
    }

    public void TakeDamage(int Amount)
    {
        //Debug.Log("Enemy Took damage");
        CurrentHealth -= Amount;

        Flash.SpriteFlash();
        HitStop.DoHitStop();

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}