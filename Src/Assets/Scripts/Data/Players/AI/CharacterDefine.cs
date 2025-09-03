using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CharacterDefine
{
    public string ID { get; set; }
    public string Name { get; set; }
    public int IniHonesty { get; set; }
    public int IniEmotion { get; set; }
    public int HonestyChange_TurnBased { get; set; }
    public int EmotionChange_TurnBased { get; set; }
    public int HonsetyChange_DamageBased { get; set; }
    public int EmotionChange_DamageBased { get; set; }
    public int maxHonesty { get; set; }
    public int minHonesty { get; set; }
    public int maxEmotion { get; set; }
    public int minEmotion { get; set; }
}

