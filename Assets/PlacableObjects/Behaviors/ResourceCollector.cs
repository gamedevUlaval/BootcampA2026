using UnityEngine;

[RequireComponent(typeof(ResourceSpawner))]
public class ResourceCollector : MonoBehaviour
{
    [Tooltip("Seconds between collection cycles.")]
    public float collectInterval = 2f;

    [Tooltip("Max resources collected per cycle.")]
    public int resourcesPerCycle = 1;

    private ResourceSpawner spawner;
    private ResourcesInventory owner;
    private float timer;
    private int totalCollected;

    private void Awake()
    {
        spawner = GetComponent<ResourceSpawner>();
    }

    public void SetOwner(ResourcesInventory newOwner)
    {
        owner = newOwner;
    }

    private void Update()
    {
        if (owner == null) return;

        timer += Time.deltaTime;
        if (timer < collectInterval) return;
        timer = 0f;

        CollectCycle();
    }

    private void CollectCycle()
    {
        int collectedThisCycle = 0;
        var spawned = spawner.SpawnedResources;

        for (int i = 0; i < spawned.Count && collectedThisCycle < resourcesPerCycle; i++)
        {
            GameObject obj = spawned[i];
            if (obj == null) continue;

            if (!obj.TryGetComponent(out ResourceComponent resource) || resource.resourceScriptableObject == null)
                continue;

            owner.AddResource(resource.resourceScriptableObject.resourcesType, resource.resourceScriptableObject.value);
            Destroy(obj);

            collectedThisCycle++;
            totalCollected++;
        }
    }
}