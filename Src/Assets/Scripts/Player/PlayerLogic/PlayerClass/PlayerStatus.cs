using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlayerStatus
{
    public HeroDefine playerDefine;
    public HPAttribute HP;
    public NewPlayerResource resources;
    public IsAliveAttribute life;
    public PlayerStatus(HeroDefine playerDefine)
    {
        this.playerDefine = playerDefine;
        life = new();
        life.Born();
        HP = new();
        resources = new();
        HP.Set(playerDefine.MaxHP);
        HP.OnValueChanged += (oldVal, newVal, opType) =>
            DeadCheck(newVal);
        resources.Bullet.Set(playerDefine.iniResource[0]);
        resources.Sword.Set(playerDefine.iniResource[1]);
        resources.AvailableSword.Set(playerDefine.iniResource[1] - playerDefine.iniResource[2]);
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