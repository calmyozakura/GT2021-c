using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Combo : MonoBehaviour
{
    Text comboText;
    //Color comboColor;

    public float comboTime = 0f;
    public int comboCount = 0;
    float blinking = 0f;
    float blinkingSpeed = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        comboText = GetComponent<Text>();
        comboText.color = new Color(255, 255, 255, 0);  //コンボ表記透明
        //comboColor = GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (comboTime > 0)
        {
            comboTime -= Time.deltaTime;    //制限時間のカウントダウン
            comboText.text = comboCount + "Combo!";

            if (comboTime >= 2) comboText.color = new Color(255, 255, 255, 1.0f);  //絶対値でsin波を透明度に;
            else if (comboTime < 2)   //2秒以下で点滅
            {
                //blinking = (comboTime * blinkingSpeed) % 2;
                blinking = Mathf.Sin(2 * Mathf.PI * blinkingSpeed * Time.time); //sin波取得

                comboText.color = new Color(255, 255, 255, Mathf.Abs(blinking));  //絶対値でsin波を透明度に

                //comboColor.a = alpha_Sin;
                //GetComponent<Renderer>().material.color = comboColor;
            }
        }
        else if (comboTime <= 0)
        {
            comboCount = 0;
            comboText.color = new Color(255, 255, 255, 0);  //コンボ表記透明
        }
    }
}
