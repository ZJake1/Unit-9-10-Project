using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [Header("GameObjects")]
    public GameObject checkpoint;

    [Header("Components")]
    public Rigidbody rb;

    Light rightLight;
    Light leftLight;

    [Header("Speed")]
    public float moveSpeed = 15.0f;
    public float reverseSpeed = -7.5f;
    public float acceleration = 7.5f;

    [Header("Handling")]
    public float turnSpeed = 90.0f;
    public float reverseTurnSpeed = 45.0f;
    public float turnAcceleration = 180.0f;

    [Header("Race")]
    public int lapsDone = 0;
    public int nextPoint = 0;

    public int lapMinutes = 0;
    public int lapSeconds = 0;
    public float lapDeltaTime = 0.0f;

    [Header("Particles")]
    public ParticleSystem rightWheelSmoke;
    public ParticleSystem leftWheelSmoke;

    [Header("Visuals")]
    public float maxWheelTurn = 30.0f;

    public float curSpeed = 0.0f;
    public float curTurnSpeed = 0.0f;

    [Header("Audio")]
    AudioSource mainAudio;

    public AudioClip checkpointAudio;

    bool finishedRace = false;

    float h = 0.0f;
    float latestTurnDirection = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainAudio = Camera.main.GetComponent<AudioSource>();

        rightLight = transform.Find("CarBody/Light.R").GetComponent<Light>();
        leftLight = transform.Find("CarBody/Light.L").GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        // Gets the player's horizontal input
        h = Input.GetAxisRaw("Horizontal");

        // Gets the player's mobile tilt input if there was no keyboard input
        if (h == 0.0f)
                h = Mathf.Sign(Input.acceleration.x);

        // Checks if the race has started and the player hasn't finished the race
        if (Race.started && !finishedRace)
        {
            // Checks if the player is in range of a checkpoint
            CheckCheckpoint();

            // Updates the speed and turning speed variables
            UpdateSpeed();
            UpdateTurnSpeed();
        }
        else if (finishedRace)
        {
            if (Mathf.Sign(curSpeed) == 1)
                curSpeed = Mathf.Clamp(curSpeed - acceleration * Time.deltaTime, 0.0f, moveSpeed);
            else if (curSpeed < 0.0f)
                curSpeed = Mathf.Clamp(curSpeed + acceleration * Time.deltaTime, reverseSpeed, 0.0f);

            curTurnSpeed = Mathf.Clamp(curTurnSpeed - turnAcceleration * Time.deltaTime, 0.0f, turnSpeed);
        }
        
        // Updates the visual checkpoint position
        UpdateCheckpoint();

        // Updates the smoke on the back wheels of the car
        UpdateSmoke();

        // Updates the lights on the front of the car
        UpdateLights();
    }

    // FixedUpdate is called 50 times per second
    void FixedUpdate()
    {
        if (Race.started && !finishedRace)
        {
            // Visuals
            UpdateWheels();
        }

        // Movement
        UpdateVelocity();
        TurnCar();
    }

    void CheckCheckpoint()
    {
        lapDeltaTime += Time.deltaTime;
        if (lapDeltaTime >= 1.0f)
        {
            lapDeltaTime = 0.0f;
            lapSeconds++;

            if (lapSeconds > 60)
            {
                lapSeconds = 0;
                lapMinutes++;
            }
        }

        if ((Race.points[nextPoint].transform.position - transform.position).magnitude < 6.0f)
        {
            if (nextPoint != 0 && Race.points[nextPoint].tag == "Finish")
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
                }
                else
                {
                    ResetLapTimer();
                }
            }
            else
            {
                nextPoint++;
            }

            mainAudio.PlayOneShot(checkpointAudio, 1.0f);
        }
    }

    void ResetLapTimer()
    {
        lapMinutes = 0;
        lapSeconds = 0;
        lapDeltaTime = 0.0f;
    }

    void UpdateSpeed()
    {
        if (Mathf.Abs(rb.velocity.magnitude) + 0.2f < Mathf.Abs(curSpeed))
        {
            curSpeed = rb.velocity.magnitude;
        }

        if (Input.GetKey("s") || Input.acceleration.y < -0.8f)
            curSpeed = Mathf.Clamp(curSpeed - acceleration * Time.deltaTime, reverseSpeed, moveSpeed);
        else
            curSpeed = Mathf.Clamp(curSpeed + acceleration * Time.deltaTime, reverseSpeed, moveSpeed);
    }

    void UpdateTurnSpeed()
    {
        if (Mathf.Sign(curSpeed) == 1 && Mathf.Abs(curTurnSpeed / turnSpeed) > Mathf.Abs(curSpeed / moveSpeed))
            curTurnSpeed = turnSpeed * curSpeed / moveSpeed;
        else if (Mathf.Sign(curSpeed) == -1 && Mathf.Abs(curTurnSpeed / reverseTurnSpeed) > Mathf.Abs(curSpeed / reverseSpeed))
            curTurnSpeed = turnSpeed * curSpeed / reverseSpeed;

        if (Mathf.Sign(curSpeed) == -1 && Mathf.Abs(curTurnSpeed) > reverseTurnSpeed)
            curTurnSpeed = 45.0f;

        if (h != 0.0f && (Mathf.Sign(h) == Mathf.Sign(latestTurnDirection) || curTurnSpeed == 0.0f))
        {
            if (Mathf.Sign(curSpeed) == 1)
                curTurnSpeed = Mathf.Clamp(curTurnSpeed + turnAcceleration * Time.deltaTime, 0.0f, turnSpeed);
            else
                curTurnSpeed = Mathf.Clamp(curTurnSpeed + turnAcceleration * Time.deltaTime, 0.0f, reverseTurnSpeed);

            latestTurnDirection = h;
        }
        else
            curTurnSpeed = Mathf.Clamp(curTurnSpeed - turnAcceleration * Time.deltaTime, 0.0f, turnSpeed);
    }

    void UpdateCheckpoint()
    {
        if (finishedRace)
        {
            checkpoint.SetActive(false);
        }
        else
        {
            checkpoint.transform.position = Race.points[nextPoint].transform.position + new Vector3(0.0f, 2.0f, 0.0f);
            checkpoint.GetComponent<LerpMovement>().ResetCoroutine();
        }
    }

    void UpdateSmoke()
    {
        if (curTurnSpeed > turnSpeed / 2.0f)
        {
            if (!rightWheelSmoke.isPlaying)
                rightWheelSmoke.Play();
            if (!leftWheelSmoke.isPlaying)
                leftWheelSmoke.Play();
        }
        else
        {
            if (rightWheelSmoke.isPlaying)
                rightWheelSmoke.Stop();
            if (leftWheelSmoke.isPlaying)
                leftWheelSmoke.Stop();
        }
    }

    void UpdateLights()
    {
        if (Input.GetKeyDown("f"))
        {
            rightLight.enabled = !rightLight.enabled;
            leftLight.enabled = !leftLight.enabled;
        }
    }

    void UpdateWheels()
    {
        float wheelYRotation;

        if (Mathf.Sign(curSpeed) == 1)
            wheelYRotation = maxWheelTurn * latestTurnDirection * curTurnSpeed / turnSpeed;
        else
            wheelYRotation = -maxWheelTurn * latestTurnDirection * curTurnSpeed / reverseTurnSpeed;
        
        transform.Find("FrontWheel.L").transform.localRotation = Quaternion.Euler(0.0f, wheelYRotation, 0.0f);
        transform.Find("FrontWheel.R").transform.localRotation = Quaternion.Euler(0.0f, wheelYRotation, 0.0f);
    }

    // Accelerate adds force towards transform.forward
    void UpdateVelocity()
    {
        rb.velocity = transform.forward * curSpeed;
    }

    // Turn adds torque in the direction the player is inputting
    void TurnCar()
    {
        transform.localEulerAngles += new Vector3(0.0f, latestTurnDirection * curTurnSpeed * Time.fixedDeltaTime, 0.0f);
    }
}
