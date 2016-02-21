using UnityEngine;
using System.Collections;

public class plateformMoving : MonoBehaviour {

    public Vector2[] localMapPoints;
    public bool movementCycle = false;
    public float speed = 5;
    public float waitTime = 0;

    [Range(0, 2)]
    public float easeAmount = 1;

    private Vector2[] globalMapPoints;
    private int fromWaypointIndex;
    private float percentBetweenMapPoints;
    private float nextMoveTime;
    private Vector3 frameMovement;

    void Start () {
        globalMapPoints = new Vector2[localMapPoints.Length];
        for (int i = 0; i < localMapPoints.Length; i++)
        {
            globalMapPoints[i] = localMapPoints[i] + new Vector2(transform.position.x, transform.position.y);
        }
    }
	
	void FixedUpdate () {
        frameMovement = CalculatePlateformMovement();
        transform.Translate(frameMovement);
    }

    void OnCollisionStay2D(Collision2D coll)
    {
        Rigidbody2D collRB2D = coll.gameObject.GetComponent<Rigidbody2D>();
        if (collRB2D != null)
            collRB2D.transform.Translate(frameMovement);
    }

    float Ease(float x)
    {
        float a = easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

    Vector3 CalculatePlateformMovement()
    {

        if (Time.time < nextMoveTime)
        {
            return Vector3.zero;
        }

        fromWaypointIndex %= globalMapPoints.Length;

        int toWaypointIndex = (fromWaypointIndex + 1) % globalMapPoints.Length;

        float distanceBetweenWayPoints = Vector3.Distance(globalMapPoints[fromWaypointIndex], globalMapPoints[toWaypointIndex]);
        percentBetweenMapPoints += Time.deltaTime * speed / distanceBetweenWayPoints;
        percentBetweenMapPoints = Mathf.Clamp01(percentBetweenMapPoints);
        float easedPercentBetweenWaypoints = Ease(percentBetweenMapPoints);

        Vector3 newPos = Vector3.Lerp(globalMapPoints[fromWaypointIndex], globalMapPoints[toWaypointIndex], easedPercentBetweenWaypoints);

        if (percentBetweenMapPoints >= 1)
        {
            percentBetweenMapPoints = 0;
            fromWaypointIndex++;

            if (!movementCycle)
            {
                if (fromWaypointIndex >= globalMapPoints.Length - 1)
                {
                    fromWaypointIndex = 0;
                    System.Array.Reverse(globalMapPoints);
                }
            }

            nextMoveTime = Time.time + waitTime;
        }

        return newPos - transform.position;
    }

    void OnDrawGizmos()
    {
        if (localMapPoints != null)
        {
            Gizmos.color = Color.green;
            float size = 0.3f;

            for (int i = 0; i < localMapPoints.Length; i++)
            {
                Vector2 globalMapPointPos = (Application.isPlaying) ? globalMapPoints[i] : localMapPoints[i] + new Vector2(transform.position.x, transform.position.y);
                Gizmos.DrawLine(globalMapPointPos - Vector2.up * size, globalMapPointPos + Vector2.up * size);
                Gizmos.DrawLine(globalMapPointPos - Vector2.left * size, globalMapPointPos + Vector2.left * size);
            }
        }
    }
}
