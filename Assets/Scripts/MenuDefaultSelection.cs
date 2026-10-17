using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class MenuDefaultSelection : MonoBehaviour
{

    [SerializeField] private Selectable firstSelected;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        StartCoroutine(SelectWhenReady());
    }

    private IEnumerator SelectWhenReady()
    {
        yield return null;

        while (IsSubmitHeld())
            yield return null;

        if (EventSystem.current == null)
        {
            Debug.LogWarning("MenuDefaultSelection: no active EventSystem.", this);
            yield break;
        }

        if (firstSelected != null && firstSelected.IsActive()
            && firstSelected.IsInteractable())
        {
            EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
        }
    }

    private static bool IsSubmitHeld()
    {
        Keyboard keyboard = Keyboard.current;
        Gamepad gamepad = Gamepad.current;

        return (keyboard != null &&
            (keyboard.enterKey.isPressed || keyboard.numpadEnterKey.isPressed
                || keyboard.spaceKey.isPressed))
            || (gamepad != null && gamepad.buttonSouth.isPressed);
    }

    private void OnDisable()
    {
        StopAllCoroutines();

        EventSystem eventSystem = EventSystem.current;
        if (eventSystem == null)
            return;

        GameObject selected = eventSystem.currentSelectedGameObject;
        if (selected != null && selected.transform.IsChildOf(transform))
            eventSystem.SetSelectedGameObject(null);
    }
}
