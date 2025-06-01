using UnityEngine;

public class HitStop : MonoBehaviour
{
    public float m_HitStopDuration = 0.07f;
    private float HitStopTimer = 0f;

    private Animator Anim;

    void Awake()
    {
        Anim = GetComponent<Animator>();
    }

    public void DoHitStop()
    {
        if (Anim == null) return;
        Anim.speed = 0f;
        HitStopTimer = m_HitStopDuration;
    }

    void Update()
    {
        if (HitStopTimer > 0)
        { 
            HitStopTimer -= Time.deltaTime;
            if (HitStopTimer <= 0)
            {
                Anim.speed = 1f;
            }
        }
    }
}