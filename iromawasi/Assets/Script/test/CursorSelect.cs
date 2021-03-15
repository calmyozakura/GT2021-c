using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSelect : MonoBehaviour
{
    //[SerializeField] GameObject selectMainImage; //現在選択しているメインパネルを表示

    public void SelectImageMove(int chooseMain)
    {
        GetComponent<RectTransform>().anchoredPosition
            = new Vector2(-2 + (2 * (chooseMain % 3)), 2 - (2 * (chooseMain / 3)));
    }
}
