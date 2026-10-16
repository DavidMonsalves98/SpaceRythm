using UnityEngine;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    [SerializeField] private PruebaSecuenciaRitmica evaluator;
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject hud;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (evaluator == null || menu == null || hud == null)
        {
            Debug.LogError("One or more required components are missing", this);
            enabled = false;
            return;
        }

        menu.SetActive(true);
        hud.SetActive(false);
    }

    void Play()
    {
        Deselect();

        hud.SetActive(true);
        menu.SetActive(false);
        evaluator.StartTest();
    }

    void Restart()
    {
        Deselect();

        evaluator.StartTest();
    }

    void Deselect()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
