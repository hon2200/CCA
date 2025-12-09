using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

class BattleManager: MonoSingleton<BattleManager>
{
    public Action OnDefeated { get; set; }
    public Action OnWinning { get; set; }
    //已经创建好游戏玩家的情况下，要重新开始需要进行的内容
    public Action<string> OnStartGame { get; set; }
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
        PhaseList.Add(ChasePhase.Instance);
        PhaseList.Add(PreResolutionPhase.Instance);
        PhaseList.Add(ResolutionPhase.Instance);
        PhaseList.Add(EndPhase.Instance);
        Turn = new();
        Turn.OnValueChanged += (oldVal, newVal, message) =>
        {
            Text.text = "Turn" + Turn.Value.ToString();
        };
        OnNewWave += () =>
        {
            CurrentPhaseIndex = 0;
            PlayerManager.Instance.CreateCurrentLevelWave();
            StartPhase.Instance.OnEnteringPhase();
        };

        OnStartGame += (string message) =>
        {
            CurrentPhaseIndex = 0;
            Turn.Clear();
            PlayerManager.Instance.NextPlayerID = 1;
            if (message == "Hero")
                PlayerManager.Instance.CreatingPlayers_BasedOnGameSetting_Heroes();
            else if (message == "Level")
                PlayerManager.Instance.CreateCurrentLevelWave();
            StartPhase.Instance.OnEnteringPhase();
        };
    }
    //这个函数好像只给那个按钮用
    public void StartGame(string Type)
    {
        OnStartGame?.Invoke(Type);
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