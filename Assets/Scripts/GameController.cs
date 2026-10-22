using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private PruebaSecuenciaRitmica evaluator;
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject results;
    [SerializeField] private TMP_Text textResults;


    [SerializeField] private Button playButton;
    [SerializeField] private Button restartButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (evaluator == null || menu == null || hud == null || results == null || textResults == null || playButton == null || restartButton == null)
        {
            Debug.LogError("One or more required components are missing", this);
            enabled = false;
            return;
        }
        evaluator.GameFinished += ShowResults;
        ShowMenu();
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

    private void ShowMenu()
    {
        Deselect();

        menu.SetActive(true);
        hud.SetActive(false);
        results.SetActive(false);

        SelectButton(playButton);
    }

    public void ReturnMenu()
    {
        ShowMenu();
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

        SelectButton(restartButton);

    }

    private void OnDestroy()
    {
        if (evaluator != null)
        {
            evaluator.GameFinished -= ShowResults;
        }
    }

    private void SelectButton(Button button)
    {
        if (button == null || EventSystem.current == null || !button.gameObject.activeInHierarchy || !button.interactable )
        {
            return;
        }
        EventSystem.current.SetSelectedGameObject(button.gameObject);
    }

}
