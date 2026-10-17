using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text quantityText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Display(ItemDefinition item, int quantity)
    {
        if (itemIcon == null || quantityText == null)
        {
            Debug.LogError("InventorySlotUI: assign Item Icon and Quantity Text on the slot prefab.", this);
            return;
        }

        bool occupied = item != null && quantity > 0;
        itemIcon.sprite = occupied ? item.Icon : null;
        itemIcon.enabled = occupied && item.Icon != null;
        quantityText.text = occupied ? quantity.ToString() : string.Empty;
    }
}
