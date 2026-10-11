using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] private string prompt = "Open catalog";
    [SerializeField] private UnityEvent onInteract = new UnityEvent();
    public string Prompt => prompt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Interact()
    {
        if (!isActiveAndEnabled)
            return;

        Debug.Log($"{gameObject.name}: {prompt}", this);

        onInteract.Invoke();
    }
}
