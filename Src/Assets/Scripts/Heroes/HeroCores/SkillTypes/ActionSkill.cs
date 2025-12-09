using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//行动技：技能就是新增一个行动
//还需要卡牌的UI(Card Liberary.cs)
//行动信息(ActionDataBase.cs)
public abstract class ActionSkill : Skill
{
    public ActionSkill(string skillID, string actionID) : base(skillID) 
    { 
        ActionID = actionID;
    }
    public string ActionID { get; protected set; }
    public virtual void AddingAvailableAction(Player thisPlayer)
    {
        thisPlayer.AvailableActions.Add(ActionID);
    }
}
