using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BuffManager : ObservableList<Buff>
{
    /// <summary>
    /// The player who owns this buff list. Set by the owning Player/PlayerStatus so that applied buffs get Owner set.
    /// </summary>
    public Player BuffOwner { get; set; }

    //施加Buff
    public void Apply(Buff newBuff)
    {
        bool hasBuff = false;
        foreach(var buff in this)
        {
            if (buff.ID == newBuff.ID)
            {
                hasBuff = true;
                buff.Value += newBuff.Value;
                if (IsObserving)
                    OnListChanged?.Invoke(this, "Apply");
            }
        }
        if(!hasBuff)
        {
            Add(newBuff);
        }
        Debug.Log($"施加了buff{newBuff.ID}\n");
    }

    public new void Add(Buff item)
    {
        if (item != null && BuffOwner != null)
            item.Owner = BuffOwner;
        base.Add(item);
    }
}
