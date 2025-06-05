using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public EnemyData EnemyData;
    private int CurrentHealth;

    private Flash Flash;
    private HitStop HitStop;

    private Animator Anim;
    private BoxCollider2D Box;
    private Rigidbody2D Body;

    void Awake()
    {
        Anim = GetComponent<Animator>();
        Box = GetComponent<BoxCollider2D>();
        Body = GetComponent<Rigidbody2D>();

        Flash = GetComponent<Flash>();
        HitStop = GetComponent<HitStop>();
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
        Box.enabled = false;
        Body.constraints = RigidbodyConstraints2D.FreezePosition;
        Anim.SetTrigger("Dead");
        Anim.SetBool("IsDead", true);
        Anim.SetBool("CanMove", false);
    }

    private void OnDeathDestroy()
    {
        Destroy(gameObject);
    }
}