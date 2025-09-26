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
    // 行动技能需要执行的方法
    public abstract void ExecuteAction();

    // 可以在执行前检查条件
    public virtual bool CanExecute()
    {
        return true; // 默认可以执行
    }
}
