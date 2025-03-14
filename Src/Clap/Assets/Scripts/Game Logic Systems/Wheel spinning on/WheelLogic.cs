using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//还没有写和平
public class WheelLogic : MonoBehaviour
{
    //需要知道一共多少人，手动赋值
    public People People;
    //需要知道每个玩家，手动赋值
    //注意下标为玩家编号，所以长度为Player的数量+1
    public List<GameObject> Player;
    //需要知道每个玩家的状态和瞬时值来完成交互，这里在Awake里进行
    //需要知道玩家的move，以便在新的一回合来临之前更新
    public List<Move> move;
    public List<Status> status;
    public List<Instant> instant;
    public List<InteractInstant> interactInstant;
    
    public void Awake()
    {
        //初始化Status和Instant和InteractInstant
        //这里是因为三个数组的第一个元素并不想赋值
        move.Add(null);
        instant.Add(null);
        status.Add(null);
        interactInstant.Add(null);
        for (int people = 1; people <= People.initialPeople; people++)
        {
            move.Add(Player[people].GetComponent<Move>());
            instant.Add(Player[people].GetComponent<Instant>());
            status.Add(Player[people].GetComponent<Status>());
            interactInstant.Add(Player[people].GetComponent<InteractInstant>());
        }
    }
    //这个在我看来是一件非常低效的事情//监听各位玩家是否准备好了
    public void Update()
    {
        int readyNumber = 0;
        for (int people = 1; people <= People.initialPeople; people++)
        {
            if (instant[people].ready)
                readyNumber++;
        }
        if(readyNumber == People.alivePeople)
        {
            Spinning();
            //轮子转一周之后，所有玩家重新进入待命状态。
            for(int people= 1; people <= People.initialPeople; people++)
            {
                instant[people].ready = false;
                instant[people].ClearInstants();
                move[people].ClearMoves();
            }
        }
    }
    public void Spinning()
    {
        PerformComeon();
        PerformCharge();
        CalculateMaxAttackLevel();
        PerformAttack();
    }
    //先进行过来的攻击目标改变
    public void PerformComeon()
    {
        List<int> comeonPeople = new();
        for (int people = 1; people <= People.initialPeople; people++) 
        {
            if (instant[people].comeon==true)
            {
                comeonPeople.Add(people);
            }
        }
        if (comeonPeople.Count > 0)  
        {
            for(int people = 1; people <= People.initialPeople; people++)
            {
                //先进行有关攻击的目标改变
                int howManyAttack = instant[people].attack.Count;
                for (int order = 0; order < howManyAttack; order++)
                {
                    for (int i = 0; i < comeonPeople.Count; i++)
                    {
                        //过来带来的行动分裂
                        instant[people].attack.Add(instant[people].attack[order]);
                        //过来带来的目标改变，把刚加入的行动目标改成过来的那个人
                        instant[people].attack[instant[people].attack.Count - 1].target = comeonPeople[i];
                    }
                }
                //将原本的行动删除
                for (int order = 0; order < howManyAttack; order++) 
                {
                    instant[people].attack.RemoveAt(order);
                }
                //再用同样的方法
                int howManyCharge = instant[people].charge.Count;
                for(int order = 0; order < howManyCharge; order++)
                {
                    for (int i = 0; i < comeonPeople.Count; i++)
                    {
                        //过来带来的行动分裂
                        instant[people].charge.Add(instant[people].charge[order]);
                        //过来带来的目标改变，把刚加入的行动目标改成过来的那个人
                        instant[people].charge[instant[people].charge.Count - 1].target = comeonPeople[i];
                    }
                }
                //将原本的行动删除
                for (int order = 0; order < howManyAttack; order++)
                {
                    instant[people].charge.RemoveAt(order);
                }
            }
        }
    }
    //玩家开始进行补给活动
    public void PerformCharge()
    {
        for (int people = 1; people <= People.initialPeople; people++)
        {
            for (int order = 0; order < instant[people].charge.Count; order++)
            {
                //补给目标的资源数+=补给行动者补给的数量，分子弹和剑
                if (instant[people].charge[order].catagory == 1)
                    status[instant[people].charge[order].target].bullet += instant[people].charge[order].number;
                if (instant[people].charge[order].catagory == 2)
                    status[instant[people].charge[order].target].sword += instant[people].charge[order].number;
            }
        }
    }
    //赋值maxAttackLevel
    public void CalculateMaxAttackLevel()
    {
        for (int people = 1; people <= People.initialPeople; people++)
        {
            for (int order = 0; order < instant[people].attack.Count; order++)
            {
                //如果新攻击的攻击力比原攻击高
                if (instant[people].attack[order].level > interactInstant[people].maxAttackLevel[instant[people].attack[order].target])
                {
                    interactInstant[people].maxAttackLevel[instant[people].attack[order].target] = instant[people].attack[order].level;
                }
            }
        }
    }
    //开始攻击！
    public void PerformAttack()
    {
        for(int from= 1; from <= People.initialPeople; from++)
        {
            for (int order = 0; order < instant[from].attack.Count; order++)
            {
                int to = instant[from].attack[order].target;
                //攻击力判定
                if (interactInstant[from].maxAttackLevel[to] > interactInstant[to].maxAttackLevel[from])
                {
                    switch (instant[from].attack[order].shoot)
                    {
                        case 1:
                            if (instant[to].doblock[1] == false && instant[to].doca[1] == false)
                            {
                                status[to].HP = status[to].HP - instant[from].attack[order].damage;
                                interactInstant[from].damageSource[to] = true;
                                interactInstant[from].possibleKiller[to] = true;
                            }
                            if (instant[to].doca[1] == true)
                            {
                                status[from].HP = status[from].HP - instant[from].attack[order].damage;
                                interactInstant[to].damageSource[from] = true;
                            }
                            break;
                        case 2:
                            if (instant[to].doblock[2] == false)
                            {
                                status[to].HP = status[to].HP - instant[from].attack[order].damage;
                                interactInstant[from].possibleKiller[to] = true;
                            }
                            break;
                        case 3:
                            if (instant[to].doblock[1] == false && instant[to].doca[1] == false && instant[to].doca[4] == false)
                            {
                                status[to].HP = status[to].HP - instant[from].attack[order].damage;
                                interactInstant[from].damageSource[to] = true;
                                interactInstant[from].possibleKiller[to] = true;
                            }
                            if (instant[to].doca[1] == true || instant[to].doca[4] == true)
                            {
                                status[from].HP = status[from].HP - instant[from].attack[order].damage;
                                interactInstant[to].possibleKiller[from] = true;
                            }
                            break;
                        case 4:
                            if (instant[to].doblock[0] == true && instant[to].doca[1] == false && instant[to].doca[4] == false)
                            {
                                status[to].HP = status[to].HP - instant[from].attack[order].damage;
                                interactInstant[from].damageSource[to] = true;
                                interactInstant[from].possibleKiller[to] = true;
                            }
                            if (instant[to].doca[1] == true || instant[to].doca[4] == true)
                            {
                                status[from].HP = status[from].HP - instant[from].attack[order].damage;
                                interactInstant[to].possibleKiller[from] = true;
                            }
                            break;
                        case 5:
                            if (instant[to].doblock[1] == false && instant[to].doca[1] == false)
                            {
                                status[to].HP = status[to].HP - instant[from].attack[order].damage;
                                interactInstant[from].damageSource[to] = true;
                                interactInstant[from].possibleKiller[to] = true;
                            }
                            if (instant[to].doca[1] == true)
                            {
                                status[from].HP = status[from].HP - instant[from].attack[order].damage;
                                interactInstant[to].possibleKiller[from] = true;
                            }
                            break;
                        case 6:
                            if (instant[to].doblock[2] == false)
                            {
                                status[to].HP = status[to].HP - instant[from].attack[order].damage;
                                interactInstant[from].damageSource[to] = true;
                                interactInstant[from].possibleKiller[to] = true;
                            }
                            break;
                            //default:
                            //;//essencial???
                    }
                    switch (instant[from].attack[order].stab)
                    {
                        case 1:
                            if (instant[to].doblock[1] == false && instant[to].doblock[2] == false && instant[to].doca[1] == false
                                && instant[to].doca[2] == false && instant[to].doca[3] == false && instant[to].doca[4] == false)
                            {
                                status[to].HP = status[to].HP - instant[from].attack[order].damage;
                                interactInstant[from].damageSource[to] = true;
                                interactInstant[from].possibleKiller[to] = true;
                            }
                            if (instant[to].doca[2] == true)
                            {
                                status[from].HP = status[from].HP - instant[from].attack[order].damage;
                                interactInstant[to].possibleKiller[from] = true;
                            }
                            if (instant[to].doca[3] == true)
                            {
                                status[from].swordinCD = status[from].sword;
                            }
                            break;
                        case 2:
                            if (instant[to].doblock[3] == false && instant[to].doca[3] == false)
                            {
                                status[to].HP = status[to].HP - instant[from].attack[order].damage;
                                interactInstant[from].damageSource[to] = true;
                                interactInstant[from].possibleKiller[to] = true;
                            }
                            if (instant[to].doca[3] == true)
                            {
                                status[from].swordinCD = status[from].sword;
                            }
                            break;
                        case 3:
                            if (instant[to].doblock[1] == false && instant[to].doblock[2] == false && instant[to].doca[1] == false
                                && instant[to].doca[2] == false && instant[to].doca[3] == false && instant[to].doca[4] == false)
                            {
                                status[to].HP = status[to].HP - instant[from].attack[order].damage;
                                interactInstant[from].damageSource[to] = true;
                                interactInstant[from].possibleKiller[to] = true;
                            }
                            if (instant[to].doca[2] == true || instant[to].doca[4] == true)
                            {
                                status[from].HP = status[from].HP - instant[from].attack[order].damage;
                                interactInstant[to].possibleKiller[from] = true;
                            }
                            if (instant[to].doca[3] == true)
                            {
                                status[from].swordinCD = status[from].sword;
                            }
                            break;
                        case 4:
                            if (instant[to].doblock[3] == false && instant[to].doca[3] == false && instant[to].doca[4] == false)
                            {
                                status[to].HP = status[to].HP - instant[from].attack[order].damage;
                                interactInstant[from].damageSource[to] = true;
                                interactInstant[from].possibleKiller[to] = true;
                            }
                            if (instant[to].doca[4] == true)
                            {
                                status[from].HP = status[from].HP - instant[from].attack[order].damage;
                                interactInstant[to].possibleKiller[from] = true;
                            }
                            if (instant[to].doca[3] == true)
                            {
                                status[from].swordinCD = status[from].sword;
                            }
                            break;
                        case 5:
                            if (instant[to].doblock[4] == false)
                            {
                                status[to].HP = status[to].HP - instant[from].attack[order].damage;
                                interactInstant[from].damageSource[to] = true;
                                interactInstant[from].possibleKiller[to] = true;
                            }
                            break;
                        case 6:
                            if (instant[to].doblock[4] == false)
                            {
                                status[to].HP = status[to].HP - instant[from].attack[order].damage;
                                interactInstant[from].damageSource[to] = true;
                                interactInstant[from].possibleKiller[to] = true;
                            }
                            break;
                    }
                }
            }
        }
    }
    
}