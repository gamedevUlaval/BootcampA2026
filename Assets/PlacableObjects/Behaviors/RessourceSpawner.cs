using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Placable))]
public class RessourceSpawner : MonoBehaviour
{
    public ResourceScriptableObject resource;

    [Tooltip("Time delay between spawns in seconds.")]
    public float spawnInterval = 1.0f;

    [Tooltip("Minimum distance from this object where resources can spawn.")]
    public float minSpawnRadius = 1.0f;

    [Tooltip("Maximum distance from this object where resources can spawn.")]
    public float maxSpawnRadius = 2.0f;
    public int maxSpawnedResourcesCount = 10;

    private readonly List<GameObject> spawnedResourcesList = new List<GameObject>();

    void Start()
    {
        InvokeRepeating(nameof(spawnResource), 0f, spawnInterval);
    }

    private void spawnResource()
    {
        if (resource == null || resource.ressourcePrefab == null)
        {
            Debug.LogWarning("ressource spawner has no prefab assigned to spawn");
            return;
        }

        spawnedResourcesList.RemoveAll(resource => resource == null);

        if (spawnedResourcesList.Count >= maxSpawnedResourcesCount)
            return;

        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Mathf.Sqrt(Random.Range(minSpawnRadius * minSpawnRadius, maxSpawnRadius * maxSpawnRadius));

        Vector3 offset = new Vector3(Mathf.Cos(angle) * distance, 0f, Mathf.Sin(angle) * distance);
        Vector3 spawnPosition = transform.position + offset;

        GameObject spawned = Instantiate(resource.ressourcePrefab, spawnPosition, Quaternion.identity);
        spawnedResourcesList.Add(spawned);
    }

    void OnValidate()
    {
        minSpawnRadius = Mathf.Max(0f, minSpawnRadius);
        maxSpawnRadius = Mathf.Max(minSpawnRadius, maxSpawnRadius);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        DrawCircle(transform.position, minSpawnRadius);
        Gizmos.color = Color.green;
        DrawCircle(transform.position, maxSpawnRadius);
    }

    private void DrawCircle(Vector3 center, float radius, int segments = 48)
    {
        Vector3 prev = center + new Vector3(radius, 0f, 0f);
        for (int i = 1; i <= segments; i++)
        {
            float a = (i / (float)segments) * Mathf.PI * 2f;
            Vector3 next = center + new Vector3(Mathf.Cos(a) * radius, 0f, Mathf.Sin(a) * radius);
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
}