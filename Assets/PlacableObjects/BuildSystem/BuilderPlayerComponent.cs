using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ResourcesInventory))]
[RequireComponent(typeof(PlayerInput))]
public class BuilderPlayerComponent : MonoBehaviour
{
    [SerializeField] private List<BuildableBlueprintScriptableObject> availableBlueprints = new List<BuildableBlueprintScriptableObject>();

    [Tooltip("Where the building appears. If empty, it spawns in front of the player.")]
    [SerializeField] private Transform buildPoint;
    [SerializeField] private float fallbackBuildDistance = 2f;
    [SerializeField] private float groundOffset = 0.5f;
    [SerializeField] private float spawnerSearchRadius = 3f;

    public event Action<BuildableBlueprintScriptableObject> OnBuilt;

    public IReadOnlyList<BuildableBlueprintScriptableObject> AvailableBlueprints => availableBlueprints;
    public int PlayerIndex => playerInput != null ? playerInput.playerIndex : 0;

    private ResourcesInventory inventory;
    private PlayerInput playerInput;
    private PlayerController playerController;
    private BuildMenuUI menu;
    private ResourcesHUD hud;

    private void Awake()
    {
        inventory = GetComponent<ResourcesInventory>();
        playerInput = GetComponent<PlayerInput>();
        playerController = GetComponent<PlayerController>();
        menu = GetComponentInChildren<BuildMenuUI>(true);
    }

    private void Start()
    {
        foreach (ResourcesHUD candidate in FindObjectsByType<ResourcesHUD>(FindObjectsSortMode.None))
        {
            if (candidate.PlayerIndex == PlayerIndex)
            {
                hud = candidate;
                hud.SetInventory(inventory);
                break;
            }
        }
    }

    private void OnDestroy()
    {
        if (hud != null) hud.SetInventory(null);
        if (menu != null) Destroy(menu.gameObject);
    }

    private void OnBuildMenu(InputValue value)
    {
        if (!value.isPressed || menu == null) return;

        menu.Toggle();

        if (playerController != null) playerController.movementLocked = menu.IsOpen;
    }

    public bool TryBuild(BuildableBlueprintScriptableObject blueprint)
    {
        if (blueprint == null || blueprint.prefab == null) return false;
        if (!IsBuildable(blueprint)) return false;

        ResourceSpawner target = null;
        if (IsReplacementBlueprint(blueprint))
        {
            target = FindNearestMatchingSpawner(blueprint);
            if (target == null) return false;
        }

        ConsumeResources(blueprint);

        GameObject instance;
        if (target != null)
        {
            instance = ReplaceSpawner(blueprint, target);
        }
        else
        {
            GetSpawnPose(out Vector3 position, out Quaternion rotation);
            position.y += groundOffset;
            instance = Instantiate(blueprint.prefab, position, rotation);
        }

        if (instance.TryGetComponent(out ResourceCollector collector))
            collector.SetOwner(inventory);

        OnBuilt?.Invoke(blueprint);
        return true;
    }

    public bool IsBuildable(BuildableBlueprintScriptableObject blueprint)
    {
        if (inventory == null || blueprint == null) return false;

        foreach (ResourceEntry cost in blueprint.recipe)
        {
            int owned = 0;
            foreach (ResourceEntry entry in inventory.inventory)
            {
                if (entry.resourceType == cost.resourceType) owned += entry.value;
            }

            if (owned < cost.value) return false;
        }

        if (IsReplacementBlueprint(blueprint) && FindNearestMatchingSpawner(blueprint) == null)
            return false;

        return true;
    }

    private void ConsumeResources(BuildableBlueprintScriptableObject blueprint)
    {
        foreach (ResourceEntry cost in blueprint.recipe)
        {
            int remaining = cost.value;

            for (int i = 0; i < inventory.inventory.Count && remaining > 0; i++)
            {
                ResourceEntry entry = inventory.inventory[i];
                if (entry.resourceType != cost.resourceType) continue;

                int taken = Mathf.Min(entry.value, remaining);
                entry.value -= taken;
                remaining -= taken;
            }
        }

        inventory.NotifyChanged();
    }

    private void GetSpawnPose(out Vector3 position, out Quaternion rotation)
    {
        if (buildPoint != null)
        {
            position = buildPoint.position;
            rotation = buildPoint.rotation;
        }
        else
        {
            position = transform.position + transform.forward * fallbackBuildDistance;
            rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    private bool IsReplacementBlueprint(BuildableBlueprintScriptableObject blueprint)
    {
        return blueprint.prefab != null && blueprint.prefab.TryGetComponent<ResourceCollector>(out _);
    }

    private ResourceScriptableObject GetBlueprintResource(BuildableBlueprintScriptableObject blueprint)
    {
        if (blueprint.prefab != null && blueprint.prefab.TryGetComponent(out ResourceSpawner prefabSpawner))
            return prefabSpawner.resource;

        return null;
    }

    private ResourceSpawner FindNearestMatchingSpawner(BuildableBlueprintScriptableObject blueprint)
    {
        ResourceScriptableObject required = GetBlueprintResource(blueprint);
        if (required == null) return null;

        ResourceSpawner nearest = null;
        float bestSqr = spawnerSearchRadius * spawnerSearchRadius;

        foreach (ResourceSpawner spawner in FindObjectsByType<ResourceSpawner>(FindObjectsSortMode.None))
        {
            if (spawner.TryGetComponent<ResourceCollector>(out _)) continue;
            if (spawner.resource == null || spawner.resource.resourcesType != required.resourcesType) continue;

            float sqr = (spawner.transform.position - transform.position).sqrMagnitude;
            if (sqr <= bestSqr)
            {
                bestSqr = sqr;
                nearest = spawner;
            }
        }

        return nearest;
    }

    private GameObject ReplaceSpawner(BuildableBlueprintScriptableObject blueprint, ResourceSpawner old)
    {
        Vector3 position = old.transform.position;
        Quaternion rotation = old.transform.rotation;

        List<GameObject> existing = new List<GameObject>(old.SpawnedResources);

        Destroy(old.gameObject);

        GameObject instance = Instantiate(blueprint.prefab, position, rotation);

        if (instance.TryGetComponent(out ResourceSpawner newSpawner))
            newSpawner.AdoptResources(existing);

        return instance;
    }
}