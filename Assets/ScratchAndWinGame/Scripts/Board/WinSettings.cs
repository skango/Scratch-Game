using UnityEngine;

[System.Serializable]
public class SectionPrice
{
    public int Price;
    public float probability;
}

[System.Serializable]
public enum AreaType
{
    Money,
    Bonus
}


[System.Serializable]
public class WinSettings
{
    public SectionPrice PricePreset;
    public AreaType type;
    public bool DidIWon;

    public WinSettings(SectionPrice _pricePreset, AreaType _type)
    {
        PricePreset = _pricePreset;
        type = _type;
        DidIWon = WinOrLoose(_pricePreset);
    }

    // Mwethod which calculates if ticket or ticket section is winning one
    private bool WinOrLoose(SectionPrice pricePreset)
    {
        float RandomNumber = Random.Range(0f, 1f);
        if (RandomNumber <= pricePreset.probability) return true;
        else return false;
    }
}
