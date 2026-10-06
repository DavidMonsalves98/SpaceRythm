using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class PruebaVidaNave : MonoBehaviour
{

    [SerializeField] private VidaNave spaceship;
    [SerializeField] private TMP_Text stateText;
    [SerializeField] private int damagePerHit = 20;

    [SerializeField] private Slider lifebar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (spaceship == null || stateText == null || lifebar == null)
        {
            Debug.LogError(
                "One or more required components are not assigned in the Inspector.", this
            );

            enabled = false;
            return;
        }

        lifebar.minValue = 0;
        lifebar.maxValue = spaceship.Life;
        lifebar.wholeNumbers = true;
        lifebar.interactable = false;

        spaceship.OnLifeChanged += UpdateUI;

        UpdateUI();
    }

    private void OnDestroy()
    {
        if (spaceship != null)
        {
            spaceship.OnLifeChanged -= UpdateUI;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            Debug.LogError("Keyboard is not available.");
            return;
        }

        if (keyboard.rKey.wasPressedThisFrame)
        {
            spaceship.RestoreLife();
            return;
        }

        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            spaceship.TakeDamage(damagePerHit);
        }
    }

    private void UpdateUI()
    {
        lifebar.value = spaceship.CurrentLife;
        string state;

        if (spaceship.IsDestroyed)
        {
            state = "Destroyed";
        }
        else
        {
            state = "Operational";
        }

        stateText.text = $"Life: {spaceship.CurrentLife}/{spaceship.Life}\n" +
            $"{state}\n" +
            "Space: Take Damage | R: Restore Life";
    }

}
