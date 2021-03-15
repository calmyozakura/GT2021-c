using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    Text timerText;
    float timeCount = 60.0f;            //制限時間

    // Start is called before the first frame update
    void Start()
    {
        timerText = GetComponent<Text>();
    }

    public void TimerCount()
    {
        if (timeCount >= 0)
        {
            timeCount -= Time.deltaTime;    //制限時間のカウントダウン

            timerText.text = timeCount.ToString("f1");  //時間の表示
        }
    }
}
