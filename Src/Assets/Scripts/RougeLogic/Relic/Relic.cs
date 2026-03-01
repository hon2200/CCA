using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Relic
{
    public string ID;
    public string Name;
    public string Description;
    public int Price;
    public int PriceFloat;
    public Rarity Rarity;
    public List<int> counts;
    protected Relic(string id)
    {
        ID = id;
        Init();                 // ¡û ALWAYS init at creation
    }
    protected void Init()
    {
        RelicDatabase.Instance.RelicDictionary.TryGetValue(ID, out var relicDefine);
        ID= relicDefine.ID;
        Name= relicDefine.Name;
        Description= relicDefine.Description;
        Rarity = relicDefine.Rarity;
        switch(Rarity)
        {
            case Rarity.Common:
                Price = 150;
                PriceFloat = 20;
                break;
            case Rarity.Rare:
                Price = 200;
                PriceFloat = 30;
                break;
            case Rarity.Boss:
                Price = 300;
                PriceFloat = 50;
                break;
        }
        counts = new();
    }
    public virtual void OnPickup() { }
}
