using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitsController : MonoBehaviour
{
    [SerializeField] private GameObject rockFollowIndicator, paperFollowIndicator, scissorsFollowIndicator;

    private AiUnitDetector detector;

    private Dictionary<UnitType, List<AiUnit>> followingUnits;
    private Dictionary<UnitType, GameObject> folllowIndicators;
    private Dictionary<UnitType, bool> folllowToggles;


    private void Awake()
    {
        followingUnits = new Dictionary<UnitType, List<AiUnit>>();
        folllowIndicators = new Dictionary<UnitType, GameObject>();
        folllowToggles = new Dictionary<UnitType, bool>();

        foreach (UnitType type in (UnitType[])Enum.GetValues(typeof(UnitType)))
        {
            followingUnits.Add(type, new List<AiUnit>());
            folllowToggles.Add(type, false);
        }

        folllowIndicators.Add(UnitType.Rock, rockFollowIndicator);
        folllowIndicators.Add(UnitType.Paper, paperFollowIndicator);
        folllowIndicators.Add(UnitType.scissors, scissorsFollowIndicator);

        detector = GetComponentInChildren<AiUnitDetector>();
        detector.OnUnitDetected += OnUnitDetected;
    }

    private void OnUnitDetected(AiUnit unit)
    {
        if (folllowToggles[unit.UnitType])
        {
            unit.FollowTarget(transform);
            followingUnits[unit.UnitType].Add(unit);
        }
    }

    private void Update()
    {
        ///// Temporary move inputs
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector3 movement = new Vector3(moveX, 0, moveY).normalized;
        transform.Translate(movement * 6 * Time.deltaTime, Space.World);
        /////

        if (Input.GetKeyDown(KeyCode.Q))
        {
            OnUnitTypeToggle(UnitType.Rock);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            OnUnitTypeToggle(UnitType.Paper);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            OnUnitTypeToggle(UnitType.scissors);
        }
    }

    private void OnUnitTypeToggle(UnitType type) 
    {
        List<AiUnit> detectedUnits = detector.UnitsInDetectionRange.Where(x => x.UnitType == type).ToList();

        if (detectedUnits.Count > 0 && !folllowToggles[type])
        {
            foreach (AiUnit unit in detectedUnits)
            {
                unit.FollowTarget(transform);
                followingUnits[unit.UnitType].Add(unit);
            }
            folllowIndicators[type].SetActive(true);
            folllowToggles[type] = true;
        }
        else
        {
            foreach (AiUnit unit in followingUnits[type])
            {
                unit.DefendPosition();
            }

            followingUnits[type].Clear();
            folllowIndicators[type].SetActive(false);
            folllowToggles[type] = false;
        }
    }
}