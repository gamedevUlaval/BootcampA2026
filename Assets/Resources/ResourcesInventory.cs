using Mono.Cecil;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class ResourcesInventory : MonoBehaviour
{
    public List<ResourceEntry> inventory;

    public bool AddResource(ResourcesType resourceType, int value)
    {
        ResourceEntry resourceEntryToModify = inventory.Find(x => x.resourceType == resourceType);

        if (resourceEntryToModify != null)
        {
            resourceEntryToModify.value += value;
            return true;
        }
        else
        {
            Debug.Log("Cannot pickup because resource type not found in inventory.");
            return false;
        }
    }
}
