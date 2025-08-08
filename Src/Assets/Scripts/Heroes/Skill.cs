using System.Collections;
using System.Collections.Generic;



public abstract class Skill
{
    public string Name;
    public string Discription;
}

//行动技，效果是新增一个行动
public abstract class ActionSkill : Skill
{
    public string ActionID;
}

//主动技，在某一个阶段，你随时可以发动这个技能，通常是回合开始阶段，回合结束阶段或补刀阶段。
//行动阶段和结算阶段各有其他事情。
public abstract class ActiveSkill : Skill
{
    public enum ActivePeriod
    {
        StartPhase = 1,
        EndPhase = 2,
        AdditionalPhase = 3
    }
    public ActivePeriod activePeriod;
}

//触发技，某种特定情况下触发，通常是结算阶段中间
public abstract class TriggerSkill : Skill
{
    public abstract void OnAttackEffective();
    public abstract void OnDealWithDamage();
    public abstract void OnDefendActive();
    public abstract void OnCounterActive();
    public abstract void OnWounded();
    public abstract void OnGameStart();
    public abstract void OnNegativeHP();
}

//行动处理技，通常用来在某个阶段改变行动的参数
public abstract class CookSkill : Skill
{
    public abstract void OnPreCook();
    public abstract void OnPostCook();
    public abstract void OnFirstCook();
}