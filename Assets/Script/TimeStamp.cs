using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeStamp : MonoBehaviour
{
    
    [SerializeField] Text timeStamp;
    // Start is called before the first frame update
    void Start()
    {
        string filepath1 = "Build";
        System.DateTime dt = System.IO.File.GetLastWriteTime(filepath1);
        timeStamp.text = "" + dt.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
