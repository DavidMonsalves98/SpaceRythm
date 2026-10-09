using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISpaceship : MonoBehaviour
{
    [SerializeField] private VidaNave spaceship;
    [SerializeField] private TMP_Text lifeText;
    [SerializeField] private Slider lifeBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (spaceship == null || lifeText == null || lifeBar == null)
        {
            Debug.LogError("One or more required components are missing.");
            enabled = false;
            return;
        }
        lifeBar.minValue = 0;
        lifeBar.maxValue = spaceship.Life;
        lifeBar.wholeNumbers = true;
        lifeBar.interactable = false;

        spaceship.OnLifeChanged += UpdateUI;

        UpdateUI();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        if (spaceship != null)
        {
            spaceship.OnLifeChanged -= UpdateUI;
        }
    }

    private void UpdateUI()
    {
        lifeBar.value = spaceship.CurrentLife;
        string state_spacship;

        if(spaceship.IsDestroyed)
        {
            state_spacship = "Destroyed";
        }
        else
        {
            state_spacship = "Alive";
        }

        lifeText.text = $"Life: {spaceship.CurrentLife}/{spaceship.Life}\n " +
            $"State: {state_spacship}";
    }

}
