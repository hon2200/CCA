using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EmotionDefine
{
    public string Key { get; set; }
    public int ID { get; set; }
    public string Name { get; set; }
    public int EmotionalValueLowerLimit { get; set; }
    public int EmotionalValueUpperLimit { get; set; }
    public List<int> ActionTendency { get; set; }
    public int MultiAttackCheckValue { get; set; }
    public int MultiProvokeCheckValue { get; set; }
}
