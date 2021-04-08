using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public CarMovement carVariables;

    static GameObject HUD;

    Text lapsText;
    Text speedText;
    Text lapTimeText;

    static GameObject endScreen;
    static Text endScreenText;

    static string lapTimeString;

    // Start is called before the first frame update
    void Start()
    {
        HUD = transform.Find("HUD").gameObject;
        lapsText = transform.Find("HUD/LapsText").GetComponent<Text>();
        speedText = transform.Find("HUD/SpeedText").GetComponent<Text>();
        lapTimeText = transform.Find("HUD/LapTimeText").GetComponent<Text>();

        endScreen = transform.Find("EndScreen").gameObject;
        endScreenText = transform.Find("EndScreen/RaceEndText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        string raceText;

        if (Race.laps <= 0)
            raceText = "∞";
        else
            raceText = Race.laps.ToString();

        lapsText.text = carVariables.lapsDone + "/" + raceText;
        speedText.text = (carVariables.rb.velocity.magnitude * 2.237f).ToString("F1") + " MPH";

        lapTimeString = carVariables.lapMinutes.ToString("00") + ":" + carVariables.lapSeconds.ToString("00") + ":" + Mathf.Floor(carVariables.lapDeltaTime * 100).ToString("00");
        lapTimeText.text = lapTimeString;
    }

    public static void EndRace()
    {
        endScreenText.text = "You have completed the track in " + lapTimeString + "! \n Click to play again";
        HUD.SetActive(false);
        endScreen.SetActive(true);
    }
}
