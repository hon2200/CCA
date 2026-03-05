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

    public void Apply(Buff newBuff)
    {
        foreach (var buff in this)
        {
            if (buff.ID == newBuff.ID)
            {
                if (newBuff.ApplyTo(buff))
                {
                    if (IsObserving)
                        OnListChanged?.Invoke(this, "Apply");
                    Debug.Log($"施加了buff{newBuff.ID}\n");
                    return;
                }
                break; // same ID but did not merge (e.g. BuffOperator) — will add as new element below
            }
        }
        AddInternal(newBuff);
        Debug.Log($"施加了buff{newBuff.ID}\n");
    }

    /// <summary>Use Apply(Buff) to add or stack buffs. Calling Add(buff) forwards to Apply so behavior is correct.</summary>
    public new void Add(Buff item)
    {
        Apply(item);
    }

    /// <summary>Internal add path: set Owner, add to list with notification, run BurnMark.OnStacksApplied when applicable.</summary>
    private void AddInternal(Buff item)
    {
        if (item != null && BuffOwner != null)
            item.Owner = BuffOwner;
        base.Add(item, "Apply");
        if (item is BurnMark burn)
            burn.OnStacksApplied(burn.Value);
    }
}
