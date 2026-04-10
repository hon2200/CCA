using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HeroDefine
{
    public string ID;
    public string Name;
    public string Description;
    public int MaxHP;
    public List<string> SkillIDList;
    public bool Enable;
    
    // 默认构造函数供JSON反序列化使用
    public HeroDefine() { }
    
    public HeroDefine(string ID, int maxHP)
    {
        this.ID = ID;
        this.MaxHP = maxHP;
    }
    
    // 重写ToString方法便于调试
    public override string ToString()
    {
        return $"HeroDefine[ID={ID}, Name={Name}, HP={MaxHP}, Skills={SkillIDList?.Count ?? 0}]";
    }
}

public enum PlayerType
{
    AI = 1,
    Human = 2,
}