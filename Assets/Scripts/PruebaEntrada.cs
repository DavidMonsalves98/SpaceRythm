using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PruebaEntrada : MonoBehaviour
{
    [SerializeField] private TMP_Text ActionText;

    private int ActionCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (ActionText == null)
        {
            Debug.LogError("ActionText is not assigned in the inspector.", this);
            enabled = false;
            return;
        }
        ActionText.text = "Press Space for Action";
    }

    // Update is called once per frame
    void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            Debug.LogError("No keyboard detected. Please ensure a keyboard is connected.", this);
            return;
        }
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            ExecuteAction();
        }
    }

    private void ExecuteAction()
    {
        ActionCount++;
        ActionText.text = $"Action executed {ActionCount} times.";

        Debug.Log($"Action detected. Total: {ActionCount}");
    }
}
