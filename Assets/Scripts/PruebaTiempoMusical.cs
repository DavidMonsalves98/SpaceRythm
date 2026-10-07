using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PruebaTiempoMusical : MonoBehaviour
{
    [SerializeField] private AudioSource music;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private double bpm = 74;
    [SerializeField] private double firstBeatOffset = 0.0;


    private double musicStartTime;
    private double bps;
    private bool startSolicited = false;
    private int lastBeatCount = -1;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (music == null || timeText == null)
        {
            Debug.LogError("Music or TimeText is not assigned in the inspector.", this);
            enabled = false;
            return;
        }
        if (music.clip == null)
        {
            Debug.LogError("Music AudioSource does not have an assigned AudioClip.", this);
            enabled = false;
            return;
        }

        if (bpm <= 0 || firstBeatOffset < 0)
        {
            Debug.LogError("BPM must be greater than 0 and "
                + "firstBeatOffset must be non-negative.", this);
            enabled = false;
            return;
        }

        bps = 60.0 / bpm;

        timeText.text = "Press Space to Start Music";
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
            StartMusic();
        }
        else if (keyboard.spaceKey.wasPressedThisFrame && !startSolicited)
        {
            StartMusic();
        }

        if (!startSolicited)
        {
            return;
        }

        double elapsedTime = AudioSettings.dspTime - musicStartTime;

        if (elapsedTime < 0)
        {
            timeText.text = "Preparing to start music...";
            return;
        }

        if (elapsedTime >= music.clip.length)
        {
            timeText.text = $"Music finished: {music.clip.length:F2} s\n" +
                "R: Restart";
            return;
        }
        if (elapsedTime < firstBeatOffset)
        {
            timeText.text = $"Music time: {elapsedTime:F2} s\n" +
                "Waiting for the first beat...";
            return;
        }

        double beatPosition = (elapsedTime - firstBeatOffset) / bps;

        int currentBeatCount = (int)System.Math.Floor(beatPosition);

        if (currentBeatCount != lastBeatCount)
        {
            lastBeatCount = currentBeatCount;

            BeatRegister(currentBeatCount);
        }

        timeText.text = $"Music time: {elapsedTime:F2} s\n" +
            $"Beat position: {beatPosition:F2}\n" +
            $"BPM: {bpm:F2} | R: Restart";

    } 

    private void StartMusic()
    {
        music.Stop();
        musicStartTime = AudioSettings.dspTime + 1.0;
        music.PlayScheduled(musicStartTime);
        startSolicited = true;
    }

    private void BeatRegister(int beatCount)
    {
        Debug.Log($"New beat: {beatCount}");
    }
}
