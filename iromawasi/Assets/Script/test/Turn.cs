using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Turn : MonoBehaviour
{
    Text turnText;

    public int nowTurn = 0;

    private void Start()
    {
        turnText = GetComponent<Text>();
        turnText.text = "" + nowTurn;
    }

    public void TurnCount()
    {
        nowTurn--;
        turnText.text = "" + nowTurn;
    }
}
