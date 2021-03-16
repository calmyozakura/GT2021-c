using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Turn : MonoBehaviour
{
    Text turnText;

    int nowTurn = 0;

    private void Start()
    {
        turnText = GetComponent<Text>();
    }

    public void TurnCount()
    {
        nowTurn++;
        turnText.text = "" + nowTurn;
    }
}
