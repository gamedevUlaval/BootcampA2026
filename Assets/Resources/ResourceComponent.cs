using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ResourceComponent : MonoBehaviour
{
    public ResourceScriptableObject resourceScriptableObject;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ResourcesInventory resourcesInventory))
        {
            if (resourceScriptableObject != null)
            {
                if (resourcesInventory.AddResource(resourceScriptableObject.resourcesType, resourceScriptableObject.value))
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                Debug.LogError("ResourceScriptableObject should be assigned to the ResourceComponent.");
            }
        }
    }
}
