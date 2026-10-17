using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private Transform slotsRoot;
    [SerializeField] private InventorySlotUI slotPrefab;

    [Header("Pagination")]
    [SerializeField] private TMP_Text pageText;
    [SerializeField] private Button previousPageButton;
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Selectable fallbackSelection;

    private InventorySlotUI[] views;
    private PlayerInventory subscribedInventory;
    private int currentPage;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (subscribedInventory == null)
            return;

        Keyboard keyboard = Keyboard.current;
        Gamepad gamepad = Gamepad.current;

        bool previous = (keyboard != null && keyboard.pageUpKey.wasPressedThisFrame) || (gamepad != null && gamepad.leftShoulder.wasPressedThisFrame);
        bool next = (keyboard != null && keyboard.pageDownKey.wasPressedThisFrame) || (gamepad != null && gamepad.rightShoulder.wasPressedThisFrame);

        if (previous)
            PreviousPage();
        else if (next)
            NextPage();
    }
    private void OnEnable()
    {
        if (inventory == null || slotsRoot == null || slotPrefab == null)
        {
            Debug.LogError("InventoryUI: assign Inventory, Slots Root and Slot Prefab.", this);
            return;
        }

        if (views == null)
        {
            if (slotsRoot.childCount != 0)
            {
                Debug.LogError("InventoryUI: SlotsGrid must be empty in the scene. Save the slot prefab, then remove the preview slots from the grid.", this);
                return;
            }

            views = new InventorySlotUI[inventory.SlotsPerPage];
            for (int i = 0; i < views.Length; i++)
            {
                views[i] = Instantiate(slotPrefab, slotsRoot, false);
                views[i].name = $"Slot_{i + 1:00}";
                views[i].gameObject.SetActive(true);
            }
        }

        subscribedInventory = inventory;
        subscribedInventory.Changed += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (subscribedInventory != null)
            subscribedInventory.Changed -= Refresh;

        subscribedInventory = null;
    }

    public void Refresh()
    {
        if (views == null || inventory == null)
            return;

        int pages = inventory.PageCount;
        currentPage = Mathf.Clamp(currentPage, 0, pages - 1);
        int offset = currentPage * inventory.SlotsPerPage;

        for (int i = 0; i < views.Length; i++)
        {
            int index = offset + i;
            views[i].Display(inventory.GetItemAt(index), inventory.GetQuantityAt(index));
        }

        if (pageText != null)
            pageText.text = $"Pagina {currentPage + 1} / {pages}";
        if (previousPageButton != null)
            previousPageButton.interactable = currentPage > 0;
        if (nextPageButton != null)
            nextPageButton.interactable = currentPage < pages - 1;

        EventSystem events = EventSystem.current;
        if (events == null)
            return;

        GameObject selected = events.currentSelectedGameObject;
        bool disabledPageSelected =
            (previousPageButton != null && selected == previousPageButton.gameObject
                && !previousPageButton.interactable)
            || (nextPageButton != null && selected == nextPageButton.gameObject
                && !nextPageButton.interactable);

        if (disabledPageSelected)
        {
            Selectable target = null;
            if (previousPageButton != null && previousPageButton.IsInteractable())
                target = previousPageButton;
            else if (nextPageButton != null && nextPageButton.IsInteractable())
                target = nextPageButton;
            else if (fallbackSelection != null && fallbackSelection.IsActive()
                && fallbackSelection.IsInteractable())
                target = fallbackSelection;

            events.SetSelectedGameObject(target != null ? target.gameObject : null);
        }
    }
    public void PreviousPage()
    {
        if (!isActiveAndEnabled || inventory == null)
            return;

        currentPage = Mathf.Max(0, currentPage - 1);
        Refresh();
    }

    public void NextPage()
    {
        if (!isActiveAndEnabled || inventory == null)
            return;

        currentPage = Mathf.Min(inventory.PageCount - 1, currentPage + 1);
        Refresh();
    }
}
