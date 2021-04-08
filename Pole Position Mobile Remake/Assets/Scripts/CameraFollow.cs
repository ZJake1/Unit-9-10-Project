using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Required")]
    public Transform target;

    [Header("Settings")]
    public int cameraType = 0;
    public float cameraSmoothing = 1.0f;

    [Header("Third Person")]
    public float thirdPersonHeight = 3.0f;
    public float thirdPersonForward = -7.0f;

    [Header("Top Down")]
    public float topDownHeight = 10.0f;

    string[] cameraTypes = { "Follow Front Facing", "Follow Back Facing", "Top Down" };

    // Update is called once per frame
    void Update()
    {
        UpdateCameraType();
    }

    // FixedUpdate is called 50 times per second
    void FixedUpdate()
    {
        UpdateCamera();
    }

    void UpdateCameraType()
    {
        if (Input.GetKeyDown("v"))
        {
            if (cameraType + 1 < cameraTypes.Length)
                cameraType++;
            else
                cameraType = 0;
        }
    }

    void UpdateCamera()
    {
        if (cameraTypes[cameraType] == "Follow Front Facing")
        {
            // Position
            transform.position = Vector3.Lerp(transform.position, target.position + (target.forward * thirdPersonForward + new Vector3(0.0f, thirdPersonHeight, 0.0f)), cameraSmoothing);

            // Rotation
            Vector3 targetDir = target.position - transform.position;
            Quaternion targetRot = Quaternion.LookRotation(targetDir);
            transform.rotation = Quaternion.Lerp(transform.localRotation, targetRot, cameraSmoothing);
        }
        else if (cameraTypes[cameraType] == "Follow Back Facing")
        {
            // Position
            transform.position = Vector3.Lerp(transform.position, target.position + (-target.forward * thirdPersonForward + new Vector3(0.0f, thirdPersonHeight, 0.0f)), cameraSmoothing);

            // Rotation
            Vector3 targetDir = target.position - transform.position;
            Quaternion targetRot = Quaternion.LookRotation(targetDir);
            transform.rotation = Quaternion.Lerp(transform.localRotation, targetRot, cameraSmoothing);
        }
        else if (cameraTypes[cameraType] == "Top Down")
        {
            // Position
            transform.position = Vector3.Lerp(transform.position, target.position + new Vector3(0.0f, topDownHeight, 0.0f), cameraSmoothing);

            // Rotation
            Quaternion targetRot = Quaternion.Euler(90.0f, target.localEulerAngles.y, 0.0f);
            transform.rotation = Quaternion.Lerp(transform.localRotation, targetRot, cameraSmoothing);
        }
    }
}
