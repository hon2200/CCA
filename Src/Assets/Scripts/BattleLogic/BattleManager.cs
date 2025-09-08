using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

class BattleManager: MonoSingleton<BattleManager>
{
    public Action OnDefeated;
    public Action OnWinning;
    //已经创建好游戏玩家的情况下，要重新开始需要进行的内容
    public Action OnRestarting;
    //新的一波来袭
    public Action OnNewWave;
    public TextMeshPro Text;
    public TurnAttribute Turn { get; private set; }
    private List<Phase> PhaseList { get; set; }
    private int CurrentPhaseIndex { get; set; }
    //初始化PhaseList
    private void Start()
    {
        PhaseList = new();
        PhaseList.Add(StartPhase.Instance);
        PhaseList.Add(ActionPhase.Instance);
        PhaseList.Add(ResolutionPhase.Instance);
        PhaseList.Add(EndPhase.Instance);
        Turn = new();
        Turn.OnValueChanged += (oldVal, newVal, message) =>
        {
            Text.text = "Turn" + Turn.Value.ToString();
        };
        OnRestarting += () =>
        {
            StartGame();
        };
        OnNewWave += () =>
        {
            CurrentPhaseIndex = 0;
            StartPhase.Instance.OnEnteringPhase();
        };
    }
    public void StartGame()
    {
        CurrentPhaseIndex = 0;
        Turn.Clear();
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