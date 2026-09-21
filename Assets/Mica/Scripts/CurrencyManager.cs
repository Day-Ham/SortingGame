using TMPro;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    [Header("Currency")]
    public int coin;
    public int mana;

    [Header("UI")]
    [SerializeField] private TMP_Text coinTxt;
    [SerializeField] private TMP_Text manaTxt;

    private void Start()
    {
        UpdateUI();
    }

    public void ChangeCoin(int value)
    {
        coin += value;
        UpdateUI();
    }

    public void ChangeMana(int value)
    {
        mana += value;
        UpdateUI();
    }

    private void UpdateUI()
    {
        coinTxt.text = coin.ToString("F0");
        manaTxt.text = mana.ToString("F0");
    }
}
