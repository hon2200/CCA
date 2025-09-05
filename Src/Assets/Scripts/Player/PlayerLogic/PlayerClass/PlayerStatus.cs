using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlayerStatus
{
    public int MaxHP { get; set; }
    public HPAttribute HP { get; set; }
    public NewPlayerResource resources { get; set; }
    public IsAliveAttribute life { get; set; }
    public PlayerStatus()
    {
        HP = new();
        resources = new();
        life = new();
        life.Born();
        HP.OnValueChanged += (oldVal, newVal, opType) =>
            DeadCheck(newVal);
    }
    public PlayerStatus(int MaxHP, List<int> iniReources)
    {
        HP = new();
        resources = new();
        life = new();
        life.Born();
        HP.OnValueChanged += (oldVal, newVal, opType) =>
            DeadCheck(newVal);
        HP.Set(MaxHP);
        this.MaxHP = MaxHP;
        resources.Bullet.Set(iniReources[0]);
        resources.Sword.Set(iniReources[1]);
        resources.AvailableSword.Set(iniReources[1] - iniReources[2]);
    }

    //将这个函数添加到HPAttribute的订阅中，一旦HP的值发生改变，立刻检查是否死亡。
    private void DeadCheck(int HP)
    {
        if (HP <= 0)
            life.Die();
    }
    public void SaveStatus()
    {
        HP.Save();
        resources.Save();
        life.Save();
    }
    public void LoadStatus()
    {
        HP.Load();
        resources.Load();
        life.Load();
    }
}

public class NewPlayerResource
{
    public BulletAttibute Bullet;
    public SwordAttribute Sword;
    public AvailableSwordAttribute AvailableSword;
    public NewPlayerResource()
    {
        Bullet = new();
        Sword = new();
        AvailableSword = new();
    }
    public void Save()
    {
        Bullet.Save();
        Sword.Save();
        AvailableSword.Save();
    }
    public void Load()
    {
        Bullet.Load();
        Sword.Load();
        AvailableSword.Load();
    }
}