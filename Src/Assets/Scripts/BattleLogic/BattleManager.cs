using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using static UnityEditor.ShaderData;

class BattleManager: MonoSingleton<BattleManager>
{
    public bool isRunning = false;
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
        PhaseList.Add(new StartPhase());
        PhaseList.Add(new ActionPhase());
        PhaseList.Add(new ChasePhase());
        PhaseList.Add(new ResolutionPhase());
        PhaseList.Add(new EndPhase());
        Turn = new();
        Turn.OnValueChanged += (oldVal, newVal, message) =>
        {
            Text.text = "Turn" + Turn.Value.ToString();
        };
        OnNewWave += () =>
        {
            PlayerManager.Instance.CreateCurrentLevelWave();
            StartRunPhase();
        };

        OnStartGame += (string message) =>
        {
            Turn.Clear();
            PlayerManager.Instance.NextPlayerID = 1;
            if (message == "Hero")
                PlayerManager.Instance.CreatingPlayers_BasedOnGameSetting_Heroes();
            else if (message == "Level")
                PlayerManager.Instance.CreateCurrentLevelWave();
            StartRunPhase();
        };
        OnDefeated += () => isRunning = false;
        OnWinning += () => isRunning = false;
    }
    //这个函数好像只给那个按钮用
    public void StartGame(string Type)
    {
        OnStartGame?.Invoke(Type);
    }
    public void StartRunPhase()
    {
        CurrentPhaseIndex = 0;
        isRunning = true;
        StartCoroutine(GameLoop());
    }
    private IEnumerator RunPhase(Phase phase)
    {

        // 1. phase logic
        phase.OnEnteringPhase();


        // 2. let skills / UI register choices
        phase.EnteringCallSkills();

        // 3. wait for player decisions
        yield return WaitForChoices();

        // 4. exit
        phase.OnExitingPhase();
        phase.ExitingCallSkills();
        yield return null;
    }

    private IEnumerator WaitForChoices()
    {
        while (!ChoiceBarrier.Instance.IsComplete)
            yield return null;
    }
    private IEnumerator GameLoop()
    {
        while (isRunning)
        {
            var phase = PhaseList[CurrentPhaseIndex];
            yield return RunPhase(phase);
            CurrentPhaseIndex = (CurrentPhaseIndex + 1) % PhaseList.Count;
        }
    }
}