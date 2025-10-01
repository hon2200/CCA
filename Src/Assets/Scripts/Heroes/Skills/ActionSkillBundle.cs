using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//ID:是英雄的技能的ID
//ActionID:行动的ID
//种瓜得瓜
public class GatheringFoodI : ActionSkill
{
    public GatheringFoodI()
    {
        ID = "Gathering FoodI";
        ActionID = "triple_bullet";
    }
}

public class GatheringFoodII : ActionSkill
{
    public GatheringFoodII()
    {
        ID = "Gathering FoodII";
        ActionID = "triple_sword";
    }
}

//火药桶
public class PowderKeg : ActionSkill
{
    public PowderKeg()
    {
        ID = "Powder Keg";
        ActionID = "powder_keg";
    }
}