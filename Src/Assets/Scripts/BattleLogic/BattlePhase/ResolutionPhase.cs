using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//回合结算阶段脚本
//这个脚本存在的问题：极度依赖于Player类的内部结构，一旦Player类发生改变，这里面的所有相关引用都需要调整，
//关键是这种问题不止在这个脚本里面，基本上到处都有，Player类内部，Status和Action也即将产生耦合。
//怎么办呢？
//或许把代码分块会好一点，在Player类里面专门分一个块用来处理相关操作，这样这个结算类的功能就会简单很多，
//因为除了结算阶段，一些其他地方也会用到结算函数，这是其一
//Player类自己的函数依赖于自己内部结构，这个没有任何问题，但是我想避免外部函数对其的依赖
public class ResolutionPhase : Singleton<ResolutionPhase>, Phase
{
    public void OnEnteringPhase()
    {
        EventPanelLogic.Instance.OpenEventPanel();
        Resolution();
        PrintEvent.Instance.ClearText();
        PrintEvent.Instance.PrintText();
        PrintResult_Debug();
        //结算阶段不等待玩家
        BattleManager.Instance.PhaseAdvance();
    }
    public void OnExitingPhase()
    {

    }
    //结算阶段，结算大家的行动
    //由于目前还没引入各种buff，所以结算阶段就先写在这里
    public void Resolution()
    {
        foreach (var player in PlayerManager.Instance.Players.Values)
        {
            player.Consume();
            player.CoolDownSword();
        }
        foreach (var player in PlayerManager.Instance.Players.Values)
        {
            player.Provoke();
        }
        foreach(var player in PlayerManager.Instance.Players.Values)
        {
            player.Comeon();
        }
        foreach(var player in PlayerManager.Instance.Players.Values)
        {
            player.Supply();
            player.Attack();
        }
        PrintResult_Debug();
    }
    public void PrintResult_Debug()
    {
        if (BattleManager.Instance.Turn.Value == 1)
        {
            Log.PrintSpecificPropertiesInDictionary(PlayerManager.Instance.Players, 
                new string[] {"ID_inGame", "status"},"Log/InGame/PlayerStatus.txt");
            Log.PrintNestedPropertyInDictionary(PlayerManager.Instance.Players,
                "action", "Log/InGame/PlayerAction.txt");
        }
        else
        {
            Log.PrintSpecificPropertiesInDictionary(PlayerManager.Instance.Players,
                new string[] { "ID_inGame", "status" }, "Log/InGame/PlayerStatus.txt", false);
            Log.PrintNestedPropertyInDictionary(PlayerManager.Instance.Players,
                "action", "Log/InGame/PlayerAction.txt",false);
        }

    }
}
