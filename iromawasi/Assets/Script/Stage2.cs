using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage2 : MonoBehaviour
{
    const int mainPanel = 9;    //メインパネルの数
    const int sidePanel = 16;    //サイドパネルの数

    int mainNumber;     //3*3のナンバー
    int[] sideNumber = new int[sidePanel];     //4*4のナンバー
    int tmpNumber;          //数字入れ替え時の一時保存
    int[] bonusLevel = new int[sidePanel]; //サイドパネルのボーナス確認 (0=なし,1=あり)
    bool[] bonusFlg = new bool[sidePanel]; //このターン既にボーナスパネルになったかどうか
    int tmpBonus;         //ボーナス入れ替え時の一時保存
    int judgNum = 0;  //和を計算する配列
    int score = 0;      //スコア

    int chooseMain = 0; //現在選んでいるメインナンバー

    bool ClossTilt;     //十字キーがニュートラルに戻ったか

    //[SerializeField] Image[] sideImage; //サイドスフィアをいれる
    [SerializeField] GameObject[] sideSphere; //サイドスフィアをいれる
    Color[] sideSphereColor = new Color[sidePanel];  //マテリアル色を変えるための仮入れ
    Color tmpSideColor;
    GameObject tmpObj;
    Renderer[] sideSphereRenderer = new Renderer[sidePanel];    //実際にオブジェクトの色を変更する
    //[SerializeField] GameObject[] obj;  //アニメーションさせるためのオブジェクト一時消し
    [SerializeField] PanelAnim[] panelAnim;
    PanelAnim tmpAnim;

    //GameObject tmpObj;
    //[SerializeField] Image[] sideBonusFrame;一時消し
    [SerializeField] GameObject mainSphere;
    Color mainSphereColor;  //マテリアル色を変えるための仮入れ
    Color tmpMainColor;
    Renderer mainSphereRenderer;    //実際にオブジェクトの色を変更する

    int[] mainColorNumber = { 4, 32, 128, 512 };    //メイン色の配列(赤、青、緑、黄)
    int[] sideColorNumber = { 1, 8, 32, 128 };     //サイド色の配列(赤、青、緑、黄)
    [SerializeField] GameObject selectMainImage; //現在選択しているメインパネルを表示

    [SerializeField] GameObject Score;  //スコアのテキストオブジェクト
    Text scoreText;
    [SerializeField] GameObject Timer;  //制限時間のテキストオブジェクト
    Text timerText;
    float timeCount = 10.0f;            //制限時間

    [SerializeField] GameObject gameTimer;  //制限時間のテキストオブジェクト
    Text gametimerText;
    float gametimeCount = 60.0f;            //制限時間

    //[SerializeField] GameObject Turn;   //ターンのテキストオブジェクト
    //Text turnText;
    //int nowTurn = 0;

    float alpha_Time = 1.0f;   //点滅させる時間
    float alpha_Sin;    //消すときに点滅させる
    bool alpha_Flg;
    int check = 0; //中身を順にみる変数

    bool[] flgCheck = new bool[mainPanel + 1];  //ポイントになった箇所を記憶,5はnull
    int mainColorNum = 0;               //全パネルが同じ色になったら色を変える

    bool[] panelMove = new bool[2]; //右か左にパネル移動させるフラグ
    float changeTime = 0.15f;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < mainPanel; i++)
        {
            mainNumber = mainColorNumber[Random.Range(0, 3)];
            mainSphereColor = mainSphere.GetComponent<Renderer>().material.color;
        }

        for (int i = 0; i < sidePanel; i++)
        {
            sideNumber[i] = sideColorNumber[Random.Range(0, 3)];
            sideSphereColor[i] = sideSphere[i].GetComponent<Renderer>().material.color;
            //panelAnim[i] = obj[i].GetComponent<PanelAnim>();一時消し
        }

        scoreText = Score.GetComponent<Text>();
        timerText = Timer.GetComponent<Text>();
        gametimerText = gameTimer.GetComponent<Text>();
        //turnText = Turn.GetComponent<Text>();

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
            PointCheck();
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
        mainNumber = mainColorNumber[Random.Range(0, 3)];
        //nowTurn++;
        //turnText.text = "" + nowTurn;
    }

    void alpha()
    {
        if (flgCheck[check] && check <= (mainPanel - 1))   //0~9で条件を満たしたら
        {
            if (alpha_Time >= 0)    //条件を満たしたパネルの点滅
            {
                alpha_Time -= Time.deltaTime * 2;    //制限時間のカウントダウン
                alpha_Sin = alpha_Time;
                //alpha_Sin = Mathf.Sin(Time.time) / 2 + 0.5f;

                sideSphereColor[(check / 3) + check].a = alpha_Sin; //透明度を下げる
                sideSphereColor[(check / 3) + check + 1].a = alpha_Sin; //透明度を下げる
                sideSphereColor[(check / 3) + check + 4].a = alpha_Sin; //透明度を下げる
                sideSphereColor[(check / 3) + check + 5].a = alpha_Sin; //透明度を下げる

                sideSphere[(check / 3) + check].GetComponent<Renderer>().material.color = sideSphereColor[(check / 3) + check];
                sideSphere[(check / 3) + check + 1].GetComponent<Renderer>().material.color = sideSphereColor[(check / 3) + check + 1];
                sideSphere[(check / 3) + check + 4].GetComponent<Renderer>().material.color = sideSphereColor[(check / 3) + check + 4];
                sideSphere[(check / 3) + check + 5].GetComponent<Renderer>().material.color = sideSphereColor[(check / 3) + check + 5];
            }
            else if (alpha_Time <= 0)
            {
                alpha_Time = 1.0f;
                //flgCheck[check] = false;
                ColorChange();

                //スコア+100と各パネルのボーナス分スコア+50
                score += 100 + (50 * (bonusLevel[(check / 3) + check] + bonusLevel[(check / 3) + check + 1]
                    + bonusLevel[(check / 3) + check + 5] + bonusLevel[(check / 3) + check + 4]));

                scoreText.text = "" + score;
                check += 1;
            }
        }
        else if (check > (mainPanel - 1))    //最後に盤面を変える
        {
            for (int i = 0; i < mainPanel; i++)
            {
                if (flgCheck[i])
                {
                    //ランダムな数値にいれかえ
                    //mainNumber = mainColorNumber[Random.Range(0, 3)];
                    sideNumber[(i / 3) + i] = sideColorNumber[Random.Range(0, 3)];
                    sideNumber[(i / 3) + i + 1] = sideColorNumber[Random.Range(0, 3)];
                    sideNumber[(i / 3) + i + 5] = sideColorNumber[Random.Range(0, 3)];
                    sideNumber[(i / 3) + i + 4] = sideColorNumber[Random.Range(0, 3)];
                }
                //mainColorNum += mainNumber[i];  //[0]^[3]の合計を得る 一時消し
            }

            //for (int j = 0; j < mainPanel; j++)  //[0]^[9]の合計と色*4を見る { 4, 32, 128, 512} 一時消し
            //{
            //    //while (mainColorNum == (mainColorNumber[j] * mainPanel))  //9色同じだったら処理を繰り返す
            //    //{
            //    //    mainColorNum = 0;   //一度numを0にし
            //    //    for (int f = 0; f < mainPanel; f++)
            //    //    {
            //    //        if (flgCheck[f]) mainNumber[f] = mainColorNumber[Random.Range(0, 2)]; //消したマスをランダムな色に変えて
            //    //        mainColorNum += mainNumber[f];       //もう一度[0]^[3]の合計を得る
            //    //    }
            //    //}
            //}
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

    void SelectImageMove()
    {
        selectMainImage.GetComponent<RectTransform>().anchoredPosition
            = new Vector2(-2 + (2 * (chooseMain % 3)), 2 - (2 * (chooseMain / 3)));
    }

    void PointCheck()
    {

        for (int i = 0; i < mainPanel; i++)
        {

            judgNum += sideNumber[(i / 3) + i];
            judgNum += sideNumber[(i / 3) + i + 1];
            judgNum += sideNumber[(i / 3) + i + 5];
            judgNum += sideNumber[(i / 3) + i + 4];

            if (judgNum == mainNumber)   //色を満たした
            {
                flgCheck[i] = true;

                //ボーナスフラグon
                bonusFlg[(i / 3) + i] = true;
                bonusFlg[(i / 3) + i + 1] = true;
                bonusFlg[(i / 3) + i + 5] = true;
                bonusFlg[(i / 3) + i + 4] = true;

                alpha_Flg = true;
            }

            judgNum = 0;
        }
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
                //if (!alpha_Flg) PointCheck();一時消し
                TurnCount();        //経過ターンの更新表示
            }

        }
        if (gametimeCount >= 0)
        {
            gametimeCount -= Time.deltaTime;    //制限時間のカウントダウン

            gametimerText.text = gametimeCount.ToString("f1");  //時間の表示

            if (gametimeCount <= 0)
            {
            }

        }
    }

    public void ColorChange()
    {
        for (int i = 0; i < mainPanel; i++)
        {
            switch (mainNumber)
            {
                case 4:
                    mainSphereColor = Color.red;
                    mainSphere.GetComponent<Renderer>().material.color = mainSphereColor;
                    break;
                case 32:
                    mainSphereColor = Color.blue;
                    mainSphere.GetComponent<Renderer>().material.color = mainSphereColor;
                    break;
                case 128:
                    mainSphereColor = Color.yellow;
                    mainSphere.GetComponent<Renderer>().material.color = mainSphereColor;
                    break;
                case 512:
                    mainSphereColor = Color.green;
                    mainSphere.GetComponent<Renderer>().material.color = mainSphereColor;
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
                    sideSphereColor[i] = Color.red;
                    sideSphere[i].GetComponent<Renderer>().material.color = sideSphereColor[i];
                    break;
                case 8:
                    sideSphereColor[i] = Color.blue;
                    sideSphere[i].GetComponent<Renderer>().material.color = sideSphereColor[i];
                    break;
                case 32:
                    sideSphereColor[i] = Color.yellow;
                    sideSphere[i].GetComponent<Renderer>().material.color = sideSphereColor[i];
                    break;
                case 128:
                    sideSphereColor[i] = Color.green;
                    sideSphere[i].GetComponent<Renderer>().material.color = sideSphereColor[i];
                    break;
                default:
                    break;
            }

            //一時消
            //if (bonusLevel[i] == 0) sideBonusFrame[i].color = Color.gray; //ボーナスがなければ灰縁
            //else if (bonusLevel[i] > 0) sideBonusFrame[i].color = Color.yellow;   //ボーナスがあれば金縁
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

    void PanelMove()
    {
        if (changeTime > 0) changeTime -= Time.deltaTime;
        else if (changeTime <= 0)
        {
            if (panelMove[0])   //反時計周り
            {
                //パネルの回転アニメーション
                panelAnim[(chooseMain / 3) + chooseMain].animFlg[1] = true; //down
                panelAnim[(chooseMain / 3) + chooseMain + 1].animFlg[2] = true; //left
                panelAnim[(chooseMain / 3) + chooseMain + 5].animFlg[3] = true; //up
                panelAnim[(chooseMain / 3) + chooseMain + 4].animFlg[0] = true; //right

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

                ////色の入れ替え
                //tmpSideColor = sideSphereColor[(chooseMain / 3) + chooseMain];
                //sideSphereColor[(chooseMain / 3) + chooseMain] = sideSphereColor[(chooseMain / 3) + chooseMain + 1];
                //sideSphereColor[(chooseMain / 3) + chooseMain + 1] = sideSphereColor[(chooseMain / 3) + chooseMain + 5];
                //sideSphereColor[(chooseMain / 3) + chooseMain + 5] = sideSphereColor[(chooseMain / 3) + chooseMain + 4];
                //sideSphereColor[(chooseMain / 3) + chooseMain + 4] = tmpSideColor;

                //オブジェクト入れ替え
                tmpObj = sideSphere[(chooseMain / 3) + chooseMain];
                sideSphere[(chooseMain / 3) + chooseMain] = sideSphere[(chooseMain / 3) + chooseMain + 1];
                sideSphere[(chooseMain / 3) + chooseMain + 1] = sideSphere[(chooseMain / 3) + chooseMain + 5];
                sideSphere[(chooseMain / 3) + chooseMain + 5] = sideSphere[(chooseMain / 3) + chooseMain + 4];
                sideSphere[(chooseMain / 3) + chooseMain + 4] = tmpObj;

                //スクリプト入れ替え
                tmpAnim = panelAnim[(chooseMain / 3) + chooseMain];
                panelAnim[(chooseMain / 3) + chooseMain] = panelAnim[(chooseMain / 3) + chooseMain + 1];
                panelAnim[(chooseMain / 3) + chooseMain + 1] = panelAnim[(chooseMain / 3) + chooseMain + 5];
                panelAnim[(chooseMain / 3) + chooseMain + 5] = panelAnim[(chooseMain / 3) + chooseMain + 4];
                panelAnim[(chooseMain / 3) + chooseMain + 4] = tmpAnim;

                panelMove[0] = false;
            }
            else if (panelMove[1])  //時計周り
            {
                //パネルの回転アニメーション
                panelAnim[(chooseMain / 3) + chooseMain].animFlg[0] = true; //right
                panelAnim[(chooseMain / 3) + chooseMain + 1].animFlg[1] = true; //down
                panelAnim[(chooseMain / 3) + chooseMain + 5].animFlg[2] = true; //up
                panelAnim[(chooseMain / 3) + chooseMain + 4].animFlg[3] = true; //left

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

                //オブジェクト入れ替え
                tmpObj = sideSphere[(chooseMain / 3) + chooseMain];
                sideSphere[(chooseMain / 3) + chooseMain] = sideSphere[(chooseMain / 3) + chooseMain + 4];
                sideSphere[(chooseMain / 3) + chooseMain + 4] = sideSphere[(chooseMain / 3) + chooseMain + 5];
                sideSphere[(chooseMain / 3) + chooseMain + 5] = sideSphere[(chooseMain / 3) + chooseMain + 1];
                sideSphere[(chooseMain / 3) + chooseMain + 1] = tmpObj;

                ////色の入れ替え
                //tmpSideColor = sideSphereColor[(chooseMain / 3) + chooseMain];
                //sideSphereColor[(chooseMain / 3) + chooseMain] = sideSphereColor[(chooseMain / 3) + chooseMain + 4];
                //sideSphereColor[(chooseMain / 3) + chooseMain + 4] = sideSphereColor[(chooseMain / 3) + chooseMain + 5];
                //sideSphereColor[(chooseMain / 3) + chooseMain + 5] = sideSphereColor[(chooseMain / 3) + chooseMain + 1];
                //sideSphereColor[(chooseMain / 3) + chooseMain + 1] = tmpSideColor;

                ////スクリプト入れ替え
                tmpAnim = panelAnim[(chooseMain / 3) + chooseMain];
                panelAnim[(chooseMain / 3) + chooseMain] = panelAnim[(chooseMain / 3) + chooseMain + 4];
                panelAnim[(chooseMain / 3) + chooseMain + 4] = panelAnim[(chooseMain / 3) + chooseMain + 5];
                panelAnim[(chooseMain / 3) + chooseMain + 5] = panelAnim[(chooseMain / 3) + chooseMain + 1];
                panelAnim[(chooseMain / 3) + chooseMain + 1] = tmpAnim;

                panelMove[1] = false;
            }

            //panelAnim[0].change = false;
            //ColorChange();   //パネルの色変更
            changeTime = 0.1f;
        }
    }
}
