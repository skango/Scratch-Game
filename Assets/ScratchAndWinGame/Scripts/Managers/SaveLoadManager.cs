using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class for saving and loading game progress to player prefs
public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager instance;

    public int MoneyScore;
    public int BonusScore;

    public int MoneyOldScore;
    public int BonusOldScore;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void increaseValues(AreaType type, int value)
    {
        MoneyOldScore = MoneyScore;
        BonusOldScore = BonusScore;

        if (type == AreaType.Money) MoneyScore += value;
        if (type == AreaType.Bonus) BonusScore += value;
    }

    public void SaveDataToPlayerPrefs()
    {
        PlayerPrefs.SetInt("SavedMoney", MoneyScore);
        PlayerPrefs.SetInt("SavedBonus", BonusScore);
    }

    public void LoadDataFromPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("SavedMoney")) MoneyScore = PlayerPrefs.GetInt("SavedMoney");
        if (PlayerPrefs.HasKey("SavedBonus")) BonusScore = PlayerPrefs.GetInt("SavedBonus");
    }
}
