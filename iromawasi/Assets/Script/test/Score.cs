using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Score : MonoBehaviour
{
    Text scoreText;

    int score = 0;      //スコア
    // Start is called before the first frame update
    void Start()
    {
        scoreText = this.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ScoreAdd(int[] bonusLevel,int check)
    {
        //スコア+100と各パネルのボーナス分スコア+50
        score += 100 + (50 * (bonusLevel[(check / 3) + check] + bonusLevel[(check / 3) + check + 1]
            + bonusLevel[(check / 3) + check + 5] + bonusLevel[(check / 3) + check + 4]));

        scoreText.text = "" + score;
    }
}
