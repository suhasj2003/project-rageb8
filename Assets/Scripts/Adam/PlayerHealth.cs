using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("PlayerData")]
    public PlayerData PlayerData;
    private int CurrentHealth;

    public Slider HealthSlider;

    [Header("Knockback")]
    public float KnockbackForce = 10f;
    public float KnockbackDuration = 0.3f;

    [Header("HitEffect")]
    public Flash Flash;
    public HitStop HitStop;

    private Rigidbody2D Body;
    private bool IsKnockbackActive = false;
    private float KnockbackTimer = 0f;
    private Vector2 KnockbackVelocity;

    void Awake()
    {
        CurrentHealth = PlayerData.MaxHP;
        HealthSlider.maxValue = PlayerData.MaxHP;
        HealthSlider.value = CurrentHealth;
        Body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (IsKnockbackActive)
        {
            Body.linearVelocity = KnockbackVelocity;
            KnockbackTimer -= Time.deltaTime;
            if (KnockbackTimer <= 0f)
            {
                IsKnockbackActive = false;
                Body.linearVelocity = Vector2.zero;
            }
        }
    }

    public void TakeDamage(int Damage)
    {
        CurrentHealth -= Damage;
        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
        }

        HealthSlider.value = CurrentHealth;
        
        if (CurrentHealth <= 0)
        {
            Die();
        }

        Flash.SpriteFlash();
        HitStop.DoHitStop();
    }

    public void ApplyKnockback(Vector3 AttackerPosition)
    {
        if (IsKnockbackActive) return;

        Vector2 direction = (transform.position - AttackerPosition).normalized;
        KnockbackVelocity = direction * KnockbackForce;
        KnockbackTimer = KnockbackDuration;
        IsKnockbackActive = true;
    }

    public void Heal(int amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > PlayerData.MaxHP)
        {
            CurrentHealth = PlayerData.MaxHP;
        }
        HealthSlider.value = CurrentHealth;
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}