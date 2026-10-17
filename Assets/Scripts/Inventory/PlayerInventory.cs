using System;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Serializable]
    private class StartingItem
    {
        public ItemDefinition item;
        [Min(1)] public int quantity = 1;
    }

    [Tooltip("Slots per page. Extra pages are allocated automatically when needed.")]
    [SerializeField, Min(1)] private int slotCount = 20;
    [SerializeField] private StartingItem[] startingItems = new StartingItem[0];

    private ItemDefinition[] items;
    private int[] quantities;
    private bool initializing;

    public event Action Changed;
    public int SlotsPerPage => Mathf.Max(1, slotCount);
    public int SlotCount => items != null ? items.Length : SlotsPerPage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        items = new ItemDefinition[SlotsPerPage];
        quantities = new int[items.Length];
        initializing = true;

        if (startingItems != null)
        {
            foreach (StartingItem entry in startingItems)
            {
                if (entry == null || !TryAdd(entry.item, entry.quantity))
                    Debug.LogWarning("PlayerInventory: a starting item could not be added. Check item and quantity.", this);
            }
        }

        initializing = false;
        Changed?.Invoke();
    }

    public ItemDefinition GetItemAt(int index)
    {
        return items != null && index >= 0 && index < items.Length ? items[index] : null;
    }

    public int GetQuantityAt(int index)
    {
        return quantities != null && index >= 0 && index < quantities.Length ? quantities[index] : 0;
    }

    public long GetQuantity(ItemDefinition item)
    {
        if (items == null || item == null)
            return 0;

        long total = 0;
        for (int i = 0; i < items.Length; i++)
            if (items[i] == item)
                total += quantities[i];

        return total;
    }

    public bool CanAdd(ItemDefinition item, int quantity)
    {
        if (items == null || item == null || quantity <= 0)
            return false;

        return RequiredCapacity(item, quantity) <= int.MaxValue;
    }

    private long RequiredCapacity(ItemDefinition item, int quantity)
    {
        long available = 0;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
                available += item.MaxStack;
            else if (items[i] == item)
                available += Math.Max(0, item.MaxStack - quantities[i]);
        }

        long missing = Math.Max(0L, (long)quantity - available);
        long extraSlots = (missing + item.MaxStack - 1L) / item.MaxStack;
        long extraPages = (extraSlots + SlotsPerPage - 1L) / SlotsPerPage;
        return items.Length + extraPages * SlotsPerPage;
    }

    public bool TryAdd(ItemDefinition item, int quantity)
    {
        if (!CanAdd(item, quantity))
            return false;

        int requiredCapacity = (int)RequiredCapacity(item, quantity);
        if (requiredCapacity > items.Length)
        {
            ItemDefinition[] expandedItems = new ItemDefinition[requiredCapacity];
            int[] expandedQuantities = new int[requiredCapacity];
            Array.Copy(items, expandedItems, items.Length);
            Array.Copy(quantities, expandedQuantities, quantities.Length);
            items = expandedItems;
            quantities = expandedQuantities;
        }

        int remaining = quantity;
        for (int i = 0; i < items.Length && remaining > 0; i++)
        {
            if (items[i] != item)
                continue;

            int added = Math.Min(remaining, Math.Max(0, item.MaxStack - quantities[i]));
            quantities[i] += added;
            remaining -= added;
        }

        for (int i = 0; i < items.Length && remaining > 0; i++)
        {
            if (items[i] != null)
                continue;

            int added = Math.Min(remaining, item.MaxStack);
            items[i] = item;
            quantities[i] = added;
            remaining -= added;
        }

        NotifyChanged();
        return true;
    }

    public bool TryRemove(ItemDefinition item, int quantity)
    {
        if (item == null || quantity <= 0 || GetQuantity(item) < quantity)
            return false;

        int remaining = quantity;
        for (int i = 0; i < items.Length && remaining > 0; i++)
        {
            if (items[i] != item)
                continue;

            int removed = Math.Min(remaining, quantities[i]);
            quantities[i] -= removed;
            remaining -= removed;

            if (quantities[i] == 0)
                items[i] = null;
        }

        NotifyChanged();
        return true;
    }

    private void NotifyChanged()
    {
        if (!initializing)
            Changed?.Invoke();
    }

    public int PageCount
    {
        get
        {
            if (items != null)
                for (int i = items.Length - 1; i >= 0; i--)
                    if (items[i] != null)
                        return i / SlotsPerPage + 1;

            return 1;
        }
    }
}
