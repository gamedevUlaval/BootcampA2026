using System;
using Unity.Mathematics;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private bool destroyOnDeath = true;

    private float maxPoints;
    private float currentPoints;

    public UnitType Type = UnitType.None;

    public float CurrentHealthPoints 
    {
        get => currentPoints;
        set 
        {
            float pointsBefore = currentPoints;
            currentPoints = math.clamp(value, 0, maxPoints);

            if (pointsBefore != currentPoints)
            {
                OnPointsChanged?.Invoke(currentPoints);

                if (IsDead)
                {
                    OnDeath?.Invoke();
                    if (destroyOnDeath)
                        Destroy(gameObject);
                }
                    
            }
        }
    }

    public Action OnDeath;
    public Action<float> OnPointsChanged;
    public bool IsDead => currentPoints <= 0;

    public void Initialize(float maxPoints, UnitType type)
    {
        this.maxPoints = maxPoints;
        CurrentHealthPoints = maxPoints;
        Type = type;
    }
}