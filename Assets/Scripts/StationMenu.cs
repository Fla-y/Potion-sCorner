using UnityEngine;
using UnityEngine.InputSystem;


public class StationMenu : MonoBehaviour
{

    [SerializeField] private GameObject menuRoot;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerInteractor playerInteraction;

    private bool previousMovementEnabled;
    private bool previousInteractionEnabled;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        if (menuRoot == null || playerMovement == null || playerInteraction == null)
        {
            Debug.LogError("StationMenu: assign Menu Root, Player Movement and Player Interaction.", this);
            enabled = false;
            return;
        }

        if (menuRoot == gameObject || transform.IsChildOf(menuRoot.transform))
        {
            Debug.LogError("StationMenu must be outside Menu Root so it stays active when the menu is hidden.", this);
            enabled = false;
            return;
        }

        menuRoot.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOpen)
            return;

        Keyboard keyboard = Keyboard.current;
        Gamepad gamepad = Gamepad.current;

        if ((keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
            || (gamepad != null && gamepad.buttonEast.wasPressedThisFrame))
        {
            Close();
        }

    }

    public void Open()
    {
        if (!isActiveAndEnabled || IsOpen || !playerInteraction.InteractionEnabled)
            return;

        previousMovementEnabled = playerMovement.MovementEnabled;
        previousInteractionEnabled = playerInteraction.InteractionEnabled;

        playerInteraction.InteractionEnabled = false;
        playerMovement.SetMovementEnabled(false);

        IsOpen = true;
        menuRoot.SetActive(true);
    }

    public void Close()
    {
        if (!IsOpen)
            return;

        IsOpen = false;
        menuRoot.SetActive(false);

        if (playerMovement != null)
            playerMovement.SetMovementEnabled(previousMovementEnabled);

        if (playerInteraction != null)
            playerInteraction.InteractionEnabled = previousInteractionEnabled;
    }

    private void OnDisable()
    {
        Close();
    }
}
