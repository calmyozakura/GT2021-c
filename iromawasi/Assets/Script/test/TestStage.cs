using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestStage : MonoBehaviour
{
    [SerializeField] CursorSelect cursorSelectCS;
    [SerializeField] Turn TurnCS;
    [SerializeField] Timer TimerCS;
    [SerializeField] Combo ComboCS;

    const int mainPanel = 9;    //メインパネルの数
    const int sidePanel = 16;    //サイドパネルの数

    int[] mainNumber = new int[mainPanel];     //3*3のナンバー
    int[] sideNumber = new int[sidePanel];     //4*4のナンバー
    int tmpNumber;          //数字入れ替え時の一時保存
    int[] bonusLevel = new int[sidePanel]; //サイドパネルのボーナス確認 (0=なし,1=あり)
    bool[] bonusFlg = new bool[sidePanel]; //このターン既にボーナスパネルになったかどうか
    int tmpBonus;         //ボーナス入れ替え時の一時保存
    int judgNum = 0;  //和を計算する配列
    public static int score = 0;      //スコア

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
    [SerializeField] GameObject[] mainSphere;
    Color[] mainSphereColor = new Color[mainPanel];  //マテリアル色を変えるための仮入れ
    Color tmpMainColor;
    Renderer[] mainSphereRenderer = new Renderer[mainPanel];    //実際にオブジェクトの色を変更する

    int[] mainColorNumber = { 4, 32, 128, 512 };    //メイン色の配列(赤、青、緑、黄)
    int[] sideColorNumber = { 1, 8, 32, 128 };     //サイド色の配列(赤、青、緑、黄)
    int[] rainbowNumber = { 0, 1, 4, 5 };           //虹衛星を出すときに使う
    bool rainbow;
    int[] rainbowRand = new int[mainPanel]; //虹衛星をランダムに配置するための変数
    int rainbowTarget = 0;
    //[SerializeField] GameObject selectMainImage; //現在選択しているメインパネルを表示

    [SerializeField] GameObject Score;  //スコアのテキストオブジェクト
    Text scoreText;
    //[SerializeField] GameObject Timer;  //制限時間のテキストオブジェクト
    //Text timerText;
    //float timeCount = 60.0f;            //制限時間
    //[SerializeField] GameObject Turn;   //ターンのテキストオブジェクト
    //Text turnText;
    //int nowTurn = 0;

    float alpha_Time = 0f;   //点滅させる時間
    float alpha_Sin;    //消すときに点滅させる
    bool alpha_Flg;
    int check = 0; //中身を順にみる変数

    bool[] flgCheck = new bool[mainPanel + 1];  //ポイントになった箇所を記憶,5はnull
    int mainColorNum = 0;               //全パネルが同じ色になったら色を変える

    bool[] panelMove = new bool[2]; //右か左にパネル移動させるフラグ
                                    //float changeTime = 0.1f;

    public Material[] _material;           // 割り当てるマテリアル.
    public Texture NormalmapTexture;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < mainPanel; i++)
        {
            mainNumber[i] = mainColorNumber[Random.Range(0, 2)];
            mainSphereColor[i] = mainSphere[i].GetComponent<Renderer>().material.color;
        }

        for (int i = 0; i < sidePanel; i++)
        {
            sideNumber[i] = sideColorNumber[Random.Range(0, 2)];
            sideSphereColor[i] = sideSphere[i].GetComponent<Renderer>().material.color;
            //panelAnim[i] = obj[i].GetComponent<PanelAnim>();一時消し
        }

        scoreText = Score.GetComponent<Text>();
        //timerText = Timer.GetComponent<Text>();
        //turnText = Turn.GetComponent<Text>();

        ColorChange();   //パネルの色変更
        TurnCS.nowTurn = 5; //ターン数の指定
    }

    // Update is called once per frame
    void Update()
    {
        if (!alpha_Flg)
        {
            if (!panelMove[0] && !panelMove[1]) PanelOperation();   //パネルの操作
            else if (panelMove[0] || panelMove[1]) PanelMove();        //パネルのアニメーション
            //PointCheck();   //盤面が揃ったか見る 揃ったらすぐ変わる
            ColorChange();   //パネルの色変更
            TimerCS.TimerCount();       //制限時間のカウントと表示
            TurnEnd();      //ターン終了時の処理

            //SelectImageMove();  //現在選んでいるパネルの可視化 ここで呼ぶ
            cursorSelectCS.SelectImageMove(chooseMain);

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

    //***
    void alpha()
    {
        if (flgCheck[check] && check <= (mainPanel - 1))   //0~9で条件を満たしたら
        {

            if (alpha_Time < 0.6f)    //条件を満たしたパネルの点滅
            {
                alpha_Time += Time.deltaTime * 2;    //制限時間のカウントダウン
                alpha_Sin = (alpha_Time * 6) + 5;
                //alpha_Sin = Mathf.Sin(Time.time) / 2 + 0.5f;

                //mainSphereColor[check].a = alpha_Sin; //透明度を下げる
                //sideSphereColor[(check / 3) + check].a = alpha_Sin; //透明度を下げる
                //sideSphereColor[(check / 3) + check + 1].a = alpha_Sin; //透明度を下げる
                //sideSphereColor[(check / 3) + check + 4].a = alpha_Sin; //透明度を下げる
                //sideSphereColor[(check / 3) + check + 5].a = alpha_Sin; //透明度を下げる

                //mainSphere[check].GetComponent<Renderer>().material.color = sideSphereColor[check];
                //sideSphere[(check / 3) + check].GetComponent<Renderer>().material.color = sideSphereColor[(check / 3) + check];
                //sideSphere[(check / 3) + check + 1].GetComponent<Renderer>().material.color = sideSphereColor[(check / 3) + check + 1];
                //sideSphere[(check / 3) + check + 4].GetComponent<Renderer>().material.color = sideSphereColor[(check / 3) + check + 4];
                //sideSphere[(check / 3) + check + 5].GetComponent<Renderer>().material.color = sideSphereColor[(check / 3) + check + 5];

                mainSphere[check].GetComponent<Renderer>().material.SetFloat("_AtmosphereDensity", alpha_Sin);
                sideSphere[(check / 3) + check].GetComponent<Renderer>().material.SetFloat("_AtmosphereDensity", alpha_Sin);
                sideSphere[(check / 3) + check + 1].GetComponent<Renderer>().material.SetFloat("_AtmosphereDensity", alpha_Sin);
                sideSphere[(check / 3) + check + 4].GetComponent<Renderer>().material.SetFloat("_AtmosphereDensity", alpha_Sin);
                sideSphere[(check / 3) + check + 5].GetComponent<Renderer>().material.SetFloat("_AtmosphereDensity", alpha_Sin);
            }
            else if (alpha_Time >= 0.6f)
            {
                alpha_Time = 0;
                //flgCheck[check] = false;
                //ColorChange();

                ScoreAdd();

                check += 1;
            }
        }
        else if (check > (mainPanel - 1))    //最後に盤面を変える
        {
            if (alpha_Sin <= 8)    //半透明から透明へ
            {
                alpha_Sin += Time.deltaTime * 2;

                for (int i = 0; i < mainPanel; i++)
                {
                    if (flgCheck[i])
                    {
                        //mainSphereColor[i].a = alpha_Sin; //透明度を下げる
                        //sideSphereColor[(i / 3) + i].a = alpha_Sin; //透明度を下げる
                        //sideSphereColor[(i / 3) + i + 1].a = alpha_Sin; //透明度を下げる
                        //sideSphereColor[(i / 3) + i + 4].a = alpha_Sin; //透明度を下げる
                        //sideSphereColor[(i / 3) + i + 5].a = alpha_Sin; //透明度を下げる

                        //mainSphere[i].GetComponent<Renderer>().material.color = sideSphereColor[i];
                        //sideSphere[(i / 3) + i].GetComponent<Renderer>().material.color = sideSphereColor[(i / 3) + i];
                        //sideSphere[(i / 3) + i + 1].GetComponent<Renderer>().material.color = sideSphereColor[(i / 3) + i + 1];
                        //sideSphere[(i / 3) + i + 4].GetComponent<Renderer>().material.color = sideSphereColor[(i / 3) + i + 4];
                        //sideSphere[(i / 3) + i + 5].GetComponent<Renderer>().material.color = sideSphereColor[(i / 3) + i + 5];

                        mainSphere[i].GetComponent<Renderer>().material.SetFloat("_AtmosphereDensity", 8);
                        sideSphere[(i / 3) + i].GetComponent<Renderer>().material.SetFloat("_AtmosphereDensity", 8);
                        sideSphere[(i / 3) + i + 1].GetComponent<Renderer>().material.SetFloat("_AtmosphereDensity", 8);
                        sideSphere[(i / 3) + i + 4].GetComponent<Renderer>().material.SetFloat("_AtmosphereDensity", 8);
                        sideSphere[(i / 3) + i + 5].GetComponent<Renderer>().material.SetFloat("_AtmosphereDensity", 8);
                    }
                }
            }
            else
            {

                for (int i = 0; i < mainPanel; i++)
                {
                    if (flgCheck[i])
                    {

                        //ランダムな数値にいれかえ
                        mainNumber[i] = mainColorNumber[Random.Range(0, 2)];
                        //mainNumber[i] = mainColorNumber[0];
                        sideNumber[(i / 3) + i] = sideColorNumber[Random.Range(0, 2)];
                        sideNumber[(i / 3) + i + 1] = sideColorNumber[Random.Range(0, 2)];
                        sideNumber[(i / 3) + i + 5] = sideColorNumber[Random.Range(0, 2)];
                        sideNumber[(i / 3) + i + 4] = sideColorNumber[Random.Range(0, 2)];

                        rainbowRand[rainbowTarget] = i; //条件を満たした惑星の位置を把握しておく
                        rainbowTarget += 1;
                    }
                    mainColorNum += mainNumber[i];  //[0]^[3]合計を得る
                }

                if (ComboCS.comboCount >= 3 && !rainbow) //条件を満たした惑星のどこかに3コンボ以上で虹惑星を１つだす
                {
                    rainbow = true;
                    sideNumber[(rainbowRand[Random.Range(0, ComboCS.comboCount)] / 3) 
                        + rainbowRand[Random.Range(0, ComboCS.comboCount)] + rainbowNumber[Random.Range(0, 4)]] = sideColorNumber[2];
                }

                for (int j = 0; j < 4; j++)  //[0]^[9]の合計と色*4を見る { 4, 32, 128, 512}
                {
                    while (mainColorNum == (mainColorNumber[j] * mainPanel))  //9色同じだったら処理を繰り返す
                    {
                        mainColorNum = 0;   //一度numを0にし
                        for (int f = 0; f < mainPanel; f++)
                        {
                            if (flgCheck[f]) mainNumber[f] = mainColorNumber[Random.Range(0, 2)]; //消したマスをランダムな色に変えて
                            mainColorNum += mainNumber[f];       //もう一度[0]^[9]の合計を得る
                        }
                    }
                }
                Bonus();    //ボーナスパネル設定
                for (int f = 0; f < mainPanel; f++) flgCheck[f] = false; //念のため別のforでfalseにする
                mainColorNum = 0;
                ColorChange();   //パネルの色変更
                check = 0;
                rainbow = false;
                alpha_Flg = false;
                rainbowTarget = 0;
            }
        }
        else check += 1;
    }
    //***
    void PanelOperation()
    {
        //パネル反時計回り
        if (Input.GetButtonDown("LB"))
        {
            panelMove[0] = true;
            if (!TimerCS.countStart) TimerCS.countStart = true;
        }
        //パネル時計回り
        else if (Input.GetButtonDown("RB"))
        {
            panelMove[1] = true;
            if (!TimerCS.countStart) TimerCS.countStart = true;
        }

        //十字キーのパネル選択
        if (0 > Input.GetAxis("ClossVertical") && !ClossTilt)    //↓入力時
        {
            if (chooseMain >= 6) chooseMain -= 6;
            else chooseMain += 3;
            ClossTilt = true;
        }
        if (0 < Input.GetAxis("ClossVertical") && !ClossTilt)  //↑入力時
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
        if (0 < Input.GetAxis("ClossHorizontal") && !ClossTilt)    //→入力時
        {
            if (chooseMain % 3 == 2) chooseMain -= 2;
            else chooseMain += 1;
            ClossTilt = true;
        }

        if (0 == Input.GetAxis("ClossHorizontal") && (0 == Input.GetAxis("ClossVertical"))) ClossTilt = false;
    }
    //***
    void PointCheck()
    {

        for (int i = 0; i < mainPanel; i++)
        {

            //judgNum += sideNumber[(i / 3) + i];
            //judgNum += sideNumber[(i / 3) + i + 1];
            //judgNum += sideNumber[(i / 3) + i + 5];
            //judgNum += sideNumber[(i / 3) + i + 4];

            judgNum = mainNumber[i];

            if(judgNum == sideNumber[(i / 3) + i] * 4 || sideNumber[(i / 3) + i] == 32)
                if(judgNum == sideNumber[(i / 3) + i + 1] * 4 || sideNumber[(i / 3) + i + 1] == 32)
                    if (judgNum == sideNumber[(i / 3) + i + 5] * 4 || sideNumber[(i / 3) + i + 5] == 32)
                        if (judgNum == sideNumber[(i / 3) + i + 4] * 4 || sideNumber[(i / 3) + i + 4] == 32) //色を満たした
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

            //if (judgNum == mainNumber[i])   //色を満たした
            //{
            //    flgCheck[i] = true;

            //    //ボーナスフラグon
            //    bonusFlg[(i / 3) + i] = true;
            //    bonusFlg[(i / 3) + i + 1] = true;
            //    bonusFlg[(i / 3) + i + 5] = true;
            //    bonusFlg[(i / 3) + i + 4] = true;

            //    alpha_Flg = true;
            //}
        }
    }

    //***
    public void ColorChange()
    {
        for (int i = 0; i < mainPanel; i++)
        {
            switch (mainNumber[i])
            {
                case 4:
                    //mainSphereColor[i] = Color.red;
                    //mainSphere[i].GetComponent<Renderer>().material.color = mainSphereColor[i];
                    mainSphere[i].GetComponent<Renderer>().material = _material[0];
                    break;
                case 32:
                    //mainSphereColor[i] = Color.blue;
                    //mainSphere[i].GetComponent<Renderer>().material.color = mainSphereColor[i];
                    mainSphere[i].GetComponent<Renderer>().material = _material[1];
                    break;
                case 128:
                    mainSphereColor[i] = Color.yellow;
                    mainSphere[i].GetComponent<Renderer>().material.color = mainSphereColor[i];
                    break;
                case 512:
                    mainSphereColor[i] = Color.green;
                    mainSphere[i].GetComponent<Renderer>().material.color = mainSphereColor[i];
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
                    //sideSphereColor[i] = Color.red;
                    //sideSphere[i].GetComponent<Renderer>().material.color = sideSphereColor[i];
                    sideSphere[i].GetComponent<Renderer>().material = _material[0];
                    break;
                case 8:
                    //sideSphereColor[i] = Color.blue;
                    //sideSphere[i].GetComponent<Renderer>().material.color = sideSphereColor[i];
                    sideSphere[i].GetComponent<Renderer>().material = _material[1];
                    break;
                case 32:
                    //sideSphereColor[i] = Color.yellow;
                    //sideSphere[i].GetComponent<Renderer>().material.color = sideSphereColor[i];
                    sideSphere[i].GetComponent<Renderer>().material = _material[2];
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
    //***
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
    //***
    void PanelMove()    //アニメーション終了時に呼ぶ
    {
        //if (changeTime > 0) changeTime -= Time.deltaTime;
        //else if (changeTime <= 0)
        //{
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
                panelAnim[(chooseMain / 3) + chooseMain].animFlg[4] = true; //right2
                panelAnim[(chooseMain / 3) + chooseMain + 1].animFlg[5] = true; //down2
                panelAnim[(chooseMain / 3) + chooseMain + 5].animFlg[6] = true; //up2
                panelAnim[(chooseMain / 3) + chooseMain + 4].animFlg[7] = true; //left2

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

                ////スクリプト入れ替え
                tmpAnim = panelAnim[(chooseMain / 3) + chooseMain];
                panelAnim[(chooseMain / 3) + chooseMain] = panelAnim[(chooseMain / 3) + chooseMain + 4];
                panelAnim[(chooseMain / 3) + chooseMain + 4] = panelAnim[(chooseMain / 3) + chooseMain + 5];
                panelAnim[(chooseMain / 3) + chooseMain + 5] = panelAnim[(chooseMain / 3) + chooseMain + 1];
                panelAnim[(chooseMain / 3) + chooseMain + 1] = tmpAnim;

                ////色の入れ替え
                //tmpSideColor = sideSphereColor[(chooseMain / 3) + chooseMain];
                //sideSphereColor[(chooseMain / 3) + chooseMain] = sideSphereColor[(chooseMain / 3) + chooseMain + 4];
                //sideSphereColor[(chooseMain / 3) + chooseMain + 4] = sideSphereColor[(chooseMain / 3) + chooseMain + 5];
                //sideSphereColor[(chooseMain / 3) + chooseMain + 5] = sideSphereColor[(chooseMain / 3) + chooseMain + 1];
                //sideSphereColor[(chooseMain / 3) + chooseMain + 1] = tmpSideColor;

                panelMove[1] = false;
        //    }
        //    changeTime = 0.1f;
        }
    }

    void ScoreAdd()
    {
        //スコア+100と各パネルのボーナス分スコア+50
        //score += 100 + (50 * (bonusLevel[(check / 3) + check] + bonusLevel[(check / 3) + check + 1]
        //    + bonusLevel[(check / 3) + check + 5] + bonusLevel[(check / 3) + check + 4]));

        //スコア100と1コンボ50
        score += 100 + (50 * ComboCS.comboCount);

        scoreText.text = "" + score;

        ComboCS.comboTime = 5.0f;
        ComboCS.comboCount += 1;
    }

    void TurnEnd()
    {
        if (TurnCS.nowTurn > 0)
        {
            if (Input.GetButtonDown("X") || TimerCS.timeOut) //Xか制限時間でターン終了
            {
                if (!alpha_Flg) PointCheck();
                TimerCS.timeCount = 30.0f;
                TimerCS.timeOut = false;
                TimerCS.countStart = false;
                ComboCS.comboCount = 0; //コンボカウントリセット
                TurnCS.TurnCount();        //経過ターンの更新表示
            }
        }
        else
        {
            if (TimerCS.timeCount > 0) TimerCS.timeCount = 0f;
            //ゲーム終了かリトライ
            SceneManager.LoadScene("Result");
        }
    }
}
