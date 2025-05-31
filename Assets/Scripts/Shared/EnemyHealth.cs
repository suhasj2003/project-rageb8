using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public EnemyData EnemyData;
    private int CurrentHealth;

    public Flash Flash;
    public HitStop HitStop;

    private Animator Anim;
    private BoxCollider2D Body;

    void Awake()
    {
        Anim = GetComponent<Animator>();
        Body = GetComponent<BoxCollider2D>();
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
        Body.enabled = false;
        Anim.SetTrigger("Dead");
        Anim.SetBool("IsDead", true);
        Anim.SetBool("CanMove", false);
    }

    private void OnDeathDestroy()
    {
        Destroy(gameObject);
    }
}