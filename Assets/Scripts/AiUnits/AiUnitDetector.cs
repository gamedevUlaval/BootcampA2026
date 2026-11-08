using System;
using System.Collections.Generic;
using UnityEngine;

public class AiUnitDetector : MonoBehaviour
{
    public Action<AiUnit> OnUnitDetected;
    public Action<AiUnit> OnUnitLost;

    public List<AiUnit> UnitsInDetectionRange;

    private AiUnit owner;

    private void Awake()
    {
        UnitsInDetectionRange = new List<AiUnit>();
        owner = GetComponentInParent<AiUnit>();
    }

    private void OnDisable()
    {
        UnitsInDetectionRange.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        AiUnit unit = other.GetComponent<AiUnit>();

        if (unit != null && IsEnemy(unit) && !UnitsInDetectionRange.Contains(unit))
        {
            UnitsInDetectionRange.Add(unit);
            OnUnitDetected?.Invoke(unit);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        AiUnit unit = other.GetComponent<AiUnit>();

        if (unit != null && UnitsInDetectionRange.Contains(unit))
        {
            UnitsInDetectionRange.Remove(unit);
            OnUnitLost?.Invoke(unit);
        }
    }

    private bool IsEnemy(AiUnit unit)
    {
        return owner == null || unit.Team != owner.Team;
    }
}
