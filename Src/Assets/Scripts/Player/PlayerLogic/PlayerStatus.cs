using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlayerStatus
{
    public PlayerDefine playerDefine;
    public HPAttribute HP;
    public NewPlayerResource resources;
    public PlayerStatus(PlayerDefine playerDefine)
    {
        this.playerDefine = playerDefine;
        HP = new();
        resources = new();
        HP.Set(playerDefine.MaxHP);
        resources.Bullet.Set(playerDefine.iniResource.Bullet);
        resources.Sword.Set(playerDefine.iniResource.Sword);
        resources.AvailableSword.Set(playerDefine.iniResource.Sword - playerDefine.iniResource.SwordinCD);
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
}