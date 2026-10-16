using UnityEngine;

public class RhytmicEnemy : MonoBehaviour
{
    private PruebaSecuenciaRitmica evaluator;

    private Vector3 initialPos;
    private Vector3 finalPos;

    private double goalTime;
    private double movDuration;

    private bool configured = false;

    public void Configure(PruebaSecuenciaRitmica evaluator, double goalTime,double leadTime,
            Vector3 finalPos)
    {
        this.evaluator = evaluator;
        this.initialPos = transform.position;
        this.finalPos = finalPos;
        
        this.goalTime = goalTime;
        movDuration = leadTime;
        configured = true;
        Debug.Log($"initial pos: {this.initialPos}, final pos:{this.finalPos}");
        UpdatePosition();

    }

    // Update is called once per frame
    void Update()
    {
        if (!configured || evaluator == null)
        {
            return;
        }
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        double startRoute = goalTime - movDuration;
        double progress = (evaluator.ElapsedTime - startRoute) / movDuration;

        transform.position = Vector3.Lerp(initialPos, finalPos, (float)progress);

    }
}
