using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;

//此类存储了各个瞬时值，并起到单个玩家内部从move到instant的传递
//比原版少的功能：damagesource，人头收益结算（需要在大转盘上进行）
public class Instant : MonoBehaviour
{
    //需要知道这个玩家是谁
    public PlayerNumber playerNumber;
    //需要知道游戏有多少人,手动赋值
    public People people;
    //需要知道玩家进行了什么行动
    public Move move;
    //需要知道Status，但不改变它，只是启动status中的consume
    public Status status;
    //七个瞬时值
    public List<Charge> charge = new();//charge[行动序号],其值见Add
    public List<int> block = new(), ca = new();//block&ca[行动序号]，其值代表防御或反弹的类型
    public List<bool> doblock = new(), doca = new();//doblock&daca[防御类型]//[防御类型]//其值代表是否进行了该防御或反弹
    public List<Attack> attack = new();//attack[行动序号]其值见Attack 
    public List<int> provoke = new();//provoke[行动序号]，其值代表挑衅对象
    public bool comeon = new();//comeon，其值代表是否进行了过来
                               //两个增补瞬时值(注意清零)
    public bool ready = false;
    public void PerformAct()
    {
        for (int order = 0; order < move.move.Count; order++)
        {
            status.Consume(order);
            switch (move.catagory[order])
            {
                case 1:
                    ActCharge(order);
                    break;
                case 2:
                    ActShoot(order);
                    break;
                case 3:
                    ActStab(order);
                    break;
                case 4:
                    ActBlock(order);
                    break;
                case 5:
                    ActCounterattack(order);
                    break;
                case 6:
                    ActProvoke(order);
                    ActComeon(order);
                    break;
            }
        }
        ready = true;
    }//将move中的变量传递到各个瞬时时值中，
    //对于Player的第order个行动，调用move，赋值shoot、attack、ds
    //缺乏的结算：和平对自己的伤害and激光炮的伤害、
    public void ActShoot(int order)
    {
        int to = move.at[order];
        switch (move.move[order])
        {
            case 1://shoot
                attack.Add(new Attack(1, 4, to, 1, 0));
                break;
            case 2://double shoot
                attack.Add(new Attack(1, 4, to, 2, 0));
                break;
            case 3://lasergun
                attack.Add(new Attack(2, 4, to, 3, 0));
                break;
            case 4://lasercannon:damage在perform attack里面再赋值
                attack.Add(new Attack(0, 1, to, 4, 0));
                break;
            case 5://RPG
                attack.Add(new Attack(2, 6, to, 5, 0));
                break;
            case 6://double RPG
                attack.Add(new Attack(3, 6, to, 6, 0));
                break;
            case 7://Peace
                attack.Add(new Attack(3, 7, to, 7, 0));
                for (int i = 1; i <= people.initialPeople; i++)//和平的突出地位 
                {
                    attack.Add(new Attack(1, 7, to, 7, 0));
                }
                break;
            default:
                Debug.Log("shoot has other values?");
                break;
        }
    }
    //调用move，赋值block、doblock
    public void ActBlock(int order)
    {
        //注意doblock的长度从来没有变化过
        //doblock[0]若true，则没有进行任何反弹行动，这种写法会带来一些方便
        switch (move.move[order])
        {
            case 1: block.Add(1); doblock[1] = true; break;
            case 2: block.Add(2); doblock[2] = true; break;
            case 3: block.Add(3); doblock[3] = true; break;
            case 4: block.Add(4); doblock[4] = true; break;
        }
        //doblock[0]若为真，则代表没有进行任何防御行动，这样写能带来一些便利
        if (doblock[1] == false && doblock[2] == false && doblock[3] == false && doblock[4] == false)
            doblock[0] = true;
    }
    //调用move，赋值stab、attack
    //ds在这里没有被赋值
    public void ActStab(int order)
    {
        int to = move.at[order];
        switch (move.move[order])
        {
            case 1://stab
                attack.Add(new Attack(1, 2, to, 0, 1));
                break;
            case 2://double stab
                attack.Add(new Attack(1, 3, to, 0, 2));
                break;
            case 3://lightsaber stab
                attack.Add(new Attack(2, 2, to, 0, 3));
                break;
            case 4://lightsaber chop
                attack.Add(new Attack(2, 3, to, 0, 4));
                break;
            case 5://double chop
                attack.Add(new Attack(1, 3, to, 0, 5));
                break;
            case 6://ghost chop
                attack.Add(new Attack(1, 5, to, 0, 6));
                break;
            case 7://nock-end chop
                attack.Add(new Attack(2, 5, to, 0, 7));
                break;
            default:
                Debug.Log("Stab has other values?");
                break;
        }
    }
    //调用move，赋值add
    public void ActCharge(int order)
    {
        int to = move.at[order];
        switch (move.move[order])
        {
            case 1:
                charge.Add(new Charge(1, 1, playerNumber.number));
                break;
            case 2:
                charge.Add(new Charge(2, 1, playerNumber.number));
                break;
            default:
                Debug.Log("Add has other values?");
                break;
        }
    }
    //调用move，赋值ca
    public void ActCounterattack(int order)
    {
        switch (move.move[order])
        {
            case 1: ca.Add(1); doca[1] = true; break;
            case 2: ca.Add(2); doca[2] = true; break;
            case 3: ca.Add(3); doca[3] = true; break;
            case 4: ca.Add(4); doca[4] = true; break;
            default:
                Debug.Log("ca has other values?");
                break;
        }
        //doca[0]若true，则没有进行任何反弹行动，这种写法会带来一些方便
        if (doca[1] == false && doca[2] == false && doca[3] == false && doca[4] == false)
            doca[0] = true;
    }
    //调用move，赋值provoke
    public void ActProvoke(int order)
    {
        int to = move.at[order];
        if (move.move[order] == 1)
            provoke.Add(to);
    }
    //调用move，赋值comeon
    public void ActComeon(int order)
    {
        if (move.move[order] == 2)
            comeon = true;
    }
    public void ClearInstants()
    {
        charge.Clear();
        block.Clear();
        ca.Clear();
        for (int i = 0; i < doblock.Count; i++) 
        {
            doblock[i] = false;
        }
        for(int i=0;i<doca.Count;i++)
        {
            doca[i] = false;
        }
        attack.Clear();
        provoke.Clear();
        comeon = false;
    }
}
//补给瞬时值的结构体
[System.Serializable]//使得这个class能够在inspector面板看到
public class Charge
{
    public int catagory;//类型，1=子弹，2=剑
    public int number;//一次补给的数量
    public int target;
    public Charge(int catagory,int number,int target)
    {
        this.catagory = catagory;
        this.number = number;
        this.target = target;
    }
};
//attack瞬时值的结构体
//存储攻击有关的参数
//存储攻击力和伤害
[System.Serializable]//使得这个class能够在inspector面板看到
public class Attack
{
    public int damage;//伤害
    public int level;//等级//这里有一个默认，在attack level==0是玩家没有进行任何攻击行动的充要条件（如在挑衅的判定中）
    public int target;//攻击目标
    public int shoot;//使用了哪种类型的子弹攻击
    public int stab;//使用了哪种类型的刀剑攻击
    public Attack(int damage, int level, int target, int shoot, int stab)
    {
        this.damage = damage;
        this.level = level;
        this.target = target;
        this.shoot = shoot;
        this.stab = stab;   
    }
};



