using UnityEngine;

public class EnemyAIDemon : MonoBehaviour
{
    public Transform player;

    void Awake()
    {
        if (!player) player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (!player) return;

        FacePlayer();
    }

    private void FacePlayer()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Sign(player.position.x - transform.position.x) * Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
}
