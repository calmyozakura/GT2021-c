using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("RB")) Debug.Log("RB");
        if (Input.GetButtonDown("LB")) Debug.Log("LB");
        Debug.Log(Input.GetAxis("ClossVertical"));
        Debug.Log(Input.GetAxis("ClossHorizontal"));
        if (Input.GetButtonDown("Y")) Debug.Log("Y");
        if (Input.GetButtonDown("X")) Debug.Log("X");
        if (Input.GetButtonDown("A")) Debug.Log("A");
        if (Input.GetButtonDown("B")) Debug.Log("B");
        if (Input.GetButtonDown("Start")) Debug.Log("Start");
        if (Input.GetButtonDown("Back")) Debug.Log("Back");
    }
}
