using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerInteractor : MonoBehaviour
{
    private readonly HashSet<Collider2D> nearbyZones = new HashSet<Collider2D>();

    public Interactable CurrentTarget { get; private set; }
    public bool InteractionEnabled { get; set; } = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        nearbyZones.RemoveWhere(zone =>
            zone == null || !zone.enabled || !zone.gameObject.activeInHierarchy);

        CurrentTarget = FindNearestTarget();

        if (InteractionEnabled && CurrentTarget != null && WasInteractPressed())
            CurrentTarget.Interact();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        RegisterZone(other);
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (enabled)
            RegisterZone(other);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        nearbyZones.Remove(other);
    }
    private void OnDisable()
    {
        nearbyZones.Clear();
        CurrentTarget = null;
    }

    private void RegisterZone(Collider2D other)
    {
        if (other.isTrigger && other.GetComponentInParent<Interactable>() != null)
            nearbyZones.Add(other);
    }

    private Interactable FindNearestTarget()
    {
        Interactable nearest = null;
        float nearestDistanceSquared = float.PositiveInfinity;
        Vector2 playerPosition = transform.position;

        foreach (Collider2D zone in nearbyZones)
        {
            Interactable candidate = zone.GetComponentInParent<Interactable>();

            if (candidate == null || !candidate.isActiveAndEnabled)
                continue;

            Vector2 stationPosition = candidate.transform.position;
            float distanceSquared = (stationPosition - playerPosition).sqrMagnitude;

            if (distanceSquared < nearestDistanceSquared)
            {
                nearest = candidate;
                nearestDistanceSquared = distanceSquared;
            }
        }

        return nearest;
    }

    private bool WasInteractPressed()
    {
        Keyboard keyboard = Keyboard.current;
        Gamepad gamepad = Gamepad.current;

        return (keyboard != null && keyboard.eKey.wasPressedThisFrame) || (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame);
    }
}
