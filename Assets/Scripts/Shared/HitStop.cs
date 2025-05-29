using UnityEngine;

public class HitStop : MonoBehaviour
{
    public Animator Animator;
    public float HitStopDuration = 0.07f;
    private float HitStopTimer = 0f;

    void Awake()
    {
        if (Animator == null)
            Animator = GetComponent<Animator>();
    }

    public void DoHitStop()
    {
        if (Animator == null) return;
        Animator.speed = 0f;
        HitStopTimer = HitStopDuration;
        //Debug.Log("Animator paused");
    }

    void Update()
    {
        if (HitStopTimer > 0)
        {
            HitStopTimer -= Time.deltaTime;
            if (HitStopTimer <= 0)
            {
                Animator.speed = 1f;
                //Debug.Log("Animator resumed");
            }
        }
    }
}