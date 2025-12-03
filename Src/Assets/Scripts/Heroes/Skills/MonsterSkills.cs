using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class ProtectingAlly : PhasebasedSkill
{
    public ProtectingAlly()
    {
        ID = "Save Me";
    }
    public override void AfterResolution(Player thisPlayer)
    {
        if(thisPlayer.status.life.Value == LifeStatus.Death)
            BattleManager.Instance.OnDefeated.Invoke();
    }
}

public class ToyAssemblyLine : PhasebasedSkill
{
    List<string> Sequence = new() { "Toy Warrior", "Toy Guardian", "Toy Minion" };
    int whichOne = 0;
    List<int> Cost = new() { 3, 3, 1 };
    public ToyAssemblyLine()
    {
        ID = "Toy Assembly Line";
    }
    public override void OnStartPhase(Player Factory)
    {
        if (PlayerManager.Instance.EnemyReachMaxNumber())
            return;
        if (Factory.status.resources.Bullet.Value >= Cost[whichOne] && PlayerManager.Instance.ThereisAvailablePositions(true))
        {
            Factory.status.resources.Bullet.Use(Cost[whichOne]);
            AIDataBase.Instance.AIDictionary.TryGetValue(Sequence[whichOne], out var aIDefine);
            PlayerManager.Instance.CreateAI(aIDefine, false, LevelManager.Instance.GetCurrentLevel());
            PrintEvent.Instance.log += ("工厂制造了" + Sequence[whichOne] + "消耗" + Cost[whichOne] + "子弹");
            whichOne++;
            whichOne %= 3;
        }
    }
}

public class CastleGuardian :TriggerSkill
{
    private bool isUsed;
    public CastleGuardian()
    {
        ID = "Castle Guardian";
        isUsed = false;
    }

    public override bool OnDeath(Player castle)
    {
        if (isUsed)
            return false;
        castle.status.HP.Heal(castle.status.MaxHP);
        castle.status.life.Revive();

        var toRemove = new List<Skill>();

        foreach (Skill skill in castle.hero.skills)
        {
            if (skill.ID == "Toy Assembly Line")
                toRemove.Add(skill);
        }

        foreach (Skill skill in toRemove)
            castle.hero.skills.Remove(skill);

        castle.AvailableActions.AddRange(new List<string> { "shoot", "double_shoot" });

        if (castle is AIPlayer aiCastle)
        {
            CharacterDataBase.Instance.CharacterDictionary.TryGetValue("Bellicose", out var bellicose);
            aiCastle.CharacterDefine = bellicose;
        }

        PrintEvent.Instance.log += ("再生...\n");
        isUsed = true;
        return true;
    }
}


public class FightAgain : TriggerSkill
{
    private bool isUsed;
    public FightAgain()
    {
        ID = "Fight Again";
        isUsed = false;
    }
    public override bool OnDeath(Player fighter)
    {
        if (isUsed)
            return false;
        fighter.status.HP.Heal(fighter.status.MaxHP / 2);
        fighter.status.life.Revive();

        fighter.AvailableActions.AddRange(new List<string> { "cleave" });

        PrintEvent.Instance.log += ("再战...\n");
        isUsed = true;
        return true;
    }

}

public class WaraxeDanceSkill : ActionSkill
{
    public WaraxeDanceSkill()
    {
        ID = "Waraxe Dance";
        ActionID = "waraxe_dance";
    }
}

public class TimeDistortion : PhasebasedSkill
{
    int CD = 0;
    bool isUsed = false;
    public TimeDistortion()
    {
        ID = "Time Distortion";
    }
    //感觉CD，ID，还有这个log都可以进行整合，之后看一看
    public override void OnStartPhase(Player thisPlayer)
    {
        if (CD == 0)
        {
            PrintEvent.Instance.log += "时间扭曲.. 本回合行动翻倍";
            isUsed = true;
            CD = 5;
        }
        else
            CD--;
    }
    public override void BeforeResolution(Player thisPlayer)
    {
        foreach(var player in PlayerManager.Instance.Players.Values)
        {
            List<ActionDefine> duplicatedList = new();
            foreach(var action in player.action)
            {
                var newAction = (ActionDefine)action.Clone();
                action.Costs = new();
                duplicatedList.Add(newAction);
            }
            player.action.AddRange(duplicatedList);
        }
    }
}