using UnityEngine;

public class EnemyAIRephaimTannk : MonoBehaviour
{
    public EnemyData EnemyData;
    [Header("Lunge Settings")]
    [SerializeField] private float LungeSpeed = 15f;
    [SerializeField] private float LungeDuration = 0.2f;
    [SerializeField] private float PostLungeCooldown = 0.5f;

    private float LastAttackTime;
    private bool IsLunging;
    private Vector2 LungeTargetPosition;

    private BoxCollider2D AttackHitbox;
    private Rigidbody2D Body;
    private Animator Anim;
    private Transform Player;

    void Awake()
    {
        Body = GetComponent<Rigidbody2D>();
        Body.constraints = RigidbodyConstraints2D.FreezeRotation;
        Anim = GetComponent<Animator>();
        AttackHitbox = transform.Find("AttackHitbox").GetComponent<BoxCollider2D>();
        Player = GameObject.FindWithTag("Player").transform;
    }

    void Start()
    {
        AttackHitbox.gameObject.SetActive(false);
        Anim.SetBool("CanMove", true);
    }

    void Update()
    {
        if (Player == null || IsLunging || !Anim.GetBool("CanMove")) return;

        float distance = Vector2.Distance(transform.position, Player.position);
        UpdateFacingDirection();

        if (distance <= EnemyData.AlertRange)
        {
            Anim.SetTrigger("IsAlert");
            if (distance <= EnemyData.AttackRange)
            {

                if (Time.time >= LastAttackTime + EnemyData.AttackCooldown)
                {
                    StartLungeAttack();
                }
            }
        }
    }

    private void StartLungeAttack()
    {
        IsLunging = true;
        Anim.SetBool("CanMove", false);
        LungeTargetPosition = Player.position;
        LastAttackTime = Time.time;

        LungeMovement();
    }

    private void LungeMovement()
    {
        Anim.SetTrigger("Lunge");

        float timer = 0;
        Vector2 startPosition = transform.position;

        while (timer < LungeDuration)
        {
            transform.position = Vector2.Lerp(startPosition, LungeTargetPosition, timer / LungeDuration);
            timer += Time.deltaTime;
        }

        transform.position = LungeTargetPosition;

        Anim.SetTrigger("Attack");
    }

    private void UpdateFacingDirection()
    {
        if (Player.position.x > transform.position.x)
            LeanTween.rotateY(gameObject, 0, 0).setEaseInOutSine();
        else
            LeanTween.rotateY(gameObject, 180, 0).setEaseInOutSine();
    }

    private void EndAttack()
    {
        IsLunging = false;
        Anim.SetBool("CanMove", true); 
    }

    private void ActivateAttackHitbox()
    {
        AttackHitbox.gameObject.SetActive(true);
    }

    private void DeactivateAttackHitbox()
    {
        AttackHitbox.gameObject.SetActive(false);
    }
}