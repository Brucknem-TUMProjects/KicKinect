using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Ceres : MonoBehaviour
{
    [DllImport("CeresWrapper.dll")]
    public static extern float getTrace();

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("The trace value is: " + getTrace());
    }
}
