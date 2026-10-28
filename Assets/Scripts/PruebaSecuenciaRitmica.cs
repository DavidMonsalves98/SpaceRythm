using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PruebaSecuenciaRitmica : MonoBehaviour
{

    [SerializeField] private MusicClock musicClock;
    [SerializeField] private TMP_Text textState;
    [SerializeField] private TMP_Text textBeatPosition;
    [SerializeField] private VidaNave spaceship;
    [SerializeField] private PlayerInputReader inputReader;
     
    [SerializeField] private double[] goalsTime = { 3.0, 7.0, 12.0, 16.0};

    [SerializeField] private double tolerance = 0.15;
    [SerializeField] private int damage = 35;

    private bool startSolicited = false;

    private int currentGoalIndex = 0;
    private int successfulHits = 0;

    private string lastResult = "Pending";

    private bool IsTestCompleted => currentGoalIndex >= goalsTime.Length || spaceship.IsDestroyed;
    private double testDuration = 0;
    private bool results = false;

    public double ElapsedTime
    {
        get { return musicClock.ElapsedTime; }
    }

    public double Tolerance
    {
        get { return tolerance; }
    }

    public bool ActionPending
    {
        get 
        {
            return startSolicited 
                && spaceship != null 
                && !spaceship.IsDestroyed 
                && currentGoalIndex < goalsTime.Length 
                && ElapsedTime >= 0;
        }
    }

    public double CurrentGoalTime
    {
        get 
        { 
            if (!ActionPending)
            {
                return 0;
            }
            return goalsTime[currentGoalIndex];
        }
    }

    public event System.Action<bool> ActionSolved;
    public event System.Action GameFinished;

    public int Hits => successfulHits;
    public int TotalActions => goalsTime.Length;
    public int LifeRemaining => spaceship.CurrentLife;
    public bool IsDestroyed => spaceship.IsDestroyed;
    public double Duration => testDuration;

    private void OnEnable()
    {
        if(inputReader != null)
        {
            inputReader.ActionRequested += OnActionRequested;
        }
    }

    private void OnDisable()
    {
        if (inputReader != null)
        {
            inputReader.ActionRequested -= OnActionRequested;
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (musicClock == null || textState == null || spaceship == null || textBeatPosition == null)
        {
            Debug.LogError("One or more required components are missing.", this);
            enabled = false;
            return;
        }
        if (!musicClock.IsConfigured)
        {
            Debug.LogError("MusicClock requires an AudioSource with an AudioClip.", this);
            enabled = false;
            return;
        }
        if (goalsTime == null || 
            goalsTime.Length == 0 || 
            tolerance <= 0 || 
            damage <= 0)
        {
            Debug.LogError("GoalsTime array must not be null or empty, tolerance and damage must be greater than 0.", this);
            enabled = false;
            return;
        }

        for (int i = 0; i < goalsTime.Length; i++)
        {
            if (goalsTime[i] < 0 || goalsTime[i] + tolerance >= musicClock.ClipDuration)
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

        textState.text = "Press Play to Start";

    }

    // Update is called once per frame
    void Update()
    {
        if (!startSolicited)
        {
            return;
        }

        if (results)
        {
            ShowState(testDuration);
            return;
        }

        double elapsedTime = musicClock.ElapsedTime;

        textBeatPosition.text = $"BP: {musicClock.BeatPosition:F2}";

        if (elapsedTime < 0)
        {
            textState.text = "Preparing Music";
            return;
        }

        while (!spaceship.IsDestroyed && currentGoalIndex < goalsTime.Length
                && elapsedTime > goalsTime[currentGoalIndex] + tolerance + 1.0
        )
        {
            SolveTest(false, "Missed");
        }


        FinishTestIfNeeded();

        if (results)
        {
            ShowState(testDuration);
        }
        else
        {
            ShowState(elapsedTime);
        }

    }

    private void OnActionRequested()
    {
        if (!ActionPending || results)
        {
            return;
        }

        EvaluatePulse(musicClock.ElapsedTime);
        FinishTestIfNeeded();

    }

    public void StartTest()
    {
        if (!musicClock.StartPlayback())
        {
            return;
        }

        currentGoalIndex = 0;
        successfulHits = 0;
        lastResult = "Pending";

        testDuration = 0;
        results = false;

        spaceship.RestoreLife();

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
            musicClock.StopPlayback();
        }

        ActionSolved?.Invoke(isHit);
    }

    private void FinishTestIfNeeded()
    {
        if (!IsTestCompleted || results)
        {
            return;
        }

        musicClock.StopPlayback();
        testDuration = musicClock.ElapsedTime;

        results = true;

        GameFinished?.Invoke();
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
        "F: action";
    }
}