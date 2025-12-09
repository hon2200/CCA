using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SkillDefine
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string SkillType { get; set; }
    public bool IsLimited { get; set; }
    public int LimitedTimes { get; set; }
    public string Discription { get; set; }
    public string Explanation { get; set; }
    public string Bubble { get; set; }
    public List<int> Costs { get; set; }
    public int CD { get; set; }
    public bool ForeKnow { get; set; }

}
