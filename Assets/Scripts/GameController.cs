using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    [SerializeField] private PruebaSecuenciaRitmica evaluator;
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject results;
    [SerializeField] private TMP_Text textResults;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (evaluator == null || menu == null || hud == null || results == null || textResults == null)
        {
            Debug.LogError("One or more required components are missing", this);
            enabled = false;
            return;
        }
        evaluator.GameFinished += ShowResults;
        menu.SetActive(true);
        hud.SetActive(false);
        results.SetActive(false);
    }

    public void Play()
    {
        Deselect();

        hud.SetActive(true);
        menu.SetActive(false);
        results.SetActive(false);
        evaluator.StartTest();
    }

    public void Restart()
    {
        Play();
    }

    void Deselect()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void ReturnMenu()
    {
        Deselect();
        hud.SetActive(false);
        menu.SetActive(true);
        results.SetActive(false);
    }

    void ShowResults()
    {
        string title;
        if (evaluator.IsDestroyed)
        {
            title = "Destroyed";
        }
        else
        {
            title = "Completed";
        }

        textResults.text =
            $"{title}\n\n" +
            $"Hits: {evaluator.Hits}/" +
            $"{evaluator.TotalActions}\n" +
            $"Life remaining: {evaluator.LifeRemaining}\n" +
            $"Duration: {evaluator.Duration:F2} s";

        Deselect();

        hud.SetActive(false);
        menu.SetActive(false);
        results.SetActive(true);

    }

    private void OnDestroy()
    {
        if (evaluator != null)
        {
            evaluator.GameFinished -= ShowResults;
        }
    }

}
