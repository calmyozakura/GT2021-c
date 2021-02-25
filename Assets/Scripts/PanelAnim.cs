using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelAnim : MonoBehaviour
{
    Animator anim;
    public bool[] animFlg = new bool[4];

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
            //anim.SetBool("right", true);
            //anim.SetBool("right", false);
            GetComponent<Animator>().SetTrigger("onceright");
           animFlg[0] = false;
        }
        else if (animFlg[1])
        {
            //anim.SetBool("down", true);
            //anim.SetBool("down", false);
            GetComponent<Animator>().SetTrigger("oncedown");
            animFlg[1] = false;
        }
        else if (animFlg[2])
        {
            //anim.SetBool("left", true);
            //anim.SetBool("left", false);
            GetComponent<Animator>().SetTrigger("onceleft");
            animFlg[2] = false;
        }
        else if (animFlg[3])
        {
            //anim.SetBool("up", true);
            GetComponent<Animator>().SetTrigger("onceup");
            //anim.SetBool("up", false);
            animFlg[3] = false;
        }
    }
}
