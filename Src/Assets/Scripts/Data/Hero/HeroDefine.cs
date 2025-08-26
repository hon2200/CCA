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
    public PlayerResource iniResource;
    public List<string> SkillIDList;
    public HeroDefine(string ID,int maxHP,PlayerResource playerResource)
    {
        this.ID = ID;
        this.MaxHP = maxHP;
        this.iniResource = playerResource;
    }
}

public struct PlayerResource
{
    public int Bullet;
    public int Sword;
    public int SwordinCD;
    public PlayerResource(int Bullet, int Sword, int SwordinCD)
    {
        this.Bullet = Bullet;
        this.Sword = Sword;
        this.SwordinCD = SwordinCD;
    }
}

public enum PlayerType
{
    AI = 1,
    Human = 2,
}