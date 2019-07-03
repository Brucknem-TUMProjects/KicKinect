using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallReset_Keyboard : MonoBehaviour
{
    public KeyCode key = KeyCode.R;
    public Vector3 resetPosition;
    public Rigidbody physicsToReset;

    // Start is called before the first frame update
    void Start()
    {
        resetPosition = transform.position;
        physicsToReset = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(key)){
            transform.position = resetPosition;
            physicsToReset.velocity = Vector3.zero;
            physicsToReset.angularVelocity = Vector3.zero;
        }
    }
}
