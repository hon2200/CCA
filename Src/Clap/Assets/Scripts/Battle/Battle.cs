using Common.Data;
using GameServer.Managers;
using JetBrains.Annotations;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using static Room;
using static UnityEditor.Progress;
using static UnityEngine.GraphicsBuffer;
/// <summary>
/// 此类存在极大局限性，后续如果改变游戏逻辑，需要重构,当然一般情况不需要
/// </summary>
internal struct Round
{
    private int id;
    private int cardid;
    private ItemType type;
    private string name;
    private List<int> targetid;
    public bool IsReady => cardid!=-1&&targetid!=null;

    public int Id => id;
    public int Cardid => cardid;
    public ItemType Type => type;
    public string Name => name;
    public List<int> Targetid => targetid;
    public void Add(int ids, int cardids, List<int> targetids)
    {
        
        DataManager.Instance.Items.TryGetValue(cardids,out ItemDefine item);
        if (cardids == -1)
        {
            item = new ItemDefine();
            item.ItemType = ItemType.None;
            item.Name = "无";
            targetids=new List<int>();
        }
        if (targetid==null)
        {
            targetid = new List<int>();
        }
        id = ids;
        cardid = cardids;
        type = item.ItemType;
        name = item.Name;
        targetid = targetids;
    }
    public void ChangeTarget(List<int> targetids)
    {
        targetid = targetids;
    }
}

public class Battle : Singleton<Battle>
{
    public int currentRound = 1;
    public TextMeshProUGUI text;
    public TextEffectManager textEffectManager;
    public string DisplayText = "";

    internal Dictionary<int, Round> CurrentRoundCharacters = new Dictionary<int, Round>();//用于记录当前回合行动
    Dictionary<int, RoundChange> RoundChanges = new Dictionary<int, RoundChange>();//用于记录玩家的属性变动
    int[] orders;
    List<int> comeid;
    public void Add(int id, int cardid = -1, List<int> targetid = null)
    {
        Round s = new Round();
        s.Add(id, cardid, targetid);
        CurrentRoundCharacters[id] = s;
    }
    public void OnEnable()
    {

    }
    public bool AllReady()
    {
        foreach (var kv in CurrentRoundCharacters.Values)
        {
            if (!kv.IsReady)
            {
                return false;
            }
        }
        return true;
    }
    public void Show(int id, ItemType type, string name, List<int> targetid)
    {
        switch (type)
        {
            case ItemType.补给:
                DisplayText += $"玩家{id}使用了{name}对自己进行补给\n";
                break;
            case ItemType.近战:
            case ItemType.远程:
            case ItemType.防御:
            case ItemType.反制:
                DisplayText += $"玩家{id}使用了{name}+{type}了玩家{targetid}\n";
                break;
            case ItemType.特效:
                DisplayText += $"玩家{id}使用了{name}\n";
                break;
            default:
                break;
        }
        Debug.Log(DisplayText);
        //textEffectManager.Show(DisplayText);
    }
    public void ShowCharcter()
    {
        orders = new int[Room.Instance.room.NumberCount];
        foreach (var data in BattleManager.Instance.Characters.Values)
        {
            RoundChanges.Add(data.ID, new RoundChange
            {
                Id = data.ID,
                isDefend = null,
                isParry = null,
                IsCome = false,
                buffs = data.Buffs,
            });
        }

        List<int> toAdd = new List<int>();
        var tempList = CurrentRoundCharacters.Values.ToList();
        foreach (var kv in tempList)
        {
            if (kv.Cardid == 29)
            {
                if (comeid == null)
                {
                    comeid = new List<int>();
                }
                comeid.Add(kv.Id);
            }
            if (kv.IsReady == false)
            {

                List<int> s = new List<int>();
                s.Add(kv.Id);
                toAdd.Add(kv.Id);
                Add(kv.Id, 4, s);
                Show(kv.Id, ItemType.补给, "补给", s);
                continue;
            }
            Show(kv.Id, kv.Type, kv.Name, kv.Targetid);
        }
        foreach (var id in toAdd)
        {
            List<int> s = new List<int> { id };
            Add(id, 4, s);
        }
        Order();
    }
    public void Order()
    {
        var characters = CurrentRoundCharacters.Values.ToList();
        characters.Sort((a, b) =>
            DataManager.Instance.Items[a.Cardid].Order
            .CompareTo(DataManager.Instance.Items[b.Cardid].Order));
        for (int i = 0; i < characters.Count; i++)
        {
            orders[i] = characters[i].Id;
        }
        Active();
    }
    public void Active()
    {
        int i = 0;
        for (i = 0; i < orders.Length; i++)
        {
            Round currentRound = CurrentRoundCharacters[orders[i]];//用于记录当前回合行动
            ItemDefine item = DataManager.Instance.Items[currentRound.Cardid];//用于记录当前卡牌的信息
            if (BoolCome(currentRound, item)) currentRound = CurrentRoundCharacters[orders[i]];//第四步，是否受战吼影响{
            RoundChange roundChange = RoundChanges[orders[i]];//用于记录玩家的属性变动
            List<RoundChange> targetChange = new List<RoundChange>();//用于记录对敌人的属性变动
            foreach (var targetid in currentRound.Targetid)
            {
                targetChange.Add(RoundChanges[targetid]);//用于记录对敌人的属性变动
            }
            LastBuff(roundChange);//第一步，检测上回合残留buff，例如嘲讽
            if (item.ItemType == ItemType.防御)
            {
                roundChange=IsDefend(roundChange, item);//第二步，检测是否使用防御，用于判定抵消伤害
            }
            if (item.ItemType == ItemType.反制)
            {
                roundChange=IsParry(roundChange, item); //第三步，过来，反制，附加回伤buff，同时未来拓展buff
            }
            roundChange=Deplete(roundChange, targetChange, item, currentRound);//第五步，计算消耗或补给

            if (item.ItemType == ItemType.近战 || item.ItemType == ItemType.远程)
            {
                Damage(targetChange, item, currentRound); //第六步，伤害消耗计算，附加特殊buff，例如和平
                //TargetChange(roundChange, item, currentRound); //第六步，目标变更，有关于反制和防御的目标变更
            }
        }
        BuffRecalculate(); //第七步，buff表重计算，更新
        StatusChange(); //第八步，状态变更，例如死亡，眩晕，禁锢等
        UpdateData(); //第九步，更新数据
    }
    private void LastBuff(RoundChange roundChange)//第一步，检测上回合残留buff，例如非伤害类buff
    {
        //启用Buff后完善buff表
    }
    private RoundChange IsDefend(RoundChange data, ItemDefine item) //第二步，检测是否使用防御，用于判定抵消伤害
    {

        if (data.isDefend == null)
        {
            data.isDefend = item.DEFType;
            RoundChanges[data.Id] = data;
            
        }
        if (item.Buff != 0) {  }
        return RoundChanges[data.Id];
    }
    private RoundChange IsParry(RoundChange data, ItemDefine item) //第三步，反制，附加回伤buff，同时未来拓展buff
    {
        if (data.isParry == null)
        {
            data.isParry = item.ParryType;
            RoundChanges[data.Id] = data;
           
        }
        if (item.Buff != 0) {  }
        return RoundChanges[data.Id];
    }
    private bool BoolCome(Round currentRound, ItemDefine item) //第四步，是否受战吼影响
    {
        if (item.ItemType == ItemType.补给 || item.ItemType == ItemType.近战 || item.ItemType == ItemType.远程)
        {
            if (comeid != null && comeid.Count != 0)
            {
                var round = CurrentRoundCharacters[currentRound.Id];
                round.ChangeTarget(comeid);
                CurrentRoundCharacters[currentRound.Id] = round;
                return true;
            }
        }
        return false;
    }
    private RoundChange Deplete(RoundChange data, List<RoundChange> targetChange, ItemDefine item, Round currentRound) //第五步，计算消耗
    {
        List<RoundChange> ss = new List<RoundChange>();
        if (item.ItemType == ItemType.补给)
        {
            ss = targetChange;
        }
        else
        {
            ss.Add(data);
        }
        if (item.Name == "激光炮")
        {
            for (int i = 0; i < ss.Count; i++)
            {
                var target = ss[i];
                CharacterData sss = new CharacterData();
                foreach (var sq in BattleManager.Instance.Characters)
                {
                    if (sq.Value.ID == currentRound.Id)
                    {
                        sss = sq.Value;
                        break;
                    }
                }
                target.BulletChanged -= (int)(sss.HP - 1);
                message s = new message
                {
                    from = currentRound.Id,
                    cardid = item.ID,
                    type = HurtType.Bullet,
                    value = -(int)(sss.HP - 1),
                };
                target.Add(s);
                RoundChanges[target.Id] = target;
            }
        }
        if (item.CostType != null)
        {
            switch (item.CostType)
            {
                case "子弹":
                    for (int i = 0; i < ss.Count; i++)
                    {
                        var target = ss[i];
                        target.BulletChanged -= item.Cost;
                        message s = new message
                        {
                            from = currentRound.Id,
                            cardid = item.ID,
                            type = HurtType.Bullet,
                            value = -item.Cost,
                        };
                        target.Add(s);
                        RoundChanges[target.Id] = target;
                    }
                    break;
                case "剑":
                    for (int i = 0; i < ss.Count; i++)
                    {
                        var target = ss[i];
                        target.SwardChanged -= item.Cost;
                        if (item.Cost > 0) target.InCDChanged += item.Cost;
                        message s = new message
                        {
                            from = currentRound.Id,
                            cardid = item.ID,
                            type = HurtType.Sward,
                            value = -item.Cost,
                        };
                        target.Add(s);
                        if (item.Cost > 0)
                        {
                            s = new message
                            {
                                from = currentRound.Id,
                                type = HurtType.InCD,
                                value = item.Cost,
                            };
                            target.Add(s);
                        }
                        RoundChanges[target.Id] = target;
                    }
                    break;
            }
        }
        if (item.Buff != 0) { /*buff填充*/}//持续性消耗类buff，或者透支类buff
        return RoundChanges[data.Id];
    }
    private void Damage(List<RoundChange> targetdata, ItemDefine item, Round currentRound) //第六步，伤害计算，附加特殊buff，例如和平
    {
        for (int i = 0; i < targetdata.Count; i++)
        {
            var target = targetdata[i];
            Round targetRound = CurrentRoundCharacters[target.Id];
            if (item.Hurt != 0)
            {
                switch (item.HurtType)
                {
                    case "血量":
                        if ((target.isDefend != null && target.isDefend.Contains<int>(item.ID))|| (targetRound.Targetid.Contains(currentRound.Id)&& DataManager.Instance.Items[targetRound.Cardid].ATK> item.ATK))
                        {
                            target.OffsetsDamage += item.Hurt;
                            message s = new message
                            {
                                cardid = item.ID,
                                from = currentRound.Id,
                                type = HurtType.None,
                                value = 0,
                            };
                            target.Add(s);
                            RoundChanges[target.Id] = target;
                        }
                        else if (target.isParry != null && target.isParry.Contains<int>(item.ID))
                        {
                            target = RoundChanges[currentRound.Id];
                            target.HpChanged -= item.Hurt;
                            message s = new message
                            {
                                cardid = item.ID,
                                from = targetdata[i].Id,
                                type = HurtType.Hp,
                                value = -item.Hurt,
                            };
                            target.Add(s);
                            RoundChanges[target.Id] = target;
                        }
                        else
                        {
                            target.HpChanged -= item.Hurt;

                            message s = new message
                            {
                                cardid = item.ID,
                                from = currentRound.Id,
                                type = HurtType.Hp,
                                value = -item.Hurt,
                            };
                            target.Add(s);
                            RoundChanges[target.Id] = target;
                        }
                        break;
                }
            }
            if (item.Name == "激光炮")
            {
                CharacterData sss = new CharacterData();
                foreach (var s in BattleManager.Instance.Characters)
                {
                    if (s.Value.ID == currentRound.Id)
                    {
                        sss = s.Value;
                        break;
                    }
                }
                if (target.isDefend != null && target.isDefend.Contains<int>(item.ID))
                {
                    target.OffsetsDamage += sss.HP;
                    message s = new message
                    {
                        cardid = item.ID,
                        from = currentRound.Id,
                        type = HurtType.None,
                        value = 0,
                    };
                    target.Add(s);
                    RoundChanges[target.Id] = target;
                }
                else if (target.isParry != null && target.isParry.Contains<int>(item.ID))
                {
                    target = RoundChanges[currentRound.Id];
                    target.HpChanged -= sss.HP;
                    message s = new message
                    {
                        cardid = item.ID,
                        from = targetdata[i].Id,
                        type = HurtType.Hp,
                        value = -sss.HP,
                    };
                    target.Add(s);
                    RoundChanges[target.Id] = target;
                }
                else
                {
                    target.HpChanged -= sss.HP;

                    message s = new message
                    {
                        cardid = item.ID,
                        from = currentRound.Id,
                        type = HurtType.Hp,
                        value = -sss.HP,
                    };
                    target.Add(s);
                    RoundChanges[target.Id] = target;
                }
            }
            if (item.Buff != 0) { /*buff填充*/}
        }
    }
    //private void TargetChange(RoundChange data, ItemDefine item, Round currentRound) //第六步，目标变更，有关于反制和防御的目标变更
    //{

    //}
    private void BuffRecalculate() //第七步，buff表重计算，更新
    {
        foreach (var kv in RoundChanges.Values)
        {
            if (kv.buffs != null)
            {
                /*buff填充*/
            }
        }
    }
    private void StatusChange() //第八步，状态变更，例如死亡，眩晕，禁锢等
    {
        List<int> keys = new List<int>(RoundChanges.Keys);
        foreach (var kv in keys)
        {
            RoundChange values = RoundChanges[kv];
            CharacterData data = new CharacterData();
            foreach (var s in BattleManager.Instance.Characters)
            {
                if (s.Value.ID == kv)
                {
                    data = s.Value;
                    break;
                }
            }
            if (values.HpChanged + data.HP <= 0)
            {
                foreach (var from in values.AttributeChanged)
                {
                    if (from.Key == kv) continue;
                    RoundChange roundChange = RoundChanges[from.Key];
                    roundChange.BulletChanged += (int)(data.HP / 5) + 2;
                    message s = new message
                    {
                        from = data.ID,
                        type = HurtType.Bullet,
                        value = (int)(data.HP / 5) + 2,
                    };
                    roundChange.Add(s);
                    RoundChanges[from.Key] = roundChange;
                }
            }
            /*buff填充*/
        }
    }
    private void UpdateData() //第九步，更新数据       
    {
        List<RoundMove> moves = new List<RoundMove>();
        foreach (var kv in CurrentRoundCharacters.Values)
        {
            RoundMove move = new RoundMove
            {
                id = kv.Id,
                type = kv.Type,
                Name = kv.Name,
                targetid = kv.Targetid,
                cardid = kv.Cardid,
            };
            moves.Add(move);
        }
        Room.Instance.RoundMoves.Add(currentRound, moves);
        List<RoundChanged> changeds = new List<RoundChanged>();
        foreach (var kv in RoundChanges.Values)
        {
            RoundChanged changed = new RoundChanged
            {
                id = kv.Id,
                HpChanged = kv.HpChanged,
                BulletChanged = kv.BulletChanged,
                SwardChanged = kv.SwardChanged,
                InCDChanged = kv.InCDChanged,
                MaxHpChanged = kv.MaxHpChanged,
            };
            changeds.Add(changed);
        }
        Room.Instance.RoundChangeds.Add(currentRound, changeds);

        foreach (var kv in RoundChanges)
        {
            int key = 0;
            CharacterData data = new CharacterData();
            foreach (var s in BattleManager.Instance.Characters)
            {
                if (s.Value.ID == kv.Key)
                {
                    data = s.Value;
                    key = s.Key;
                    data.Changed(kv.Value.HpChanged, kv.Value.BulletChanged, kv.Value.SwardChanged, kv.Value.InCDChanged, kv.Value.MaxHpChanged);
                    break;
                }
            }
            BattleManager.Instance.Characters[key] = data;
        }
        NextRound();

    }
    private void NextRound()
    {
        currentRound++;
        CurrentRoundCharacters.Clear();
        RoundChanges.Clear();
        if (comeid != null) comeid.Clear();
        foreach (var kv in Room.Instance.room.NumberID)
        {
            if (!BattleManager.Instance.IsDead(kv))
            {
                Add(kv);
            }
        }
    }
}
