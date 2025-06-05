using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public int Damage = 1;
    public float Lifetime = 5f;

    private Rigidbody2D RB;
    private Animator Anim;
    private float Timer;

    void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
    }

    void Start()
    {
        RB.constraints = RigidbodyConstraints2D.FreezePositionY;
    }

    void OnEnable()
    {
        Timer = 0f;
        if (RB != null) RB.linearVelocity = Vector2.zero;
    }

    void Update()
    {
        Timer += Time.deltaTime;
        if (Timer >= Lifetime)
        {
            ReturnToPool();
        }
    }

    // Call this to launch the projectile
    public void Launch(Vector2 direction, float speed)
    {
        if (RB != null)
            RB.linearVelocity = direction.normalized * speed;
    }

    void OnTriggerEnter2D(Collider2D Other)
    {
        // Example: check for player or enemy tag
        if (Other.CompareTag("Player"))
        {
            // If the player has a health script, deal damage
            var Health = Other.GetComponent<PlayerHealth>();
            if (Health != null)
                Health.TakeDamage(Damage);

            ReturnToPool();
        }
        // Optionally, add logic for hitting walls or other objects
        else if (Other.gameObject.layer == LayerMask.NameToLayer("Walls"))
        {
            ReturnToPool();
        }
    }

    void ReturnToPool()
    {
        Anim.SetTrigger("OnHit");
        RB.linearVelocity = Vector3.zero;
        // ProjectilePool.Instance.ReturnProjectile(this.gameObject);
    }

    void EndProjectile()
    {
        gameObject.SetActive(false);
    }
}