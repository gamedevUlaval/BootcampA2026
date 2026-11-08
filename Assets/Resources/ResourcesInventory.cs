using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesInventory : MonoBehaviour
{
    [Serializable]
    public class ResourceEntry
    {
        public ResourcesType resourceType;
        public int value;
    }

    public List<ResourceEntry> inventory = new List<ResourceEntry>();

    public bool AddResource(ResourcesType resourceType, int value)
    {
        ResourceEntry entry = inventory.Find(x => x.resourceType == resourceType);

        if (entry == null)
        {
            entry = new ResourceEntry { resourceType = resourceType };
            inventory.Add(entry);
        }

        entry.value += value;
        return true;
    }
}
