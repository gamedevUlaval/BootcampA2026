using UnityEngine;

[RequireComponent(typeof(Placable))]
public class RessourceSpawner : MonoBehaviour
{
    public ResourceScriptableObject resource;

    [Tooltip("Time delay between spawns in seconds.")]
    public float spawnInterval = 1.0f;

    [Tooltip("Maximum distance from this object where resources can spawn.")]
    public float spawnRadius = 5.0f;

    void Start()
    {
        InvokeRepeating(nameof(spawnResource), 0f, spawnInterval);
    }

    private void spawnResource()
    {
        GameObject prefabToSpawn = resource.ressourcePrefab;
        if (resource.ressourcePrefab == null)
        {
            Debug.LogWarning("ressource spawner has no prefab assigned to spawn");
            return;
        }

        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }
}