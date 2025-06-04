using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public int Damage = 1;
    public float Lifetime = 5f;

    private Rigidbody2D rb;
    private Animator Anim;
    private float timer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
    }

    void Start()
    {
        rb.constraints = RigidbodyConstraints2D.FreezePositionY;
    }

    void OnEnable()
    {
        timer = 0f;
        // Optionally reset velocity if reusing from pool
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= Lifetime)
        {
            ReturnToPool();
        }
    }

    // Call this to launch the projectile
    public void Launch(Vector2 direction, float speed)
    {
        if (rb != null)
            rb.linearVelocity = direction.normalized * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Example: check for player or enemy tag
        if (other.CompareTag("Player"))
        {
            // If the player has a health script, deal damage
            var health = other.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(Damage);

            ReturnToPool();
        }
        // Optionally, add logic for hitting walls or other objects
        else if (other.gameObject.layer == LayerMask.NameToLayer("Walls"))
        {
            ReturnToPool();
        }
    }

    void ReturnToPool()
    {
        Anim.SetTrigger("OnHit");
        rb.linearVelocity = Vector3.zero;
        // ProjectilePool.Instance.ReturnProjectile(this.gameObject);
    }

    void EndProjectile()
    {
        gameObject.SetActive(false);
    }
}