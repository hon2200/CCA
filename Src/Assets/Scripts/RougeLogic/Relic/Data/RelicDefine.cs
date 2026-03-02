using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RelicDefine
{
    public string ID { get; protected set; }
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public int Price { get; protected set; }
    public int PriceFloat { get; protected set; }
    public Rarity Rarity { get; protected set; }
    public List<int> counts { get; protected set; }

    protected RelicDefine(string id)
    {
        ID = id;
        Init();
    }

    protected virtual void Init()
    {
        counts = counts ?? new List<int>();
        if (RelicDatabaseOrigin.Instance?.RelicDictionary == null) return;
        if (!RelicDatabaseOrigin.Instance.RelicDictionary.TryGetValue(ID, out RelicDefineOrigin data) || data == null) return;
        ApplyDataFrom(data);
    }

    /// <summary>
    /// Applies data from a JSON-loaded RelicDefineOrigin into this instance.
    /// </summary>
    protected void ApplyDataFrom(RelicDefineOrigin data)
    {
        if (data == null) return;
        ID = data.ID ?? ID;
        Name = data.Name ?? ID;
        Description = data.Description;
        Rarity = data.Rarity;
        switch (Rarity)
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
            default:
                Price = 150;
                PriceFloat = 20;
                break;
        }
        counts = new List<int>();
    }

    public virtual void OnPickup() { }
}
