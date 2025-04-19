using System.Collections.Generic;

[System.Serializable]
public struct CharacterData
{
    private int Id;
    private float Hp;
    private float MaxHp;
    private int bulletCount;
    private int swordCount;
    private int swordCD;
    private bool isDead;
    private Dictionary<int,BuffDefine> buffs;
    public int ID => Id;
    public float HP => Hp;
    public float MaxHP => MaxHp;
    public int BulletCount => bulletCount;
    public int SwordCount => swordCount;
    public int SwordCD => swordCD;
    public bool IsDead => isDead;
    public Dictionary<int, BuffDefine> Buffs => buffs;


    internal void Init(int MaxHP, int ID)
    {
        MaxHp = MaxHP;
        Id = ID;
        isDead = false;
        buffs = new Dictionary<int, BuffDefine>();
    }
    internal void Changed(float HP, int BulletCount,int SwordCount, int SwordCD,float MaxHP)
    {
        Hp += HP;
        if (Hp <= 0)
        {
            isDead = true;
        }
        bulletCount += BulletCount;
        swordCount += SwordCount;
        if (swordCD != 0) 
        {
            swordCount += swordCD;
            swordCD = 0;
        } 
        swordCD += SwordCD;
        MaxHp += MaxHP;
    }
}