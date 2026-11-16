using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ResourceSpawner))]
public class ResourceCollector : MonoBehaviour
{
    [Tooltip("Seconds between collection cycles.")]
    public float collectInterval = 2f;

    [Tooltip("Max resources that start moving toward the collector per cycle.")]
    public int resourcesPerCycle = 3;

    [Tooltip("Total resources this collector can ever collect. 0 = unlimited.")]
    public int maxTotalCollected = 0;

    [Header("Attraction")]
    public float attractSpeed = 10f;
    public float collectDistance = 1f;

    private readonly List<ResourceComponent> incoming = new List<ResourceComponent>();

    private ResourceSpawner spawner;
    private ResourcesInventory owner;
    private float timer;
    private int totalCollected;

    private bool CapReached => maxTotalCollected > 0 && totalCollected + incoming.Count >= maxTotalCollected;

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
        if (timer >= collectInterval)
        {
            timer = 0f;
            StartCycle();
        }

        MoveIncoming();
    }

    private void StartCycle()
    {
        int started = 0;
        IReadOnlyList<GameObject> spawned = spawner.SpawnedResources;

        for (int i = 0; i < spawned.Count && started < resourcesPerCycle && !CapReached; i++)
        {
            GameObject obj = spawned[i];
            if (obj == null) continue;

            if (!obj.TryGetComponent(out ResourceComponent resource) || resource.resourceScriptableObject == null)
                continue;

            if (incoming.Contains(resource)) continue;

            incoming.Add(resource);
            started++;
        }
    }

    private void MoveIncoming()
    {
        for (int i = incoming.Count - 1; i >= 0; i--)
        {
            ResourceComponent resource = incoming[i];

            if (resource == null)
            {
                incoming.RemoveAt(i);
                continue;
            }

            resource.transform.position = Vector3.MoveTowards(
                resource.transform.position, transform.position, attractSpeed * Time.deltaTime);

            if (Vector3.Distance(resource.transform.position, transform.position) <= collectDistance)
            {
                Collect(resource);
                incoming.RemoveAt(i);
            }
        }

        if (maxTotalCollected > 0 && totalCollected >= maxTotalCollected && incoming.Count == 0)
            enabled = false;
    }

    private void Collect(ResourceComponent resource)
    {
        owner.AddResource(resource.resourceScriptableObject.resourcesType, resource.resourceScriptableObject.value);
        Destroy(resource.gameObject);
        totalCollected++;
    }
}