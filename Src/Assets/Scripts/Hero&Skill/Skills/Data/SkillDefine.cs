using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;



// The same problem as ActionDefine, how to make it safe, what if I forgot to Clone?
public abstract class SkillDefine
{
    public string ID { get; protected set; }
    public int CD { get; protected set; }
    public int CDProgress { get; protected set; }
    public List<int> Costs { get; protected set; }
    public string Name { get; protected set; }
    public bool IsLimited { get; protected set; }
    public int LimitedTimes { get; protected set; }
    public string Description { get; protected set;  }
    public int UsedTimes { get; protected set; }
    public Player Owner { get; protected set;  }
    /// <summary>
    /// Set the owning player. Call after cloning a skill onto a hero (e.g. in Hero constructor).
    /// </summary>
    public void SetOwner(Player owner) { Owner = owner; }
    protected SkillDefine(string id, Player owner = null)
    {
        ID = id;
        Init();                 // ?? ALWAYS init at creation
        Owner = owner;
    }
    protected virtual void Init()
    {
        // Base: reset runtime state. Data (CD, Costs, Name, etc.) is filled by derived Init() from JSON.
        CDProgress = 0;
        UsedTimes = 0;
        Costs = Costs ?? new List<int> { 0, 0, 0 };
    }

    /// <summary>
    /// Applies data from a JSON-loaded define (SkillDefineData) into this instance.
    /// Used by MonsterSkill/HeroSkill.Init() to complete from SkillDatabaseOrigin's Original* dictionaries.
    /// </summary>
    protected void ApplyDataFrom(SkillDefineOrigin data)
    {
        if (data == null) return;
        CD = data.CD;
        Name = data.Name ?? ID;
        IsLimited = data.IsLimited;
        LimitedTimes = data.LimitedTimes;
        Description = data.Description;
        Costs = data.Costs != null ? new List<int>(data.Costs) : new List<int> { 0, 0, 0 };
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
    //??????????????????????CD
    private void Log(Player thisPlayer)
    {
        PrintEvent.Instance.log += (thisPlayer.Name + "?????" + Name + "\n");
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
    public void Copy(SkillDefine target)
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

    public SkillDefine Clone()
    {
        // Use runtime type constructor instead of hardcoded ActionDefine
        var type = GetType();
        var clone = (SkillDefine)Activator.CreateInstance(type);
        Copy(clone);
        return clone;
    }
}

public abstract class ActiveSkill : SkillDefine
{
    public ActiveSkill(string id) : base(id) { }
    public abstract void SkillEffect();
}

/// <summary>
/// Base type for monster skills. Init() loads CD, Costs, Name, etc. from SkillDatabase.OriginalMonsterSkillDic.
/// </summary>
public class EnemySkill : SkillDefine
{
    public EnemySkill(string id, Player owner = null) : base(id, owner) { }

    protected override void Init()
    {
        base.Init();
        if (SkillDatabaseOrigin.Instance.OriginalEnemySkillDic != null
            && SkillDatabaseOrigin.Instance.OriginalEnemySkillDic.TryGetValue(ID, out SkillDefineOrigin data))
            ApplyDataFrom(data);
    }
}

/// <summary>
/// Base type for hero skills. Init() loads CD, Costs, Name, etc. from SkillDatabase.OriginalHeroSkillDic.
/// </summary>
public class HeroSkill : SkillDefine
{
    public HeroSkill(string id, Player owner = null) : base(id, owner) { }

    protected override void Init()
    {
        base.Init();
        if (SkillDatabaseOrigin.Instance?.OriginalHeroSkillDic != null
            && SkillDatabaseOrigin.Instance.OriginalHeroSkillDic.TryGetValue(ID, out SkillDefineOrigin data))
            ApplyDataFrom(data);
    }
}