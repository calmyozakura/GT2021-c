using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour
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
    }
}
