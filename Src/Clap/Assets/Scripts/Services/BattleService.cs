using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct Attack
{
    public int id;
    public int damage;
    public int targetid;
    public int cardid;
}

public class BattleService : Singleton<BattleService>
{
    public Dictionary<int, Attack> cards = new Dictionary<int, Attack>();

    internal void Action(int id, int cardid, int targetid=0)
    {
    }
}
