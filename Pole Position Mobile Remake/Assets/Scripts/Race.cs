using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Race : MonoBehaviour
{
    public static GameObject[] points;
    public static int laps = 0;

    public static bool started = false;
    public static float startTimer = 0.0f;

    public static GameObject winner;

    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;

        points = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            points[i] = transform.GetChild(i).gameObject;
        }
    }

    void Update()
    {
        if (started)
        {
            if (winner)
            {
                UI.EndRace();
                print(winner.name + " has won the race!");
            }
        }
        else
        {
            startTimer += Time.deltaTime;
            if (startTimer >= 3)
            {
                StartRace();
            }
        }
    }

    void StartRace()
    {
        started = true;
    }

    void EndRace()
    {

    }
}
