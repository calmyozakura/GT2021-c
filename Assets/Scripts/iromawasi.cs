using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class iromawasi : MonoBehaviour
{

    int[] mainNumber = new int[4];     //2*2のナンバー
    int[] sideNumber = new int[9];     //3*3のナンバー
    int tmpNumber;          //数字入れ替え時の一時保存
    int[] bonusLevel = new int[9]; //サイドパネルのボーナス確認 (0=なし,1=あり)
    bool[] bonusFlg = new bool[9]; //このターン既にボーナスパネルになったかどうか
    int tmpBonus;         //ボーナス入れ替え時の一時保存
    int judgNum = 0;  //和を計算する配列
    int score = 0;      //スコア

    int chooseMain = 0; //現在選んでいるメインナンバー

    bool ClossTilt;     //十字キーがニュートラルに戻ったか

    [SerializeField] Image[] sideImage;      //ボーナスフラグで色を変える
    [SerializeField] Image[] mainImage;      //ボーナスフラグで色を変える
    int[] mainColorNumber = { 4, 32, 128, 512};    //メイン色の配列(赤、青、緑、黄)
    int[] sideColorNumber = { 1, 8, 32, 128 };     //サイド色の配列(赤、青、緑、黄)
    [SerializeField] GameObject selectMainImage; //現在選択しているメインパネルを表示

    [SerializeField] GameObject Score;  //スコアのテキストオブジェクト
    Text scoreText;
    [SerializeField] GameObject Timer;  //制限時間のテキストオブジェクト
    Text timerText;
    float timeCount = 10.0f;            //制限時間
    [SerializeField] GameObject Turn;   //ターンのテキストオブジェクト
    Text turnText;
    int nowTurn = 0;

    float alpha_Time = 1.0f;   //点滅させる時間
    float alpha_Sin;    //消すときに点滅させる
    bool alpha_Flg;
    int check = 0; //中身を順にみる変数

    bool[] flgCheck = new bool[5];  //ポイントになった箇所を記憶,5はnull
    int mainColorNum = 0;               //全パネルが同じ色になったら色を変える
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 4; i++)
        {
            mainNumber[i] = mainColorNumber[Random.Range(0, 1)];
        }

        for (int i = 0; i < 9; i++)
        {
            sideNumber[i] = sideColorNumber[Random.Range(0, 1)];
        }

        scoreText = Score.GetComponent<Text>();
        timerText = Timer.GetComponent<Text>();
        turnText = Turn.GetComponent<Text>();

        ColorChange();   //パネルの色変更
    }

    // Update is called once per frame
    void Update()
    {
        if (!alpha_Flg)
        {
            PanelOperation();   //パネルの操作
            ColorChange();   //パネルの色変更
            TimerCount();       //制限時間のカウントと表示
            SelectImageMove();  //現在選んでいるパネルの可視化
        }
        else if (alpha_Flg) alpha();

        //ゲーム終了
        if (Input.GetButtonDown("Y"))   //Yを押すか無限ループしたら終了
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
        }
    }

    void TurnCount()
    {
        nowTurn++;
        turnText.text = "" + nowTurn;
    }

    void alpha()
    {
        if (flgCheck[check] && check <= 3)   //0~3で条件を満たしたら
        {
            if (alpha_Time >= 0)    //条件を満たしたパネルの点滅
            {
                alpha_Time -= Time.deltaTime;    //制限時間のカウントダウン
                alpha_Sin = alpha_Time;
                //alpha_Sin = Mathf.Sin(Time.time) / 2 + 0.5f;

                sideImage[(check / 2) + check].color = new Color(1.0f, 1.0f, 1.0f, alpha_Sin);    //透明度を下げる
                sideImage[(check / 2) + check + 1].color = new Color(1.0f, 1.0f, 1.0f, alpha_Sin);    //透明度を下げる
                sideImage[(check / 2) + check + 4].color = new Color(1.0f, 1.0f, 1.0f, alpha_Sin);    //透明度を下げる
                sideImage[(check / 2) + check + 3].color = new Color(1.0f, 1.0f, 1.0f, alpha_Sin);    //透明度を下げる
            }
            else if (alpha_Time <= 0)
            {
                //PointCheck();
                alpha_Time = 1.0f;
                //flgCheck[check] = false;
                ColorChange();

                //スコア+100と各パネルのボーナス分スコア+50
                score += 100 + (50 * (bonusLevel[(check / 2) + check] + bonusLevel[(check / 2) + check + 1]
                    + bonusLevel[(check / 2) + check + 4] + bonusLevel[(check / 2) + check + 3]));

                scoreText.text = "" + score;
                check += 1;
            }
        }
        else if (check > 3)    //最後に盤面を変える
        {
            for (int i = 0; i < 4; i++)
            {
                if (flgCheck[i])
                {
                    //ランダムな数値にいれかえ
                    mainNumber[i] = mainColorNumber[Random.Range(0, 2)];
                    //mainNumber[i] = mainColorNumber[0];
                    sideNumber[(i / 2) + i] = sideColorNumber[Random.Range(0, 2)];
                    sideNumber[(i / 2) + i + 1] = sideColorNumber[Random.Range(0, 2)];
                    sideNumber[(i / 2) + i + 4] = sideColorNumber[Random.Range(0, 2)];
                    sideNumber[(i / 2) + i + 3] = sideColorNumber[Random.Range(0, 2)];
                }
                mainColorNum += mainNumber[i];
            }

            for (int j = 0; j < 4; j++)  //4箇所の合計と色*4を見る { 4, 32, 128, 512}
            {
                while (mainColorNum == (mainColorNumber[j] * 4))  //4色同じだったら処理を繰り返す
                {
                    mainColorNum = 0;   //一度numを0にし
                    for (int f = 0; f < 4; f++)
                    {
                        if(flgCheck[f]) mainNumber[f] = mainColorNumber[Random.Range(0, 2)]; //消したマスをランダムな色に変えて
                        mainColorNum += mainNumber[f];       //もう一度numを入れなおす
                    }
                    Debug.Log("色変え"); //色変えと出て変わった
                }
            }
            for(int f = 0; f < 4; f++) flgCheck[f] = false; //念のため別のforでfalseにする
            mainColorNum = 0;
            check = 0;
            alpha_Flg = false;
        }
        else check += 1;
    }

    void PanelOperation()
    {
        //パネル反時計回り
        if (Input.GetButtonDown("LB"))
        {
            //ナンバー入れ替え
            tmpNumber = sideNumber[(chooseMain / 2) + chooseMain];
            sideNumber[(chooseMain / 2) + chooseMain] = sideNumber[(chooseMain / 2) + chooseMain + 1];
            sideNumber[(chooseMain / 2) + chooseMain + 1] = sideNumber[(chooseMain / 2) + chooseMain + 4];
            sideNumber[(chooseMain / 2) + chooseMain + 4] = sideNumber[(chooseMain / 2) + chooseMain + 3];
            sideNumber[(chooseMain / 2) + chooseMain + 3] = tmpNumber;

            //ボーナス入れ替え
            tmpBonus = bonusLevel[(chooseMain / 2) + chooseMain];
            bonusLevel[(chooseMain / 2) + chooseMain] = bonusLevel[(chooseMain / 3) + chooseMain + 1];
            bonusLevel[(chooseMain / 2) + chooseMain + 1] = bonusLevel[(chooseMain / 3) + chooseMain + 4];
            bonusLevel[(chooseMain / 2) + chooseMain + 4] = bonusLevel[(chooseMain / 3) + chooseMain + 3];
            bonusLevel[(chooseMain / 2) + chooseMain + 3] = tmpBonus;

            //PointCheck();        //各パネルの得点計算
        }
        //パネル時計回り
        if (Input.GetButtonDown("RB"))
        {
            //ナンバー入れ替え
            tmpNumber = sideNumber[(chooseMain / 2) + chooseMain];
            sideNumber[(chooseMain / 2) + chooseMain] = sideNumber[(chooseMain / 2) + chooseMain + 3];
            sideNumber[(chooseMain / 2) + chooseMain + 3] = sideNumber[(chooseMain / 2) + chooseMain + 4];
            sideNumber[(chooseMain / 2) + chooseMain + 4] = sideNumber[(chooseMain / 2) + chooseMain + 1];
            sideNumber[(chooseMain / 2) + chooseMain + 1] = tmpNumber;

            //ボーナス入れ替え
            tmpBonus = bonusLevel[(chooseMain / 2) + chooseMain];
            bonusLevel[(chooseMain / 2) + chooseMain] = bonusLevel[(chooseMain / 2) + chooseMain + 3];
            bonusLevel[(chooseMain / 2) + chooseMain + 3] = bonusLevel[(chooseMain / 2) + chooseMain + 4];
            bonusLevel[(chooseMain / 2) + chooseMain + 4] = bonusLevel[(chooseMain / 2) + chooseMain + 1];
            bonusLevel[(chooseMain / 2) + chooseMain + 1] = tmpBonus;


            //PointCheck();        //各パネルの得点計算
        }

        //十字キーのパネル選択
        if(0 > Input.GetAxis("ClossVertical") && !ClossTilt)    //↓入力時
        {
            if (chooseMain >= 2) chooseMain -= 2;
            else chooseMain += 2;
            ClossTilt = true;
        }
        else if (0 < Input.GetAxis("ClossVertical") && !ClossTilt)  //↑入力時
        {
            if (chooseMain <= 1) chooseMain += 2;
            else chooseMain -= 2;
            ClossTilt = true;
        }
        if(0 > Input.GetAxis("ClossHorizontal") && !ClossTilt)  //←入力時
        {
            if (chooseMain % 2 == 0) chooseMain += 1;
            else chooseMain -= 1;
            ClossTilt = true;
        }
        else if (0 < Input.GetAxis("ClossHorizontal") && !ClossTilt)    //→入力時
        {
            if (chooseMain % 2 == 1) chooseMain -= 1;
            else chooseMain += 1;
            ClossTilt = true;
        }

        if (0 == Input.GetAxis("ClossHorizontal") && (0 == Input.GetAxis("ClossVertical"))) ClossTilt = false;
    }

    void SelectImageMove()
    {
        selectMainImage.GetComponent<RectTransform>().anchoredPosition
            = new Vector2(-60 + (120 * (chooseMain % 2)), 50 - (100 * (chooseMain / 2)));
    }

    void PointCheck()
    {

        for (int i = 0; i < 4; i++)
        {

            judgNum += sideNumber[(i / 2) + i];
            judgNum += sideNumber[(i / 2) + i + 1];
            judgNum += sideNumber[(i / 2) + i + 4];
            judgNum += sideNumber[(i / 2) + i + 3];

            if (judgNum == mainNumber[i])   //色を満たした
            {
                flgCheck[i] = true;
                alpha_Flg = true;

                ////スコア+100と各パネルのボーナス分スコア+50
                //score += 100 + (50 * (bonusLevel[(i / 2) + i] + bonusLevel[(i / 2) + i + 1]
                //    + bonusLevel[(i / 2) + i + 4] + bonusLevel[(i / 2) + i + 3]));

                //scoreText.text = "" + score;

                //Debug.Log(bonusLevel[(i / 2) + i]);
                //Debug.Log(bonusLevel[(i / 2) + i + 1]);
                //Debug.Log(bonusLevel[(i / 2) + i + 4]);
                //Debug.Log(bonusLevel[(i / 2) + i + 3]);

                //ボーナスフラグon
                bonusFlg[(i / 2) + i] = true;
                bonusFlg[(i / 2) + i + 1] = true;
                bonusFlg[(i / 2) + i + 4] = true;
                bonusFlg[(i / 2) + i + 3] = true;

                //alpha_Time = 3.0f;
            }

            //ColorChange();   //パネルの色変更
            judgNum = 0;
        }

        //最後にに全パネルのボーナスと色をリセット
        //for (int i = 0; i < 9; i++)
        //{

        //    //ボーナスフラグがあれば+1なければ-1(下限0)
        //    if(bonusLevel[i] > 0 && !bonusFlg[i]) bonusLevel[i] -= 1;
        //    if(bonusFlg[i]) bonusLevel[i] = 1;

        //    if (bonusLevel[i] == 0) sideImage[i].color = Color.gray;
        //    else sideImage[i].color = Color.green;

        //    bonusFlg[i] = false;    //このターンすでにボーナスになったかのリセット
        //}

    }
    void TimerCount()
    {
        if (timeCount >= 0)
        {
            timeCount -= Time.deltaTime;    //制限時間のカウントダウン
            if (Input.GetButtonDown("X")) timeCount = 0.0f; //Xボタンでターン即終了

            timerText.text = timeCount.ToString("f1");  //時間の表示

            if (timeCount <= 0)
            {
                timeCount = 10.0f;
                if (!alpha_Flg) PointCheck();
                TurnCount();        //経過ターンの更新表示
            }

        }
    }

    void ColorChange()
    {
        for (int i= 0; i< 4; i++)
        {
            switch (mainNumber[i])
            {
                case 4:
                    mainImage[i].color = Color.red;
                    break;
                case 32:
                    mainImage[i].color = Color.blue;
                    break;
                case 128:
                    mainImage[i].color = Color.green;
                    break;
                case 512:
                    mainImage[i].color = Color.yellow;
                    break;
                default:
                    break;
            }
        }

        for(int i = 0;i < 9; i++)
        {
            switch (sideNumber[i])
            {
                case 1:
                    sideImage[i].color = Color.red;
                    break;
                case 8:
                    sideImage[i].color = Color.blue;
                    break;
                case 32:
                    sideImage[i].color = Color.green;
                    break;
                case 128:
                    sideImage[i].color = Color.yellow;
                    break;
                default:
                    break;
            }
        }
    }
}
