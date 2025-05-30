using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public EnemyData EnemyData;
    private int CurrentHealth;

    public Flash Flash;
    public HitStop HitStop;

    private Animator Anim;

    void Awake()
    {
        Anim = GetComponent<Animator>();
    }

    void Start()
    {
        CurrentHealth = EnemyData.MaxHP;
    }

    public void TakeDamage(int Amount)
    {
        //Debug.Log("Enemy Took damage");
        CurrentHealth -= Amount;

        Flash.SpriteFlicker();
        HitStop.DoHitStop();

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Anim.SetTrigger("Dead");
        Anim.SetBool("IsDead", true);
    }

    private void OnDeathDestroy()
    {
        Destroy(gameObject);
    }
}