using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kick_Keyboard : MonoBehaviour
{
    public KeyCode weak = KeyCode.Q;
    public KeyCode medium = KeyCode.W;
    public KeyCode strong = KeyCode.E;
    public Animator ac;
    // Start is called before the first frame update
    void Start()
    {
        if(ac == null)
            ac = GameObject.FindWithTag("Player").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(weak))
            ac.SetInteger("kickstrength",1);
        if(Input.GetKeyDown(medium))
            ac.SetInteger("kickstrength",2);
        if(Input.GetKeyDown(strong))
            ac.SetInteger("kickstrength",3);
    }
}
