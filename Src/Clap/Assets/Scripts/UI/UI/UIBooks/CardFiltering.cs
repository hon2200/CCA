using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CardFiltering : Singleton<CardFiltering>
{
    CharacterData currentCharacter;

    public void Set()
    {
        Rule.OnRoundChanged += Init;
        Init();
    }
    public void Init()
    {
            foreach (var cha in BattleManager.Instance.Characters.Values)
            {
                if (cha.ID == User.Instance.ID)
                {
                    currentCharacter = cha;
                    break;
                }
            }
    }
    public bool RoomChecks(ItemDefine kv)
    {///////////////////////////////////////////添加buffChech，用于一些特殊的限制卡牌效果//////////
        if (kv.ItemType == ItemType.货币) return false;
        if (BattleManager.Instance.Characters.Count == 0 || kv.CostType == null) return true;
        //如果后续要添加RuleDefine，即对卡牌进行限定，则需要启用这个方法
        if (kv.CostType == "子弹" && currentCharacter.BulletCount < kv.Cost)
        {
            return false;
        }
        if (kv.Name == "激光炮"&& currentCharacter.BulletCount < currentCharacter.HP-1)
        {
            return false;
        }
        int s = currentCharacter.SwordCount - currentCharacter.SwordCD;
        if (currentCharacter.SwordCount - currentCharacter.SwordCD < 0) s = 0;
        if (kv.CostType == "剑" && (s) < kv.Cost)
        {
            return false;
        }
        return true;
    }
}
