using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour
{
    public bool[] panelMove = new bool[2]; //右か左にパネル移動させるフラグ
    public bool ClossTilt;     //十字キーがニュートラルに戻ったか

    public void PanelOperation(int chooseMain)
    {
        //パネル反時計回り
        if (Input.GetButtonDown("LB"))
        {

            panelMove[0] = true;
        }
        //パネル時計回り
        if (Input.GetButtonDown("RB"))
        {
            panelMove[1] = true;

        }

        //十字キーのパネル選択
        if (0 > Input.GetAxis("ClossVertical") && !ClossTilt)    //↓入力時
        {
            if (chooseMain >= 6) chooseMain -= 6;
            else chooseMain += 3;
            ClossTilt = true;
        }
        else if (0 < Input.GetAxis("ClossVertical") && !ClossTilt)  //↑入力時
        {
            if (chooseMain <= 2) chooseMain += 6;
            else chooseMain -= 3;
            ClossTilt = true;
        }
        if (0 > Input.GetAxis("ClossHorizontal") && !ClossTilt)  //←入力時
        {
            if (chooseMain % 3 == 0) chooseMain += 2;
            else chooseMain -= 1;
            ClossTilt = true;
        }
        else if (0 < Input.GetAxis("ClossHorizontal") && !ClossTilt)    //→入力時
        {
            if (chooseMain % 3 == 2) chooseMain -= 2;
            else chooseMain += 1;
            ClossTilt = true;
        }

        if (0 == Input.GetAxis("ClossHorizontal") && (0 == Input.GetAxis("ClossVertical"))) ClossTilt = false;
    }
}
