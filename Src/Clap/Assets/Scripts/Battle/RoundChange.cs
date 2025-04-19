using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public struct message
{
    public int from;
    public float value;
    public int cardid;
    public HurtType type;
}
public enum HurtType
{
    None,
    Hp,
    Bullet,
    Sward,
    InCD,
    MaxHp
}
public struct RoundChange
{
    public int Id;
    public float HpChanged;
    public int BulletChanged;
    public int SwardChanged;
    public int InCDChanged;
    public float MaxHpChanged;
    public float OffsetsDamage;
    public Dictionary<int, List<message>> AttributeChanged;
    public int[] isDefend;
    public int[] isParry;
    public bool IsCome;
    public Dictionary<int, BuffDefine> buffs;




    public bool Add(message data)
    {
        AttributeChanged ??= new Dictionary<int, List<message>>();

        if (!AttributeChanged.TryGetValue(data.from, out var list))
        {
            list = new List<message>();
            AttributeChanged[data.from] = list;
        }
        if (isParry!= null && data.type == HurtType.Hp  )
        {
            if (isParry.Contains(data.cardid))
            {
                return false;
            }
        }
        if (isDefend!=null && data.type == HurtType.Hp)
        {
            if (isDefend.Contains(data.cardid))
            {
                OffsetsDamage += data.value;
                data.value = 0;
                list.Add(data);
                return true;
            }
        }
        list.Add(data);
        AttributeChanged[data.from] = list;
        return true;
    }
}