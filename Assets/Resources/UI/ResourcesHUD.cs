using System.Text;
using TMPro;
using UnityEngine;

public class ResourcesHUD : MonoBehaviour
{
    [SerializeField] private int playerIndex = 0;
    [SerializeField] private TMP_Text label;

    public int PlayerIndex => playerIndex;

    private ResourcesInventory inventory;

    private void Awake()
    {
        if (label == null) label = GetComponent<TMP_Text>();
        Refresh();
    }

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
        sb.Append("P").Append(playerIndex + 1).AppendLine();
        foreach (ResourceEntry entry in inventory.inventory)
        {
            sb.Append(entry.resourceType).Append(": ").Append(entry.value).AppendLine();
        }

        label.text = sb.ToString();
    }
}