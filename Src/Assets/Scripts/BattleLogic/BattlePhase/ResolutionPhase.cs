using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//�غϽ���׶νű�
public class ResolutionPhase : Singleton<ResolutionPhase>
{
    //����׶Σ������ҵ��ж�
    //����Ŀǰ��û�������buff�����Խ���׶ξ���д������
    public void Resolution()
    {
        foreach (var player in PlayerManager.Instance.Players.Values)
        {
            Consume(player);
        }
        foreach (var player in PlayerManager.Instance.Players.Values)
        {
            Provoke(player);
        }
        foreach(var player in PlayerManager.Instance.Players.Values)
        {
            Comeon(player);
        }
        foreach(var player in PlayerManager.Instance.Players.Values)
        {
            Supply(player);
            Attack(player);
        }
        PrintResult_Debug();
    }
    public void PrintResult_Debug()
    {
        if (BattleManager.Instance.Turn.Value == 1)
        {
            Log.PrintSpecificPropertiesInDictionary(PlayerManager.Instance.Players, 
                new string[] {"ID_inGame", "status"},"Log/InGame/PlayerStatus.txt");
            Log.PrintNestedPropertyInDictionary(PlayerManager.Instance.Players,
                "action.Value", "Log/InGame/PlayerAction.txt");
        }
        else
        {
            Log.PrintSpecificPropertiesInDictionary(PlayerManager.Instance.Players,
                new string[] { "ID_inGame", "status" }, "Log/InGame/PlayerStatus.txt", false);
            Log.PrintNestedPropertyInDictionary(PlayerManager.Instance.Players,
                "action.Value", "Log/InGame/PlayerAction.txt",false);
        }

    }
    private void Supply(Player player)
    {
        var supplys = player.SelectActionType<SupplyDefine>();
        foreach (var supply in supplys)
        {
            PlayerManager.Instance.Players.TryGetValue(supply.Target, out Player receiver);
            //����������Ҳ���԰�resource�ĳ�һ��list<int>�����ǿ��ǵ�resource���滹��swordinCD�����ԾͲ�����
            receiver.status.resources.Bullet.Get(supply.ActionInfo.SupplyNumber[0]);
            receiver.status.resources.Sword.Get(supply.ActionInfo.SupplyNumber[1]);
            receiver.status.resources.AvailableSword.Get(supply.ActionInfo.SupplyNumber[1]);
        }
    }
    private void Comeon(Player theComeonOne)
    {
        List<BattleAction<SpecialDefine>> specials = theComeonOne.SelectActionType<SpecialDefine>();
        if(theComeonOne.DoYouComeon())
            foreach(var theBeComeonedOne in PlayerManager.Instance.Players)
            {
                var attacks = theBeComeonedOne.Value.SelectActionType<AttackDefine>();
                var supplys = theBeComeonedOne.Value.SelectActionType<SupplyDefine>();
                foreach(var attack in attacks)
                {
                    PlayerManager.Instance.Players.TryGetValue(attack.Target, out Player originalTarget);
                    //�������ж��Ѿ��ǹ����ĸ������ˣ���ô��μ��������ظ�
                    if (attack.ActionInfo.isCopy)
                        continue;
                    //�����������ʹ�ù��������������ᴴ��˺��
                    //����������������Ƕ��Լ��Ĺ�������ôֱ��������һ��
                    if (originalTarget.DoYouComeon() && originalTarget.ID_inGame != theComeonOne.ID_inGame)
                    {
                        var attackcopy = (BattleAction<AttackDefine>)attack.Clone();
                        attackcopy.Target = theComeonOne.ID_inGame;
                        attackcopy.ActionInfo.isCopy = true;
                        theBeComeonedOne.Value.action.Add(attackcopy, "InGame");
                    }
                    //����ֱ�Ӹı�Ŀ��
                    else
                    {
                        attack.Target = theComeonOne.ID_inGame;
                    }
                }
                foreach(var supply in supplys)
                {
                    PlayerManager.Instance.Players.TryGetValue(supply.Target, out Player originalTarget);
                    //�������ж��Ѿ��ǹ����ĸ������ˣ���ô��μ��������ظ�
                    if (supply.ActionInfo.isCopy)
                        continue;
                    //�����������ʹ�ù��������������ᴴ��˺��
                    if (originalTarget.DoYouComeon() && originalTarget.ID_inGame != theComeonOne.ID_inGame)
                    {
                        var supplycopy = (BattleAction<SupplyDefine>)supply.Clone();
                        supplycopy.Target = theComeonOne.ID_inGame;
                        supplycopy.ActionInfo.isCopy = true;
                        theBeComeonedOne.Value.action.Add(supplycopy, "InGame");
                    }
                    //����ֱ�Ӹı�Ŀ��
                    else
                    {
                        supply.Target = theComeonOne.ID_inGame;
                    }
                }    
            }
    }
    private void Provoke(Player player)
    {
        List<BattleAction<SpecialDefine>> specials = player.SelectActionType_inHistory<SpecialDefine>(BattleManager.Instance.Turn.Value - 1, true);
        foreach (BattleAction<SpecialDefine> special in specials)
        {
            //������
            if (special.ActionInfo.ID == "provoke")
            {
                PlayerManager.Instance.Players.TryGetValue(special.Target, out Player enemy);
                special.ActionInfo.OnProvoke(player, enemy);
            }
        }
    }
    private void Attack(Player player)
    {
        List<BattleAction<AttackDefine>> attacks = player.SelectActionType<AttackDefine>();
        foreach(BattleAction<AttackDefine> attack in attacks)
        {
            //��ȡ������Ŀ�꣬��TargetΪ��ֵ���ҵ�enemy
            PlayerManager.Instance.Players.TryGetValue(attack.Target, out Player enemy);
            var enemy_attacks = enemy.SelectActionType<AttackDefine>();
            //�������ȼ��ж�
            if (attack.ActionInfo.Level > enemy.MaxLevel(player))
            {
                var counters = attack.WathoutforCounter(enemy);
                var defends = attack.WathoutforDefend(enemy);
                //��Ӧ���������ж�
                if (counters.Count > 0)
                {
                    foreach (var counter in counters)
                    {
                        counter.Item1.ActionInfo.HowtoCounter(counter.Item2, player, enemy, attack);
                    }
                }
                else if (defends.Count > 0)
                {
                    foreach(var defend in defends)
                    {
                        defend.ActionInfo.HowtoDefend(attack);
                    }
                }
                //�����������ˣ�
                else
                {
                    attack.ActionInfo.HowtoAttack(player, enemy, attack);
                }
            }
            else
            {
                attack.isEffective = false;
            }
        }
    }
    //���������Դ�������Լ����Ļظ�
    private void Consume(Player player)
    {
        bool useSword = false;
        foreach(var action in player.action)
        {
            player.status.resources.Bullet.Use(action.ActionInfo.Costs[0]);
            player.status.resources.AvailableSword.Use(action.ActionInfo.Costs[1]);
            if (action.ActionInfo.Costs[1] !=0) 
                useSword = true;
        }
        if(!useSword)
        {
            player.status.resources.AvailableSword.CoolDown(player.status.resources.Sword.Value);
        }
    }
}
