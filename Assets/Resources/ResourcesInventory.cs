using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesInventory : MonoBehaviour
{
    public List<ResourceEntry> inventory = new List<ResourceEntry>();

    public event Action OnChanged;

    public bool AddResource(ResourcesType resourceType, int value)
    {
        ResourceEntry entry = inventory.Find(x => x.resourceType == resourceType);

        if (entry == null)
        {
            entry = new ResourceEntry { resourceType = resourceType };
            inventory.Add(entry);
        }

        entry.value += value;
        OnChanged?.Invoke();
        return true;
    }

    public void NotifyChanged()
    {
        OnChanged?.Invoke();
    }
}
