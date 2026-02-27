using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicDefine
{
    public string ID;
    public string Name;
    public string Description;
    public Rarity Rarity;
    protected void Init()
    {
        
    }
}

public enum Rarity
{
    Common = 1,
    Rare = 2,
    Boss = 3
}
