using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinCounter : MonoBehaviour
{
    public static CoinCounter instance;
    
    public TMP_Text coinText;
    public int coinCount = 0;
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        coinText.text = coinCount.ToString();
    }
    
    public void AddCoin(int numberOfCoins)
    {
        coinCount += numberOfCoins;
        coinText.text = "COINS: " + coinCount.ToString();
    }
}
