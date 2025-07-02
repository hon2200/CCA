using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//这个类里面描述玩家的模版
//Player类将PlayerDefine和Player的其他功能组合在一块，完成Player类的功能
public class PlayerDefine
{
    public string ID;
    public int MaxHP;
    public PlayerType Type;
    public PlayerResource iniResource;
    public PlayerDefine(string ID, PlayerType playerType,int maxHP,PlayerResource playerResource)
    {
        this.Type = playerType;
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