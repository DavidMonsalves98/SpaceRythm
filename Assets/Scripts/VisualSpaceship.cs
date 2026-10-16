using UnityEngine;

public class VisualSpaceship : MonoBehaviour
{

    [SerializeField] private VidaNave spaceship;
    [SerializeField] private SpriteRenderer visual_ss;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (spaceship == null || visual_ss == null)
        {
            Debug.LogError("One or more components are missing", this);
            enabled = false;
            return;
        }

        spaceship.OnLifeChanged += UpdateVisual;
    }

    private void OnDestroy()
    {
        if (spaceship != null)
        {
            spaceship.OnLifeChanged -= UpdateVisual;
        }
    }

    private void UpdateVisual()
    {
        visual_ss.enabled = !spaceship.IsDestroyed;
    }
}
