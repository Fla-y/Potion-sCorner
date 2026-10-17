using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(StationMenu))]
public class SleepConfirmation : MonoBehaviour
{

    [SerializeField] private UnityEvent onSleepConfirmed = new UnityEvent();
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
        
    }
    public void ConfirmSleep()
    {
        if (!isActiveAndEnabled || menu == null || !menu.IsOpen)
            return;

        menu.Close();

        Debug.Log("Riposo confermato: qui avviera' il ciclo di vendite e nuova notte.", this);
        onSleepConfirmed.Invoke();
    }
}
