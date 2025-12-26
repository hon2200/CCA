using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// The same problem as ActionDefine, how to make it safe, what if I forgot to Clone?
public abstract class Skill
{
    public string ID { get; protected set; }
    public int CD { get; protected set; }
    public int CDProgress { get; protected set; }
    public List<int> Costs { get; protected set; }
    public string Name { get; protected set; }
    public bool IsLimited { get; protected set; }
    public int LimitedTimes { get; protected set; }
    public int UsedTimes { get; protected set; }
    protected Skill(string id)
    {
        ID = id;
        Init();                 // ← ALWAYS init at creation
    }
    //目前就只是从MonsterSkill里面寻找
    protected void Init()
    {
        SkillDataBase.Instance.MonsterSkillDic.TryGetValue(ID, out var skillDefine);
        CD = skillDefine.CD;
        CDProgress = 0;
        Costs = skillDefine.Costs;
        Name = skillDefine.Name;
        IsLimited = skillDefine.IsLimited;
        UsedTimes = 0;
        if (IsLimited)
        {
            LimitedTimes = skillDefine.LimitedTimes;
        }
        else
        {
            LimitedTimes = 0;
        }
    }
    protected virtual bool IsAvailable(Player thisPlayer)
    {
        if (CDProgress > 0)
            return false;
        if (IsLimited && UsedTimes >= LimitedTimes)
            return false;
        if (thisPlayer.status.resources.Bullet.Value < Costs[1])
            return false;
        if (thisPlayer.status.resources.Sword.AvailableSword.Value < Costs[2])
            return false;
        return true;
    }
    public void CDCountDown()
    {
        if (CDProgress > 0)
            CDProgress--;
    }
    //调用时，日志记录，并且进入CD
    private void Log(Player thisPlayer)
    {
        PrintEvent.Instance.log += (thisPlayer.Name + "使用了" + Name + "\n");
    }
    private void PayCosts(Player thisPlayer)
    {
        CDProgress = CD;
        if (IsLimited)
            UsedTimes += 1;
        thisPlayer.status.HP.Drain(Costs[0]);
        thisPlayer.status.resources.Bullet.Use(Costs[1]);
        thisPlayer.status.resources.Sword.Use(Costs[2]);
    }
    protected bool CheckAndEvoke(Player thisPlayer)
    {
        if(IsAvailable(thisPlayer))
        {
            Log(thisPlayer);
            PayCosts(thisPlayer);
            Envoke(thisPlayer);
            return true;
        }
        return false;
    }
    protected virtual void Envoke(Player thisPlayer) { }
    public void Copy(Skill target)
    {
        var type = GetType(); // e.g. NuclearBomb, not just ActionDefine

        // Get all instance properties (public and non-public) that can be written to
        var props = type.GetProperties(
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic);

        foreach (var prop in props)
        {
            if (!prop.CanWrite) continue; // skip read-only
            var value = prop.GetValue(this);

            if (value == null)
            {
                prop.SetValue(target, null);
                continue;
            }

            if (value is System.Collections.IList list)
            {
                // Create a new list of the same type and copy elements
                var listType = list.GetType();
                var newList = (System.Collections.IList)Activator.CreateInstance(listType);
                foreach (var item in list)
                    newList.Add(item);
                prop.SetValue(target, newList);
            }
            else
            {
                // Shallow copy for primitives, strings, etc.
                prop.SetValue(target, value);
            }
        }
    }

    public Skill Clone()
    {
        // Use runtime type constructor instead of hardcoded ActionDefine
        var type = GetType();
        var clone = (Skill)Activator.CreateInstance(type);
        Copy(clone);
        return clone;
    }
}

public abstract class ActiveSkill : Skill
{
    public ActiveSkill(string id) : base(id) { }
    public abstract void SkillEffect();
}