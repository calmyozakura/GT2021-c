using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMain : MonoBehaviour
{

    int[] mainNumber = new int[9];     //3*3のナンバー
    int[] sideNumber = new int[16];     //4*4のナンバー
    int tmpNumber;          //数字入れ替え時の一時保存
    int[] bonusLevel = new int[16]; //サイドパネルのボーナス確認 (0=なし,1=あり)
    bool[] bonusFlg = new bool[16]; //このターン既にボーナスパネルになったかどうか
    int tmpBonus;         //ボーナス入れ替え時の一時保存
    int judgNum = 0;  //和を計算する配列
    int score = 0;      //スコア

    int chooseMain = 0; //現在選んでいるメインナンバー

    bool ClossTilt;     //十字キーがニュートラルに戻ったか

    [SerializeField] GameObject[] mainText;  //メインナンバーのテキストオブジェクト
    [SerializeField] GameObject[] sideText;  //サイドナンバーのテキストオブジェクト
    [SerializeField] Image[] sideImage;      //ボーナスフラグで色を変える
    Text[] mainNumberText = new Text[9];   //メインナンバーを入れるテキスト
    Text[] sideNumberText = new Text[16];   //サイドナンバーを入れるテキスト

    [SerializeField] GameObject selectMainImage; //現在選択しているメインパネルを表示

    [SerializeField] GameObject Score;  //スコアのテキストオブジェクト
    Text scoreText;
    [SerializeField] GameObject Timer;  //制限時間のテキストオブジェクト
    Text timerText;
    float timeCount = 60.0f;            //制限時間

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 9; i++)
        {
            mainNumber[i] = Random.Range(2, 9);
            mainNumberText[i] = mainText[i].GetComponent<Text>();
            mainNumberText[i].text = "" + mainNumber[i];
        }

        for (int i = 0; i < 16; i++)
        {
            sideNumber[i] = Random.Range(2, 9);
            sideNumberText[i] = sideText[i].GetComponent<Text>();
            sideNumberText[i].text = "" + sideNumber[i];
        }

        scoreText = Score.GetComponent<Text>();
        timerText = Timer.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        PanelOperation();   //パネルの操作
        SelectImageMove();  //現在選んでいるパネルの可視化
        TimerCount();       //制限時間のカウントと表示

        //ゲーム終了
        if (Input.GetButtonDown("Y"))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
        }
    }

    void SideTextUpdate()
    {
        sideNumberText[(chooseMain / 3) + chooseMain].text = "" + sideNumber[(chooseMain / 3) + chooseMain];
        sideNumberText[(chooseMain / 3) + chooseMain + 1].text = "" + sideNumber[(chooseMain / 3) + chooseMain + 1];
        sideNumberText[(chooseMain / 3) + chooseMain + 5].text = "" + sideNumber[(chooseMain / 3) + chooseMain + 5];
        sideNumberText[(chooseMain / 3) + chooseMain + 4].text = "" + sideNumber[(chooseMain / 3) + chooseMain + 4];
    }

    void PanelOperation()
    {
        //パネル反時計回り
        if (Input.GetButtonDown("LB"))
        {
            //ナンバー入れ替え
            tmpNumber = sideNumber[(chooseMain / 3) + chooseMain];
            sideNumber[(chooseMain / 3) + chooseMain] = sideNumber[(chooseMain / 3) + chooseMain + 1];
            sideNumber[(chooseMain / 3) + chooseMain + 1] = sideNumber[(chooseMain / 3) + chooseMain + 5];
            sideNumber[(chooseMain / 3) + chooseMain + 5] = sideNumber[(chooseMain / 3) + chooseMain + 4];
            sideNumber[(chooseMain / 3) + chooseMain + 4] = tmpNumber;

            //ボーナス入れ替え
            tmpBonus = bonusLevel[(chooseMain / 3) + chooseMain];
            bonusLevel[(chooseMain / 3) + chooseMain] = bonusLevel[(chooseMain / 3) + chooseMain + 1];
            bonusLevel[(chooseMain / 3) + chooseMain + 1] = bonusLevel[(chooseMain / 3) + chooseMain + 5];
            bonusLevel[(chooseMain / 3) + chooseMain + 5] = bonusLevel[(chooseMain / 3) + chooseMain + 4];
            bonusLevel[(chooseMain / 3) + chooseMain + 4] = tmpBonus;

            SideTextUpdate();   //サイドパネルの更新
            PointCheck();        //各パネルの得点計算
        }
        //パネル時計回り
        if (Input.GetButtonDown("RB"))
        {
            //ナンバー入れ替え
            tmpNumber = sideNumber[(chooseMain / 3) + chooseMain];
            sideNumber[(chooseMain / 3) + chooseMain] = sideNumber[(chooseMain / 3) + chooseMain + 4];
            sideNumber[(chooseMain / 3) + chooseMain + 4] = sideNumber[(chooseMain / 3) + chooseMain + 5];
            sideNumber[(chooseMain / 3) + chooseMain + 5] = sideNumber[(chooseMain / 3) + chooseMain + 1];
            sideNumber[(chooseMain / 3) + chooseMain + 1] = tmpNumber;

            //ボーナス入れ替え
            tmpBonus = bonusLevel[(chooseMain / 3) + chooseMain];
            bonusLevel[(chooseMain / 3) + chooseMain] = bonusLevel[(chooseMain / 3) + chooseMain + 4];
            bonusLevel[(chooseMain / 3) + chooseMain + 4] = bonusLevel[(chooseMain / 3) + chooseMain + 5];
            bonusLevel[(chooseMain / 3) + chooseMain + 5] = bonusLevel[(chooseMain / 3) + chooseMain + 1];
            bonusLevel[(chooseMain / 3) + chooseMain + 1] = tmpBonus;

            SideTextUpdate();   //サイドパネルの更新
            PointCheck();        //各パネルの得点計算
        }

        //十字キーのパネル選択
        if(0 > Input.GetAxis("ClossVertical") && !ClossTilt)    //↓入力時
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
        if(0 > Input.GetAxis("ClossHorizontal") && !ClossTilt)  //←入力時
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

    void SelectImageMove()
    {
        selectMainImage.GetComponent<RectTransform>().anchoredPosition
            = new Vector2(-120 + (120 * (chooseMain % 3)), 100 - (100 * (chooseMain / 3)));
    }

    void PointCheck()
    {

        for (int i = 0; i < 9; i++)
        {
            judgNum += sideNumber[(i / 3) + i];
            judgNum += sideNumber[(i / 3) + i + 1];
            judgNum += sideNumber[(i / 3) + i + 5];
            judgNum += sideNumber[(i / 3) + i + 4];

            if (judgNum % mainNumber[i] == 0)   //倍数を満たした
            {
                //スコア+100と各パネルのボーナス分スコア+50
                score += 100 + (50 * (bonusLevel[(i / 3) + i] + bonusLevel[(i / 3) + i + 1] 
                    + bonusLevel[(i / 3) + i + 5] + bonusLevel[(i / 3) + i + 4]));

                scoreText.text = "" + score;

                Debug.Log(bonusLevel[(i / 3) + i]);
                Debug.Log(bonusLevel[(i / 3) + i + 1]);
                Debug.Log(bonusLevel[(i / 3) + i + 5]);
                Debug.Log(bonusLevel[(i / 3) + i + 4]);

                //ボーナスフラグon
                bonusFlg[(i / 3) + i] = true;
                bonusFlg[(i / 3) + i + 1] = true;
                bonusFlg[(i / 3) + i + 5] = true;
                bonusFlg[(i / 3) + i + 4] = true;

                //サイドパネルの色変更
                sideImage[(i / 3) + i].color = Color.green;
                sideImage[(i / 3) + i + 1].color = Color.green;
                sideImage[(i / 3) + i + 5].color = Color.green;
                sideImage[(i / 3) + i + 4].color = Color.green;

                //ランダムな数値にいれかえ
                mainNumber[i] = Random.Range(2, 9);
                sideNumber[(i / 3) + i] = Random.Range(2, 9);
                sideNumber[(i / 3) + i + 1] = Random.Range(2, 9);
                sideNumber[(i / 3) + i + 5] = Random.Range(2, 9);
                sideNumber[(i / 3) + i + 4] = Random.Range(2, 9);

                //パネルの数値のテキスト書き換え
                mainNumberText[i].text = "" + mainNumber[i];
                sideNumberText[(i / 3) + i].text = "" + sideNumber[(i / 3) + i];
                sideNumberText[(i / 3) + i + 1].text = "" + sideNumber[(i / 3) + i + 1];
                sideNumberText[(i / 3) + i + 5].text = "" + sideNumber[(i / 3) + i + 5];
                sideNumberText[(i / 3) + i + 4].text = "" + sideNumber[(i / 3) + i + 4];
            }

            judgNum = 0;
        }

        //最後にに全パネルのボーナスと色をリセット
        for (int i = 0; i < 16; i++)
        {

            //ボーナスフラグがあれば+1なければ-1(下限0)
            if(bonusLevel[i] > 0 && !bonusFlg[i]) bonusLevel[i] -= 1;
            if(bonusFlg[i]) bonusLevel[i] = 1;

            if (bonusLevel[i] == 0) sideImage[i].color = Color.gray;
            else sideImage[i].color = Color.green;

            bonusFlg[i] = false;    //このターンすでにボーナスになったかのリセット
        }
    }
    void TimerCount()
    {
        if (timeCount >= 0)
        {
            timeCount -= Time.deltaTime;    //制限時間のカウントダウン
            timerText.text = timeCount.ToString("f1");  //時間の表示
        }
    }
}
