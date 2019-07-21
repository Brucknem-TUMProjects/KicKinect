using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [Range(-180, 180)]
    public int degreePerSecond = 30;

    public void Update()
    {
        gameObject.transform.Rotate(Vector3.up, degreePerSecond * Time.deltaTime);
    }

    public void SetSpeed(float value)
    {
        degreePerSecond = (int)(((value * 2) - 1) * 180 );
    }
}
