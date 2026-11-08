using System.Collections.Generic;
using UnityEngine;

public class ResourceAttractor : MonoBehaviour
{
    public float attractSpeed = 10f;
    public float collectDistance = 1f;

    private readonly List<ResourceComponent> resourcesInRange = new List<ResourceComponent>();
    private ResourcesInventory inventory;

    private void Awake()
    {
        inventory = GetComponentInParent<ResourcesInventory>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ResourceComponent resource) && !resourcesInRange.Contains(resource))
            resourcesInRange.Add(resource);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out ResourceComponent resource))
            resourcesInRange.Remove(resource);
    }

    private void Update()
    {
        for (int i = resourcesInRange.Count - 1; i >= 0; i--)
        {
            ResourceComponent resource = resourcesInRange[i];

            if (resource == null)
            {
                resourcesInRange.RemoveAt(i);
                continue;
            }

            resource.transform.position = Vector3.MoveTowards(
                resource.transform.position, transform.position, attractSpeed * Time.deltaTime);

            if (Vector3.Distance(resource.transform.position, transform.position) <= collectDistance)
            {
                Collect(resource);
                resourcesInRange.RemoveAt(i);
            }
        }
    }

    private void Collect(ResourceComponent resource)
    {
        if (inventory == null || resource.resourceScriptableObject == null)
        {
            Debug.LogError("ResourceAttractor needs a ResourcesInventory in its parent and a resource data asset.");
            return;
        }

        inventory.AddResource(resource.resourceScriptableObject.resourcesType, resource.resourceScriptableObject.value);
        Destroy(resource.gameObject);
    }

    private void OnValidate()
    {
        attractSpeed = Mathf.Max(0f, attractSpeed);
        collectDistance = Mathf.Max(0f, collectDistance);
    }
}
