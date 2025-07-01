using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class BattleManager: MonoSingleton<BattleManager>
{
    public TurnAttribute Turn;
    public List<Phase> PhaseList;
    public int CurrentPhaseIndex;
    //玩家若未确定好行动，则保持等待
    private bool Ready = false;
    //初始化PhaseList
    private void Awake()
    {
        PhaseList = new();
        PhaseList.Add(StartPhase.Instance);
        PhaseList.Add(ActionPhase.Instance);
        PhaseList.Add(ResolutionPhase.Instance);
        PhaseList.Add(EndPhase.Instance);
    }
    public void StartGame()
    {
        CurrentPhaseIndex = 0;
        Turn = new();
        StartPhase.Instance.OnEnteringPhase();
    }

    public void PhaseAdvance()
    {
        PhaseList[CurrentPhaseIndex].OnExitingPhase();
        CurrentPhaseIndex++;
        if (CurrentPhaseIndex >= PhaseList.Count)
            CurrentPhaseIndex = CurrentPhaseIndex % PhaseList.Count;
        PhaseList[CurrentPhaseIndex].OnEnteringPhase();
    }
    public void ReadyUpAll()
    {
        foreach(var player in PlayerManager.Instance.Players)
        {
            player.Value.isReady.ReadyUp();
        }
    }
    public void CheckReady()
    {
        bool allReady = true;
        foreach(var player in PlayerManager.Instance.Players)
        {
            if (player.Value.isReady.Value == false)
                allReady = false;
        }
        if(allReady)
        {
            PhaseAdvance();
        }
    }
}