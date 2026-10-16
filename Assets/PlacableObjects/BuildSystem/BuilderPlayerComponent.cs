using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ResourcesInventory))]
public class BuilderPlayerComponent : MonoBehaviour
{
    public List<BuildableBlueprintScriptableObject> availableBlueprints = new List<BuildableBlueprintScriptableObject>();

    [Tooltip("Distance in front of the player where the building spawn")]
    [SerializeField] private float buildDistance = 2f;

    private PlayerController playerController;
    private BuildMenuUI menu;

    public event Action<BuildableBlueprintScriptableObject> OnBuilt;

    private ResourcesInventory inventory;

    public void Start()
    {
        playerController = GetComponent<PlayerController>();

        inventory = GetComponent<ResourcesInventory>();

        menu = FindFirstObjectByType<BuildMenuUI>();
        if (menu != null) menu.SetBuilder(this);
    }

    private void OnBuildMenu(InputValue value)
    {
        if (!value.isPressed || menu == null) return;

        menu.Toggle();

        if (playerController != null) playerController.movementLocked = menu.IsOpen;
    }

    public bool TryBuild(BuildableBlueprintScriptableObject blueprint)
    {
        if (inventory == null)
        {
            Debug.LogError("Could not build because the player misses the ResourcesInventory component", this);
            return false;
        }

        if (blueprint == null || blueprint.prefab == null)
        {
            Debug.LogWarning("Could not build: blueprint or its prefab is null", this);
            return false;
        }

        if (!IsBuildable(blueprint))
        {
            return false;
        }

        ConsumeResources(blueprint);

        GetSpawnPose(out Vector3 position, out Quaternion rotation);
        Instantiate(blueprint.prefab, position, rotation);

        OnBuilt?.Invoke(blueprint);
        return true;
    }

    public bool IsBuildable(BuildableBlueprintScriptableObject blueprint)
    {
        if (inventory == null || blueprint == null)
        {
            return false;
        }

        foreach (ResourceEntry cost in blueprint.recipe)
        {
            int owned = 0;
            foreach (ResourceEntry entry in inventory.inventory)
            {
                if (entry.resourceType == cost.resourceType)
                {
                    owned += entry.value;
                }
            }

            if (owned < cost.value)
            {
                return false;
            }
        }

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
                if (entry.resourceType != cost.resourceType)
                {
                    continue;
                }

                int taken = Mathf.Min(entry.value, remaining);
                entry.value -= taken;
                inventory.inventory[i] = entry;
                remaining -= taken;
            }
        }
    }

    private void GetSpawnPose(out Vector3 position, out Quaternion rotation)
    {
        position = transform.position + transform.forward * buildDistance;
        rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
    }
}