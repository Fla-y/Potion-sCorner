using UnityEngine;

public class InventoryDebugTool : MonoBehaviour
{

    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private ItemDefinition testItem;
    [SerializeField, Min(1)] private int amount = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Add Test Item (Play Mode)")]
    private void AddTestItem()
    {
        if (!CanRun())
            return;

        bool success = inventory.TryAdd(testItem, amount);
        Debug.Log(success ? "Test item added; new pages created if needed." : "Nothing added: invalid item, quantity or capacity.", this);
    }

    [ContextMenu("Remove Test Item (Play Mode)")]
    private void RemoveTestItem()
    {
        if (!CanRun())
            return;

        bool success = inventory.TryRemove(testItem, amount);
        Debug.Log(success ? "Test item removed." : "Nothing removed: insufficient quantity or invalid amount.", this);
    }

    private bool CanRun()
    {
        if (!Application.isPlaying || inventory == null || testItem == null)
        {
            Debug.LogWarning("Enter Play Mode and assign Inventory and Test Item first.", this);
            return false;
        }

        return true;
    }
}
