using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LevelDefine
{
    public string ID { get; set; }
    public string EnglishName { get; set; }
    public string Name { get; set; }
    public List<List<string>> EnemyList { get; set; }
    public List<List<string>> FriendList { get; set; }
    public List<string> UnlockedAction { get; set; }
    public int PlayerHP { get; set; }
    public List<int> PlayerInitialResource { get; set; }
    public string NextLevel { get; set; }
    public string PreviousLevel { get; set; }
}