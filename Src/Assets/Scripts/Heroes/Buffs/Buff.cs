using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Buff
{
    public string ID;
    public int Duration;   // 持续回合数（-1 永久）
    public int Value;      //层数
    public bool IsDebuff;

    public Buff(string id, int duration, int value = 0, bool isDebuff = false)
    {
        ID = id;
        Duration = duration;
        Value = value;
        IsDebuff = isDebuff;
    }

    //暂时删掉OnTurnStart，因为咱们的结算还是要放在结算阶段
    public virtual void OnResulution()
    {
    }
    public bool Fade()
    {
        Duration--;
        if (Duration <= 0)
            return true;
        return false;
    }
}


public class BuffManager
{
    private List<Buff> activeBuffs = new List<Buff>();

    public void Resolute()
    {
        var toRemove = new List<Buff>();
        foreach (var buff in activeBuffs)
        {
            buff.OnResulution();
            if (buff.Fade())
                toRemove.Add(buff);
        }

        foreach (var buff in toRemove)
            activeBuffs.Remove(buff);
    }
    public void Add(Buff buff)
    {
        var existing = activeBuffs.Find(b => b.ID == buff.ID);
        if (existing != null)
        {
            existing.Duration = Math.Max(existing.Duration, buff.Duration);
            existing.Value = Math.Max(existing.Value, buff.Value);
        }
        else
        {
            activeBuffs.Add(buff);
            Debug.Log($"✨ 添加 Buff：{buff.ID} ({buff.Duration} 回合)");
        }
    }

    public void Remove(string id)
    {
        activeBuffs.RemoveAll(b => b.ID == id);
        Debug.Log($"💨 移除 Buff：{id}");
    }

    public bool Has(string id) => activeBuffs.Exists(b => b.ID == id);

    public void Save() { /* 可序列化保存 */ }
    public void Load() { /* 可反序列化加载 */ }
}