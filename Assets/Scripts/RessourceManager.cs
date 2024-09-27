using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class RessourceManager : MonoBehaviour
{
    public int money;
    public static event Action UIUpdate;

    [Space]
    [SerializeField] private TextMeshProUGUI moneyText;

    private void Start()
    {
        ChangeMoneyCount(0);
    }
    public bool CheckForMoney(int amount)
    {
        if (money - amount < 0) return false;
        else return true;
    }
    public void ChangeMoneyCount(int amount)
    {
        money += amount;
        moneyText.text = money.ToString();
        UIUpdate?.Invoke();
    }
}
