using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    Text timerText;
    public float timeCount = 30.0f;            //制限時間
    public bool timeOut;
    public bool countStart;

    // Start is called before the first frame update
    void Start()
    {
        timerText = GetComponent<Text>();
        timerText.text = timeCount.ToString("f1");  //時間の表示
    }

    public void TimerCount()
    {
        timerText.text = timeCount.ToString("f1");  //時間の表示
        if (timeCount > 0 && countStart)
        {
            timeCount -= Time.deltaTime;    //制限時間のカウントダウン

        }else if (timeCount <= 0)
        {
            timeOut = true;
            countStart = false;
        }
    }
}
