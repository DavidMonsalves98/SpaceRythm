using System;
using UnityEngine;

public class VidaNave : MonoBehaviour
{

    [SerializeField] private int life = 100;

    private int currentLife;

    public event Action OnLifeChanged;

    public int CurrentLife
    {
        get { return currentLife; }
    }

    public int Life
    {
        get { return life; }
    }

    public bool IsDestroyed
    {
        get { return currentLife <= 0; }
    }

    private void Awake()
    {
        life = Mathf.Max(1, life);
        RestoreLife();
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || IsDestroyed)
        {
            return;
        }
        currentLife = Mathf.Max(0, currentLife - damage);

        OnLifeChanged?.Invoke();
    }

    public void RestoreLife()
    {
        currentLife = life;
        OnLifeChanged?.Invoke();
    }

}
