using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [Range(0, 180)]
    public int degreePerSecond = 30;

    public void Update()
    {
        gameObject.transform.Rotate(Vector3.up, degreePerSecond * Time.deltaTime);
    }
}
