using System.Text;
using TMPro;
using UnityEngine;

public class ResourcesHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text label;

    private ResourcesInventory inventory;

    public void SetInventory(ResourcesInventory newInventory)
    {
        if (inventory != null) inventory.OnChanged -= Refresh;

        inventory = newInventory;

        if (inventory != null) inventory.OnChanged += Refresh;

        Refresh();
    }

    private void OnDestroy()
    {
        if (inventory != null) inventory.OnChanged -= Refresh;
    }

    private void Refresh()
    {
        if (label == null) return;

        if (inventory == null)
        {
            label.text = string.Empty;
            return;
        }

        StringBuilder sb = new StringBuilder();
        foreach (ResourceEntry entry in inventory.inventory)
        {
            sb.Append(entry.resourceType).Append(": ").Append(entry.value).AppendLine();
        }

        label.text = sb.ToString();
    }
}