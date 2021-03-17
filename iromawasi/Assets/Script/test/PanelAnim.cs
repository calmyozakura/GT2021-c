using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelAnim : MonoBehaviour
{
    Animator anim;
    public bool[] animFlg = new bool[8];

    // Start is called before the first frame update
    void Start()
    {
        // アニメーター（アニメーション制御のやつ）を受け取る
        anim = GetComponent("Animator") as Animator;
    }

    // Update is called once per frame
    void Update()
    {
        if (animFlg[0])
        {
            GetComponent<Animator>().SetTrigger("right");
            animFlg[0] = false;
        }
        else if (animFlg[1])
        {
            GetComponent<Animator>().SetTrigger("down");
            animFlg[1] = false;
        }
        else if (animFlg[2])
        {
            GetComponent<Animator>().SetTrigger("left");
            animFlg[2] = false;
        }
        else if (animFlg[3])
        {
            GetComponent<Animator>().SetTrigger("up");
            animFlg[3] = false;
        }
        else if (animFlg[4])
        {
            GetComponent<Animator>().SetTrigger("right2");
            animFlg[4] = false;
        }
        else if (animFlg[5])
        {
            GetComponent<Animator>().SetTrigger("down2");
            animFlg[5] = false;
        }
        else if (animFlg[6])
        {
            GetComponent<Animator>().SetTrigger("left2");
            animFlg[6] = false;
        }
        else if (animFlg[7])
        {
            GetComponent<Animator>().SetTrigger("up2");
            animFlg[7] = false;
        }
    }
}
