using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(StationMenu))]
public class InventoryInput : MonoBehaviour
{

    [SerializeField] private PlayerInteractor playerInteraction;
    private StationMenu menu;

    private void Awake()
    {
        menu = GetComponent<StationMenu>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        Gamepad gamepad = Gamepad.current;
        bool pressed = (keyboard != null && keyboard.iKey.wasPressedThisFrame) || (gamepad != null && gamepad.buttonNorth.wasPressedThisFrame);

        if (!pressed || playerInteraction == null)
            return;

        if (menu.IsOpen)
            menu.Close();
        else if (playerInteraction.InteractionEnabled)
            menu.Open();
    }
}
