using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//人机的定义
//从json文件读入
//要开始考虑它的安全性问题了

public class AIDefine
{
    public string ID { get; set; }
    public string Name { get; set; }
    public int MaxHP { get; set; }
    public bool IsFriend { get; set; }
    public List<int> InitialResource { get; set; } //子弹，剑，可用剑
    public string CharacterID { get; set; }
    public List<string> AvailableActionID { get; set; }
}