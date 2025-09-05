using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//英雄的定义
//ID就是Table中的英雄的ID
public class HeroDefine
{
    public string ID;
    public string Name;
    public string Description;
    public int MaxHP;
    public List<string> SkillIDList;
    public HeroDefine(string ID, int maxHP)
    {
        this.ID = ID;
        this.MaxHP = maxHP;
    }
}

public enum PlayerType
{
    AI = 1,
    Human = 2,
}