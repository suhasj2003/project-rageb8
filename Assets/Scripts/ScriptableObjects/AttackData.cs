using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackData", menuName = "Combat/Attack Data")]
public class AttackData : ScriptableObject
{
    public string AttackName;
    public int Damage;
    public float KnockbackForce;
    // Add more fields as needed
}