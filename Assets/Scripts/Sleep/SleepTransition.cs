using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;
using System.Collections.Generic;

public class SleepTransition : MonoBehaviour
{
    [Tooltip("Lights that fade during sleep. Leave furnace and cauldron lights out.")]
    [SerializeField] private Light2D[] lightsToFade = new Light2D[0];
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerInteractor playerInteraction;

    [Header("Timing (seconds)")]
    [SerializeField, Min(0f)] private float fadeOutDuration = 1.5f;
    [SerializeField, Min(0f)] private float darkDuration = 2f;
    [SerializeField, Min(0f)] private float fadeInDuration = 1.5f;

    private bool isRunning;
    private readonly Dictionary<Light2D, float> originalIntensities = new Dictionary<Light2D, float>();
    private bool previousMovementEnabled;
    private bool previousInteractionEnabled;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BeginSleep()
    {
        if (!isActiveAndEnabled || isRunning)
            return;

        if (playerMovement == null || playerInteraction == null)
        {
            Debug.LogError("SleepTransition: assign Player Movement and Player Interaction.", this);
            return;
        }

        if (lightsToFade == null || lightsToFade.Length == 0)
        {
            Debug.LogError("SleepTransition: add at least one light to Lights To Fade.", this);
            return;
        }

        originalIntensities.Clear();
        for (int i = 0; i < lightsToFade.Length; i++)
        {
            Light2D light = lightsToFade[i];
            if (light == null)
            {
                Debug.LogError($"SleepTransition: Lights To Fade element {i} is empty.", this);
                return;
            }

            if (!originalIntensities.ContainsKey(light))
                originalIntensities.Add(light, light.intensity);
        }
        previousMovementEnabled = playerMovement.MovementEnabled;
        previousInteractionEnabled = playerInteraction.InteractionEnabled;

        isRunning = true;
        playerMovement.SetMovementEnabled(false);
        playerInteraction.InteractionEnabled = false;
        StartCoroutine(PlayTransition());
    }

    private IEnumerator PlayTransition()
    {
        yield return FadeLights(1f, 0f, fadeOutDuration);

        yield return new WaitForSecondsRealtime(darkDuration);

        yield return FadeLights(0f, 1f, fadeInDuration);
        RestoreState();
    }

    private IEnumerator FadeLights(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            SetLightLevel(Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t)));
            yield return null;
        }

        SetLightLevel(to);
    }

    private void SetLightLevel(float level)
    {
        foreach (KeyValuePair<Light2D, float> entry in originalIntensities)
        {
            if (entry.Key != null)
                entry.Key.intensity = entry.Value * level;
        }
    }

    private void RestoreState()
    {
        if (!isRunning)
            return;

        isRunning = false;

        SetLightLevel(1f);

        if (playerMovement != null)
            playerMovement.SetMovementEnabled(previousMovementEnabled);

        if (playerInteraction != null)
            playerInteraction.InteractionEnabled = previousInteractionEnabled;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        RestoreState();
    }
}
