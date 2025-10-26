using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Buff
{
    public string ID;
    public int Duration;   // 持续回合数（-1 永久）
    public int Value;
    public bool IsDebuff;

    public Buff(string id, int duration, int value = 0, bool isDebuff = false)
    {
        ID = id;
        Duration = duration;
        Value = value;
        IsDebuff = isDebuff;
    }

    public void OnTurnStart(Player owner)
    {
        if (ID == "poison")
        {
            owner.status.HP.Damage(Value, null, owner, null);
            Debug.Log($"☠️ {owner.ID_inGame} 中毒受到 {Value} 点伤害");
        }

        if (Duration > 0) Duration--;
    }

    public bool IsExpired() => Duration == 0;
}


public class BuffManager
{
    private List<Buff> activeBuffs = new List<Buff>();

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

    public void OnTurnStart(Player owner)
    {
        foreach (var buff in activeBuffs.ToList())
        {
            buff.OnTurnStart(owner);
            if (buff.IsExpired())
            {
                Debug.Log($"⌛ Buff {buff.ID} 到期");
                activeBuffs.Remove(buff);
            }
        }
    }

    public void Save() { /* 可序列化保存 */ }
    public void Load() { /* 可反序列化加载 */ }
}