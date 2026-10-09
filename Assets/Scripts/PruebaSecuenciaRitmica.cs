using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PruebaSecuenciaRitmica : MonoBehaviour
{

    [SerializeField] private AudioSource music;
    [SerializeField] private TMP_Text textState;
    [SerializeField] private VidaNave spaceship;

    [SerializeField] private double[] goalsTime = { 3.0, 7.0, 12.0, 16.0};

    [SerializeField] private double tolerance = 0.15;
    [SerializeField] private int damage = 35;

    private double startMusic;
    private bool startSolicited = false;

    private int currentGoalIndex = 0;
    private int successfulHits = 0;

    private string lastResult = "Pending";

    private bool IsTestCompleted => currentGoalIndex >= goalsTime.Length || spaceship.IsDestroyed;
    private double testDuration = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (music == null || textState == null || spaceship == null)
        {
            Debug.LogError("One or more required components are missing.", this);
            enabled = false;
            return;
        }
        if (music.clip == null)
        {
            Debug.LogError("Music AudioSource does not have an assigned AudioClip.", this);
            enabled = false;
            return;
        }
        if (goalsTime == null || goalsTime.Length == 0 || tolerance <= 0 || damage <= 0)
        {
            Debug.LogError("GoalsTime array must not be null or empty, tolerance and damage must be greater than 0.", this);
            enabled = false;
            return;
        }

        for (int i = 0; i < goalsTime.Length; i++)
        {
            if (goalsTime[i] < 0 || goalsTime[i] + tolerance >= music.clip.length)
            {
                Debug.LogError($"Goal time at index {i} is invalid. It must be non-negative and less than the length of the music clip minus tolerance.", this);
                enabled = false;
                return;
            }
            if (i > 0 && goalsTime[i] <= goalsTime[i - 1])
            {
                Debug.LogError($"Goal times must be in strictly increasing order. Goal time at index {i} is not greater than the previous goal time.", this);
                enabled = false;
                return;
            }
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
        if (keyboard.rKey.wasPressedThisFrame)
        {
            StartTest();

        }
        else if (keyboard.spaceKey.wasPressedThisFrame && !startSolicited)
        {
            StartTest();
        }

        if (!startSolicited)
        {
            return;
        }

        double elapsedTime = AudioSettings.dspTime - startMusic;

        if (elapsedTime < 0)
        {
            textState.text = "Preparing Music";
            return;
        }

        if (!spaceship.IsDestroyed && currentGoalIndex < goalsTime.Length)
        {
            if (keyboard != null && keyboard.fKey.wasPressedThisFrame)
            {
                EvaluatePulse(elapsedTime);
            }

            while (!spaceship.IsDestroyed && currentGoalIndex < goalsTime.Length
                && elapsedTime > goalsTime[currentGoalIndex] + tolerance + 1.0)
            {
                SolveTest(false, "Missed");
            }
        }
        if (IsTestCompleted)
        {
            if (testDuration == 0)
            {
                testDuration = elapsedTime;
            }
            music.Stop();
            ShowState(testDuration);
            return;
        }
        ShowState(elapsedTime);

    }

    private void StartTest()
    {
        music.Stop();

        currentGoalIndex = 0;
        successfulHits = 0;
        lastResult = "Pending";
        testDuration = 0;

        spaceship.RestoreLife();

        startMusic = AudioSettings.dspTime + 1.0;
        music.PlayScheduled(startMusic);

        startSolicited = true;

    }

    private void EvaluatePulse(double elapsedTime)
    {
        double goalTime = goalsTime[currentGoalIndex];
        double diference = elapsedTime - goalTime;
        double error = System.Math.Abs(diference);

        if (error <= tolerance)
        {
            SolveTest(true, $"Hit! | Error: {error:F2} s");
        }
        else if (diference < 0)
        {
            SolveTest(false, $"Early! | Error {error:F2}");
        }
        else
        {
            SolveTest(false, $"Late! | Error {error:F2}");
        }

    }

    private void SolveTest(bool isHit, string message)
    {
        lastResult = $"Action {currentGoalIndex + 1}: {message}";

        currentGoalIndex++;

        if (isHit)
        {
            successfulHits++;
        }
        else
        {
            spaceship.TakeDamage(damage);
        }

        if (spaceship.IsDestroyed)
        {
            music.Stop();
        }
    }

    private void ShowState(double elapsedTime)
    {
        string stateSpaceship;

        if (spaceship.IsDestroyed)
        {
            stateSpaceship = "You are destroyed!";
        }
        else if (currentGoalIndex >= goalsTime.Length)
        {
            stateSpaceship = "You have completed the sequence!";
        }
        else
        {
            stateSpaceship =
                $"Next action: {currentGoalIndex + 1}" +
                $"in {goalsTime[currentGoalIndex]:F2} s";
        }

        textState.text =
        $"{stateSpaceship}\n" +
        $"Time: {elapsedTime:F2} s\n" +
        $"Hits: {successfulHits}\n" +
        $"{lastResult}\n" +
        "F: action | R: Restart";
    }
}