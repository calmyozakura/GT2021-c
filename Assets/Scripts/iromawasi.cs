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
    float timeCount = 60.0f;            //制限時間

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 4; i++)
        {
            mainNumber[i] = mainColorNumber[Random.Range(0, 2)];
        }

        for (int i = 0; i < 9; i++)
        {
            sideNumber[i] = sideColorNumber[Random.Range(0, 2)];
        }

        scoreText = Score.GetComponent<Text>();
        timerText = Timer.GetComponent<Text>();

        ColorChange();   //パネルの色変更
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

            PointCheck();        //各パネルの得点計算
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


            PointCheck();        //各パネルの得点計算
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
                //スコア+100と各パネルのボーナス分スコア+50
                score += 100 + (50 * (bonusLevel[(i / 2) + i] + bonusLevel[(i / 2) + i + 1] 
                    + bonusLevel[(i / 2) + i + 4] + bonusLevel[(i / 2) + i + 3]));

                scoreText.text = "" + score;

                //Debug.Log(bonusLevel[(i / 2) + i]);
                //Debug.Log(bonusLevel[(i / 2) + i + 1]);
                //Debug.Log(bonusLevel[(i / 2) + i + 4]);
                //Debug.Log(bonusLevel[(i / 2) + i + 3]);

                //ボーナスフラグon
                bonusFlg[(i / 2) + i] = true;
                bonusFlg[(i / 2) + i + 1] = true;
                bonusFlg[(i / 2) + i + 4] = true;
                bonusFlg[(i / 2) + i + 3] = true;

                ////サイドパネルの色変更
                //sideImage[(i / 2) + i].color = Color.green;
                //sideImage[(i / 2) + i + 1].color = Color.green;
                //sideImage[(i / 2) + i + 4].color = Color.green;
                //sideImage[(i / 2) + i + 3].color = Color.green;

                //ランダムな数値にいれかえ
                mainNumber[i] = mainColorNumber[Random.Range(0, 2)];
                sideNumber[(i / 2) + i] = sideColorNumber[Random.Range(0, 2)];
                sideNumber[(i / 2) + i + 1] = sideColorNumber[Random.Range(0, 2)];
                sideNumber[(i / 2) + i + 4] = sideColorNumber[Random.Range(0, 2)];
                sideNumber[(i / 2) + i + 3] = sideColorNumber[Random.Range(0, 2)];
            }

            ColorChange();   //パネルの色変更
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
            timerText.text = timeCount.ToString("f1");  //時間の表示
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
