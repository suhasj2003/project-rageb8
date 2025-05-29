using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemy/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public int MaxHP;
    public float MoveSpeed;
    public int AttackDamage;
    public float AttackCooldown;
    public float AttackRange;
}