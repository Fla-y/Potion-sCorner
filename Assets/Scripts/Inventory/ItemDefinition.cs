using UnityEngine;

public enum ItemCategory
{
    Ingredient,
    Potion
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Potions Corner/Item")]
public class ItemDefinition : ScriptableObject
{
    [SerializeField] private string displayName;
    [SerializeField, TextArea] private string description;
    [SerializeField] private Sprite icon;
    [SerializeField] private ItemCategory category;
    [SerializeField, Min(1)] private int maxStack = 20;

    public string DisplayName => displayName;
    public string Description => description;
    public Sprite Icon => icon;
    public ItemCategory Category => category;
    public int MaxStack => Mathf.Max(1, maxStack);
}
