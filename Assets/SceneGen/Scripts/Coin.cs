using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int numberOfCoins;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GameObject().CompareTag("Player"))
        {
            Destroy(gameObject);;
            CoinCounter.instance.AddCoin(numberOfCoins);
        }
    }
}
