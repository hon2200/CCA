using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CharacterDefine
{
    public string ID { get; set; }
    public string Name { get; set; }
    public float IniHonesty { get; set; }
    public float IniEmotion { get; set; }
    public float HonestyChange_TurnBased { get; set; }
    public float EmotionChange_TurnBased { get; set; }
    public float HonestyChange_DamageBased { get; set; }
    public float EmotionChange_DamageBased { get; set; }
    public float MaxHonesty { get; set; }
    public float MinHonesty { get; set; }
    public float MaxEmotion { get; set; }
    public float MinEmotion { get; set; }
}

