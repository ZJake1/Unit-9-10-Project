using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpMovement : MonoBehaviour
{
    public GameObject movedObject;
    public Vector3 lerpOffset = new Vector3(0.0f, 0.0f, 0.0f);
    public float lerpTime = 1.0f;

    Vector3 startPosition;

    IEnumerator LerpCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        LerpCoroutine = LerpPosition();

        startPosition = movedObject.transform.position;

        StartCoroutine(LerpCoroutine);
    }

    public void ResetCoroutine()
    {
        StopCoroutine(LerpCoroutine);

        startPosition = movedObject.transform.position;

        StartCoroutine(LerpCoroutine);
    }

    IEnumerator LerpPosition()
    {
        float lerpPercentage = 0.0f;
        bool returning = false;

        while (true)
        {
            lerpPercentage = Mathf.Clamp(lerpPercentage + Time.deltaTime / lerpTime, 0.0f, 1.0f);

            if (returning)
                movedObject.transform.position = Vector3.Lerp(startPosition + lerpOffset, startPosition, lerpPercentage);
            else
                movedObject.transform.position = Vector3.Lerp(startPosition, startPosition + lerpOffset, lerpPercentage);

            if (lerpPercentage == 1)
            {
                returning = !returning;
                lerpPercentage = 0.0f;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
