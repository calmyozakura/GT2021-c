using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class iromawasi : MonoBehaviour
{
    const int mainPanel = 4;    //メインパネルの数
    const int sidePanel = 9;    //サイドパネルの数

    int[] mainNumber = new int[mainPanel];     //2*2のナンバー
    int[] sideNumber = new int[sidePanel];     //3*3のナンバー
    int tmpNumber;          //数字入れ替え時の一時保存
    int[] bonusLevel = new int[sidePanel]; //サイドパネルのボーナス確認 (0=なし,1=あり)
    bool[] bonusFlg = new bool[sidePanel]; //このターン既にボーナスパネルになったかどうか
    int tmpBonus;         //ボーナス入れ替え時の一時保存
    int judgNum = 0;  //和を計算する配列
    int score = 0;      //スコア

    int chooseMain = 0; //現在選んでいるメインナンバー

    bool ClossTilt;     //十字キーがニュートラルに戻ったか

    [SerializeField] Image[] sideImage;
    [SerializeField] GameObject[] obj;  //アニメーションさせるためのオブジェクト
    [SerializeField] PanelAnim[] panelAnim;
    PanelAnim tmpAnim;
    GameObject tmpObj;
    [SerializeField] Image[] sideBonusFrame;
    [SerializeField] Image[] mainImage;
    int[] mainColorNumber = { 4, 32, 128, 512 };    //メイン色の配列(赤、青、緑、黄)
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

    bool[] flgCheck = new bool[mainPanel + 1];  //ポイントになった箇所を記憶,5はnull
    int mainColorNum = 0;               //全パネルが同じ色になったら色を変える

    bool[] panelMove = new bool[2]; //右か左にパネル移動させるフラグ
    float changeTime = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < mainPanel; i++)
        {
            mainNumber[i] = mainColorNumber[Random.Range(0, 1)];
        }

        for (int i = 0; i < sidePanel; i++)
        {
            sideNumber[i] = sideColorNumber[Random.Range(0, 1)];
            panelAnim[i] = obj[i].GetComponent<PanelAnim>();
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
            if (!panelMove[0] && !panelMove[1]) PanelOperation();   //パネルの操作
            else if (panelMove[0] || panelMove[1]) PanelMove();        //パネルのアニメーション
            ColorChange();   //パネルの色変更
            TimerCount();       //制限時間のカウントと表示
            SelectImageMove();  //現在選んでいるパネルの可視化
        }
        else if (alpha_Flg) alpha();

        //ゲーム終了
        if (Input.GetButtonDown("Y"))   //Yを押したら終了
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
            for (int i = 0; i < mainPanel; i++)
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
                mainColorNum += mainNumber[i];  //[0]^[3]の合計を得る
            }

            for (int j = 0; j < mainPanel; j++)  //[0]^[3]の合計と色*4を見る { 4, 32, 128, 512}
            {
                while (mainColorNum == (mainColorNumber[j] * 4))  //4色同じだったら処理を繰り返す
                {
                    mainColorNum = 0;   //一度numを0にし
                    for (int f = 0; f < mainPanel; f++)
                    {
                        if (flgCheck[f]) mainNumber[f] = mainColorNumber[Random.Range(0, 2)]; //消したマスをランダムな色に変えて
                        mainColorNum += mainNumber[f];       //もう一度[0]^[3]の合計を得る
                    }
                }
            }
            Bonus();    //ボーナスパネル設定
            for (int f = 0; f < mainPanel; f++) flgCheck[f] = false; //念のため別のforでfalseにする
            mainColorNum = 0;
            ColorChange();   //パネルの色変更
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

            panelMove[0] = true;

            //パネルの回転アニメーション
            panelAnim[(chooseMain / 2) + chooseMain].animFlg[1] = true; //down
            panelAnim[(chooseMain / 2) + chooseMain + 1].animFlg[2] = true; //left
            panelAnim[(chooseMain / 2) + chooseMain + 4].animFlg[3] = true; //up
            panelAnim[(chooseMain / 2) + chooseMain + 3].animFlg[0] = true; //right

            //スクリプト入れ替え
            //tmpAnim = panelAnim[(chooseMain / 2) + chooseMain];
            //panelAnim[(chooseMain / 2) + chooseMain] = panelAnim[(chooseMain / 2) + chooseMain + 1];
            //panelAnim[(chooseMain / 2) + chooseMain + 1] = panelAnim[(chooseMain / 2) + chooseMain + 4];
            //panelAnim[(chooseMain / 2) + chooseMain + 4] = panelAnim[(chooseMain / 2) + chooseMain + 3];
            //panelAnim[(chooseMain / 2) + chooseMain + 3] = tmpAnim;
        }
        //パネル時計回り
        if (Input.GetButtonDown("RB"))
        {
            panelMove[1] = true;

            //パネルの回転アニメーション
            panelAnim[(chooseMain / 2) + chooseMain].animFlg[0] = true; //right
            panelAnim[(chooseMain / 2) + chooseMain + 1].animFlg[1] = true; //down
            panelAnim[(chooseMain / 2) + chooseMain + 4].animFlg[2] = true; //up
            panelAnim[(chooseMain / 2) + chooseMain + 3].animFlg[3] = true; //left

            ////スクリプト入れ替え
            //tmpAnim = panelAnim[(chooseMain / 2) + chooseMain];
            //panelAnim[(chooseMain / 2) + chooseMain] = panelAnim[(chooseMain / 2) + chooseMain + 3];
            //panelAnim[(chooseMain / 2) + chooseMain + 3] = panelAnim[(chooseMain / 2) + chooseMain + 4];
            //panelAnim[(chooseMain / 2) + chooseMain + 4] = panelAnim[(chooseMain / 2) + chooseMain + 1];
            //panelAnim[(chooseMain / 2) + chooseMain + 1] = tmpAnim;
        }

        //十字キーのパネル選択
        if (0 > Input.GetAxis("ClossVertical") && !ClossTilt)    //↓入力時
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
        if (0 > Input.GetAxis("ClossHorizontal") && !ClossTilt)  //←入力時
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

        for (int i = 0; i < mainPanel; i++)
        {

            judgNum += sideNumber[(i / 2) + i];
            judgNum += sideNumber[(i / 2) + i + 1];
            judgNum += sideNumber[(i / 2) + i + 4];
            judgNum += sideNumber[(i / 2) + i + 3];

            if (judgNum == mainNumber[i])   //色を満たした
            {
                flgCheck[i] = true;

                //ボーナスフラグon
                bonusFlg[(i / 2) + i] = true;
                bonusFlg[(i / 2) + i + 1] = true;
                bonusFlg[(i / 2) + i + 4] = true;
                bonusFlg[(i / 2) + i + 3] = true;
            }

            judgNum = 0;
        }

        alpha_Flg = true;
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

    public void ColorChange()
    {
        for (int i = 0; i < mainPanel; i++)
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

        for (int i = 0; i < sidePanel; i++)
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

            if (bonusLevel[i] == 0) sideBonusFrame[i].color = Color.gray;
            else if (bonusLevel[i] > 0) sideBonusFrame[i].color = Color.yellow;
        }
    }

    void Bonus()
    {
        for (int f = 0; f < sidePanel; f++)
        {
            //ボーナスフラグがあれば+1なければ-1(下限0)
            if (bonusLevel[f] > 0 && !bonusFlg[f]) bonusLevel[f] -= 1;
            if (bonusFlg[f]) bonusLevel[f] = 1;

            //if (bonusLevel[f] == 0) sideBonusFrame[f].color = Color.gray;
            //else if(bonusLevel[f] > 0) sideBonusFrame[f].color = Color.yellow;

            bonusFlg[f] = false;
        }
    }

    void PanelMove()    //アニメーション終了時に呼ぶ
    {
        if (changeTime > 0) changeTime -= Time.deltaTime;
        else if (changeTime <= 0)
        {
            if (panelMove[0])   //反時計周り
            {
                //ナンバー入れ替え
                tmpNumber = sideNumber[(chooseMain / 2) + chooseMain];
                sideNumber[(chooseMain / 2) + chooseMain] = sideNumber[(chooseMain / 2) + chooseMain + 1];
                sideNumber[(chooseMain / 2) + chooseMain + 1] = sideNumber[(chooseMain / 2) + chooseMain + 4];
                sideNumber[(chooseMain / 2) + chooseMain + 4] = sideNumber[(chooseMain / 2) + chooseMain + 3];
                sideNumber[(chooseMain / 2) + chooseMain + 3] = tmpNumber;

                //ボーナス入れ替え
                tmpBonus = bonusLevel[(chooseMain / 2) + chooseMain];
                bonusLevel[(chooseMain / 2) + chooseMain] = bonusLevel[(chooseMain / 2) + chooseMain + 1];
                bonusLevel[(chooseMain / 2) + chooseMain + 1] = bonusLevel[(chooseMain / 2) + chooseMain + 4];
                bonusLevel[(chooseMain / 2) + chooseMain + 4] = bonusLevel[(chooseMain / 2) + chooseMain + 3];
                bonusLevel[(chooseMain / 2) + chooseMain + 3] = tmpBonus;

                ////オブジェクト入れ替え
                //tmpObj = obj[(chooseMain / 2) + chooseMain];
                //obj[(chooseMain / 2) + chooseMain] = obj[(chooseMain / 2) + chooseMain + 1];
                //obj[(chooseMain / 2) + chooseMain + 1] = obj[(chooseMain / 2) + chooseMain + 4];
                //obj[(chooseMain / 2) + chooseMain + 4] = obj[(chooseMain / 2) + chooseMain + 3];
                //obj[(chooseMain / 2) + chooseMain + 3] = tmpObj;

                panelMove[0] = false;
            }
            else if (panelMove[1])  //時計周り
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

                panelMove[1] = false;
            }

            //panelAnim[0].change = false;
            //ColorChange();   //パネルの色変更
            changeTime = 0.1f;
        }
    }
}