using UnityEngine;

[CreateAssetMenu(fileName = "AiUnitStats", menuName = "Scriptable Objects/AiUnitStats")]
public class AiUnitStats : ScriptableObject
{
    public float MoveSpeed;
    public int HealthPoints;

    public float AttackDamage;
    public float AttackRange;

    [Tooltip("Number of attacks per second")]
    public float AttackSpeed;

    [Header("Rock Paper Scissors")]
    public float AdvantageMultiplier = 2f;
    public float DisadvantageMultiplier = 0.5f;
}