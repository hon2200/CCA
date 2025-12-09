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
    public Buff(string id, int value, bool isDebuff)
    {
        ID = id;
        Value = value;
        IsDebuff = isDebuff;
    }
    public virtual void BeforeAction(Player thisPlayer) { }
    public virtual void BeforeResolution(Player thisPlayer) { }
    //在结算阶段的最后，会结算Buff
    public virtual void OnResulution(Player thisPlayer) { }
    public virtual void Fade(Player thisPlayer) { }
}