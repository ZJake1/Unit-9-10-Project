using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pathfinding : MonoBehaviour
{
    NavMeshAgent nav;

    int lapsDone = 0;
    int nextPoint = 0;

    bool finishedRace = false;

    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    // FixedUpdate is called 50 times per second
    void FixedUpdate()
    {
        if (Race.started && !finishedRace && !nav.pathPending && nav.remainingDistance < 5.0f)
            NextPoint();
    }

    void NextPoint()
    {
        if (nextPoint != 0 && Race.points[nextPoint - 1].tag == "Finish")
        {
            lapsDone++;
            nextPoint = 0;
            if (lapsDone >= Race.laps && Race.laps != 0)
            {
                if (Race.winner == null)
                {
                    Race.winner = gameObject;
                }

                finishedRace = true;
                nav.destination = transform.position;
                return;
            }
        }
        else
        {
            nav.destination = Race.points[nextPoint].transform.position;
            nextPoint++;
        }
    }
}
