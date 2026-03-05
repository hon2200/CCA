using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Buff
{
    public string ID { get; set; }
    public int Value { get; set; }      //层数
    public bool IsDebuff { get; set; }
    public Player Owner { get; set; }
    public Buff(string id, int value, bool isDebuff, Player owner)
    {
        ID = id;
        Value = value;
        IsDebuff = isDebuff;
        Owner = owner;
    }
    //在结算阶段的最后，会结算Buff
    public virtual void Fade() { }

    /// <summary>
    /// Called by BuffManager when a buff with the same ID already exists.
    /// Return true to merge (default: add Value to existing). Return false to add this buff as a new list element.
    /// </summary>
    public virtual bool ApplyTo(Buff existing)
    {
        existing.Value += this.Value;
        return true;
    }
}