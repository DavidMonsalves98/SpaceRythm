using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class RhythmicActionSignal : MonoBehaviour
{
    [SerializeField] private PruebaSecuenciaRitmica evaluator;
    [SerializeField] private Slider actionBar;
    [SerializeField] private TMP_Text signalText;

    [SerializeField] private double leadTime = 2.0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (evaluator == null || actionBar == null || signalText == null)
        {
            Debug.LogError("One or more required components are missing", this);
            enabled = false;
            return;
        }
        if (leadTime <= 0)
        {
            Debug.LogError("leadTime must be strictly positive", this);
            enabled = false;
            return;
        }

        actionBar.minValue = 0;
        actionBar.maxValue = 1;
        actionBar.wholeNumbers = false;
        actionBar.interactable = false;

        actionBar.value = 0;
        signalText.text = "Waiting...";
    }

    // Update is called once per frame
    void Update()
    {
        if (!evaluator.ActionPending)
        {
            actionBar.value = 0;
            signalText.text = "No pending action";
            return;
        }

        double remainingTime = evaluator.CurrentGoalTime - evaluator.ElapsedTime;
        if (remainingTime > leadTime)
        {
            actionBar.value = 0;
            signalText.text = "Waiting next signal...";
            return;
        }
        double progress = 1.0 - remainingTime / leadTime;

        progress = System.Math.Max(0.0, System.Math.Min(1.0, progress));

        actionBar.value = (float)progress;

        if (System.Math.Abs(remainingTime) <= evaluator.Tolerance)
        {
            signalText.text = "AHORA!";
        }
        else if (remainingTime > 0)
        {
            signalText.text = "Get ready...";
        }
        else
        {
            signalText.text = "Finished";
        }

    }
}
