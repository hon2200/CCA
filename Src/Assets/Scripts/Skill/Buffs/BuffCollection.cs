using UnityEngine;
using System;





















/*//All The Buffs
public class Stunned: Buff, IPhaseEnterHandler
{
    public Stunned(int duration) : base("Stunned", duration, true) { }
    public void OnPhase(Phase phase,Player thisPlayer)
    {
        if(phase is StartPhase)
        {
            foreach (var action in thisPlayer.AvailableActions)
            {
                if (ActionUtil.IsAction<DefendDefine>(action) || action == "provoke") ;
                else
                {
                    thisPlayer.ForbiddenActions.Add(action);
                }
            }
        }
    }
    public override void Fade(Player thisPlayer)
    {
        Value -= 1;
        if (Value <= 0)
            thisPlayer.status.buffs.Remove(this);
    }
}

public class Bleeding : Buff, IResolutionHandler
{
    public int LostFractionalHP;//Max 5, and lose one HP;
    public Bleeding(int value) : base("Bleeding", value, true) { }
    public void AfterResolution(Player thisPlayer)
    {
        FractionalDrain(thisPlayer, Value);
    }
    private void FractionalDrain(Player thisPlayer, int amount)
    {
        LostFractionalHP += amount;
        while (LostFractionalHP >= 5)
        {
            LostFractionalHP -= 5;
            thisPlayer.status.HP.Drain(1);
            PrintEvent.Instance.log += $"{thisPlayer}因为流血失去了1点HP";
        }
    }
}

public class Burning : Buff, IResolutionHandler
{
    public int Burned;
    public Burning(int value) : base("Burning", value, true) { }
    public void AfterResolution(Player thisPlayer)
    {
        Burned += Value;
        PrintEvent.Instance.log += ("现有灼烧" + Value + "\n");

        while (Burned >= 10)
        {
            Burned -= 10;

            int bullet = thisPlayer.status.resources.Bullet.Value;
            int sword = thisPlayer.status.resources.Sword.Value;

            // Case 1: nothing to lose
            if (bullet == 0 && sword == 0)
                return;

            // Case 2: both available → random pick
            if (bullet > 0 && sword > 0)
            {
                int x = UnityEngine.Random.Range(0, 2);
                if (x == 0)
                {
                    thisPlayer.status.resources.Bullet.Lost(1);
                    PrintEvent.Instance.log += $"{thisPlayer.Name} 因灼烧失去了一点子弹";
                }
                else
                {
                    thisPlayer.status.resources.Sword.Lost(1);
                    PrintEvent.Instance.log += $"{thisPlayer.Name} 因灼烧失去了一把剑";
                }
                continue;
            }

            // Case 3: only bullet available
            if (bullet > 0)
            {
                thisPlayer.status.resources.Bullet.Lost(1);
                PrintEvent.Instance.log += $"{thisPlayer.Name} 因灼烧失去了一点子弹";
                continue;
            }

            // Case 4: only sword available
            if (sword > 0)
            {
                thisPlayer.status.resources.Sword.Lost(1);
                PrintEvent.Instance.log += $"{thisPlayer.Name} 因灼烧失去了一把剑";
            }
        }
    }

}

public class Crystallized : Buff , IResolutionHandler
{
    public Crystallized(int value) : base("Crystallized", value, true) { }
    public override void Fade(Player thisPlayer)
    {
        foreach(var action in thisPlayer.action)
        {
            if (action is AttackDefine)
                Value += 1;
            else 
                Value -= 1;
        }
        PrintEvent.Instance.log += ($"{Value} 层晶化Now\n");
        if (Value <= 0)
        {
            thisPlayer.status.buffs.Remove(this);
            PrintEvent.Instance.log += ("Remove晶化\n");
        }

    }
    public void AfterResolution(Player thisPlayer)
    {
        int stunDuration = 0;
        while (Value >= 4)
        {
            Value -= 4;
            stunDuration += 2;
        }
        if (stunDuration > 0)
        {
            thisPlayer.status.buffs.Add(new Stunned(stunDuration));
            PrintEvent.Instance.log += ($"晶化爆发，玩家被眩晕{stunDuration}回合\n");
        }
    }
}

public class Cocooned : Buff, IResolutionHandler, IPhaseEnterHandler
{
    public Cocooned(int value): base("Cocooned", value, false) { }
    public override void Fade(Player thisPlayer)
    {
        Value -= 1;
        if(Value < 0)
            thisPlayer.status.buffs.Remove(this);
    }
    public void AfterResolution(Player thisPlayer)
    {
        if(Value <= 0)
        {
            thisPlayer.status.HP.Set(thisPlayer.status.MaxHP);
            PrintEvent.Instance.log += $"{thisPlayer.Name}重生于茧";
            OnRevive(thisPlayer);
        }
    }
    public void OnPhase(Phase phase,Player thisPlayer)
    {
        if(phase is StartPhase)
        {
            foreach (var action in thisPlayer.AvailableActions)
            {
                thisPlayer.ForbiddenActions.Add(action);
            }
        }
    }

    public event Action<Player> OnRevive;

}

//Strength的Buff定义需要完善
public class Strength : Buff, IPhaseExitHandler
{
    public Strength(int value) : base("Strength", value, false) { }
    public void ExitingPhase(Phase phase,Player thisPlayer)
    {
        if(phase is ChasePhase)
        {
            foreach (var attack in thisPlayer.SelectActionType<AttackDefine>())
            {
                attack.Level += Value * 0.5f;
            }
            Debug.Log("Strength" + Value);
        }
    }
}*/