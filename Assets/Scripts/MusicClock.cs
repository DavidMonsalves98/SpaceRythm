using UnityEngine;

public class MusicClock : MonoBehaviour
{
    [SerializeField] private AudioSource music;

    private double scheduledStart;
    private double stoppedTime = 0;

    private bool isRunning = false;

    public bool IsConfigured
    {
        get { return music != null && music.clip != null; }
    }

    public double ClipDuration
    {
        get
        {
            if (!IsConfigured)
            {
                return 0;
            }
            return music.clip.length;
        }
    }

    public double ElapsedTime
    {
        get
        {
            if (!isRunning)
            {
                return stoppedTime;
            }

            return AudioSettings.dspTime - scheduledStart;
        }
    }

    public bool StartPlayback()
    {
        if (!IsConfigured)
        {
            Debug.LogError(
                "One or more required components are missing.",
                this
                );
            return false;
        }

        music.Stop();

        stoppedTime = 0;
        scheduledStart = AudioSettings.dspTime + 1.0;
        isRunning = true;

        music.PlayScheduled(scheduledStart);

        return true;

    }

    public void StopPlayback()
    {
        stoppedTime = ElapsedTime;
        isRunning = false;

        if (music != null)
        {
            music.Stop();
        }
    }

}
