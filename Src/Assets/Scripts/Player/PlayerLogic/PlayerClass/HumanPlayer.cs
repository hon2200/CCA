using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class HumanPlayer : Player
{
    //创建闯关过程的玩家
    public void InitializePlayer(int ID_inGame, LevelDefine Level)
    {
        base.Initialize(ID_inGame, PlayerType.Human, 
            Level.PlayerHP, Level.PlayerInitialResource, Level.UnlockedAction);
        OnBirth?.Invoke();
    }
    //创建英雄模式的玩家
    public void InitializePlayer(int ID_inGame, HeroDefine heroDefine)
    {
        base.Initialize(ID_inGame, PlayerType.Human, heroDefine.MaxHP);
        OnBirth?.Invoke();
    }
}
