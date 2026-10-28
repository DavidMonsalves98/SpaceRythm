using UnityEngine;

public class SFXClock : MonoBehaviour
{
    public static SFXClock Instance { get; private set; }
    [SerializeField] private AudioSource sfx;
    [SerializeField] private AudioClip alien1;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public bool IsConfigured
    {
        get { return sfx != null && sfx.clip != null; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartPlaySFX()
    {
        if (!IsConfigured)
        {
            Debug.LogError(
                "One or more required components are missing.",
                this
                );
            return;
        }
        sfx.PlayOneShot(alien1);
    }
}
