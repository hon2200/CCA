using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BuffManager : ObservableList<Buff>
{
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
            Add(newBuff);
        Debug.Log($"施加了buff{newBuff.ID}\n");
    }
}
