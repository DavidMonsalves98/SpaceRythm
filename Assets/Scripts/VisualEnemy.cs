using UnityEngine;

public class VisualEnemy : MonoBehaviour
{
    [SerializeField] private PruebaSecuenciaRitmica evaluator;
    [SerializeField] private RhytmicEnemy prefabEnemy;
    [SerializeField] private Transform spawnEnemy;
    [SerializeField] private Transform finalPosEnemy;

    [SerializeField] private double leadTime;

    private RhytmicEnemy currentEnemy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (evaluator == null || prefabEnemy == null || spawnEnemy == null || finalPosEnemy == null)
        {
            Debug.LogError("One or more components are missing", this);
            enabled = false;
            return;
        }

        if (leadTime <= 0)
        {
            Debug.LogError("Lead time must be strictly positive.", this);
            enabled = false;
            return;
        }

        evaluator.ActionSolved += SolveActionEnemy;

    }

    // Update is called once per frame
    void Update()
    {
        if (!evaluator.ActionPending)
        {
            RetreatEnemy();
            return;
        }

        double remainingTime = evaluator.CurrentGoalTime - evaluator.ElapsedTime;

        if(currentEnemy == null && remainingTime <= leadTime)
        {
            currentEnemy = Instantiate(prefabEnemy, spawnEnemy.position, spawnEnemy.rotation);
            currentEnemy.Configure(evaluator, evaluator.CurrentGoalTime, leadTime, finalPosEnemy.position);
        }

    }
    
    private void SolveActionEnemy(bool isHit)
    {
        if (isHit)
        {
            Debug.Log("is Hit!: Enemy destroyed.");
        }
        else
        {
            Debug.Log("Missed: Enemy in retreat");
        }
        RetreatEnemy();

    }

    private void RetreatEnemy()
    {
        if (currentEnemy == null)
        {
            return;
        }
        Destroy(currentEnemy.gameObject);
        currentEnemy = null;
    }

    private void OnDestroy()
    {
        if (evaluator != null)
        {
            evaluator.ActionSolved -= SolveActionEnemy;
        }
        RetreatEnemy();
    }

}
