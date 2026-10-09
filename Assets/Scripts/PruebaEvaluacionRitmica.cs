using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.U2D.IK;

public class PruebaEvaluacionRitmica : MonoBehaviour
{
    [SerializeField] private AudioSource music;
    [SerializeField] private TMP_Text textState;
    [SerializeField] private VidaNave spaceship;
    [SerializeField] private double objectiveTime = 3.0;
    [SerializeField] private double tolerance = 0.15;
    [SerializeField] private int damage = 35;

    private double startTime;
    private bool startSolicited = false;
    private bool tryingResult = false;

    private string result = "Pending";


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (music == null || textState == null || spaceship == null)
        {
            Debug.LogError("One or more required components are missing.", this);
            enabled = false;
            return;
        }

        if(music.clip == null)
        {
            Debug.LogError("Music AudioSource does not have an assigned AudioClip.", this);
            enabled = false;
            return;
        }

        if ( tolerance <= 0 || objectiveTime < tolerance || objectiveTime + tolerance >= music.clip.length)
        {
            Debug.LogError("Tolerance must be greater than 0, objectiveTime must be greater than tolerance, and objectiveTime + tolerance must be less than the length of the music clip.", this);
            enabled = false;
            return;
        }
        textState.text = "Press Space to Start Music";

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

        if (keyboard != null)
        {
            if (keyboard.rKey.wasPressedThisFrame)
            {
                StartTest();
            }
            else if (keyboard.spaceKey.wasPressedThisFrame && !startSolicited)
            {
                StartTest();
            }
        }

        if (!startSolicited)
        {
            return;
        }

        double elapsedTime = AudioSettings.dspTime - startTime;

        if (elapsedTime < 0)
        {
            textState.text = "Preparing Music";
            return;
        }

        if(!tryingResult)
        {
            if (keyboard != null && keyboard.fKey.wasPressedThisFrame)
            {
                EvaluatePulse(elapsedTime);
            }
            else if (elapsedTime > objectiveTime + tolerance + 2.0)
            {
                SolveTest(false, "Missed");
            }
        }

        textState.text = 
            $"Time: {elapsedTime:F2} s\n" +
            $"Objective: {objectiveTime:F2} s\n" +
            $"Result: {result}\n" +
            "F: action | R: Restart";

    }

    private void StartTest()
    {
        music.Stop();

        tryingResult = false;
        result = "Pending";

        spaceship.RestoreLife();

        startTime = AudioSettings.dspTime + 1.0;
        music.PlayScheduled(startTime);

        startSolicited = true;
    }


    private void EvaluatePulse(double timePulse)
    {
        double diference = timePulse - objectiveTime;
        double error = System.Math.Abs(diference);

        tryingResult = true;

        if (error <= tolerance)
        {
            SolveTest(true, $"Hit! | Error: {error:F3} s");
        }

        else if (diference < 0)
        {
            SolveTest(false, $"Early! | Error: {error:F3} s");
        }
        else
        {
            SolveTest(false, $"Late! | Error: {error:F3} s");
        }

    }

    private void SolveTest(bool isHit, string message)
    {
        if (isHit)
        {
            return;
        }

        tryingResult = true;
        result = message;

        if (!isHit)
        {
            spaceship.TakeDamage(damage);
        }

    }
}
