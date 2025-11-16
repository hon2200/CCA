using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class LevelManager : MonoSingleton<LevelManager>
{
    public LevelAttribute Level;
    public TextMeshPro Text;

    public GameObject DefeatPanel;
    public GameObject WinningPanel;
    
    public Button AdvanceButton; // AdvanceButton should be within the winning panel
    public Button RestartButton; // RestartButton should be within the defeat panel
    public Button BackwarButton;
    public void Start()
    {
        Level = new();
        Level.OnValueChanged += (oldVal, newVal, message) =>
        {
            Text.text = Level.Value;
        };
        Text.text = Level.Value;
        BattleManager.Instance.OnDefeated += () =>
        {
            DefeatPanel.SetActive(true);
        };
        BattleManager.Instance.OnWinning += () =>
        {
            if (Level.IsLastWave())
            {
                AdvanceButton.gameObject.SetActive(true);
                AudioManager.Instance.VictoryAudioPlay();
                WinningPanel.SetActive(true);
            }
            else//EnterNextWave, startGame right away
            {
                Level.NewWave();
                BattleManager.Instance.OnNewWave.Invoke();
            }
        };
        RestartButton.onClick.AddListener(Restart);
        RestartButton.onClick.AddListener(() => { DefeatPanel.SetActive(false); });
        BackwarButton.onClick.AddListener(Backward);
        AdvanceButton.onClick.AddListener(OpenBattleRewards); //Display unlockedSkill
    }
    public void OpenBattleRewards()
    {
        WinningPanel.SetActive(false);
        BattleRewardManager.Instance.OpenBattleReward();
    }
    public LevelDefine GetCurrentLevel()
    {
        LevelDataBase.Instance.LevelDictionary.TryGetValue(Level.ID, out var levelDefine);
        return levelDefine;
    }
    public LevelDefine GetPreviousLevel()
    {
        LevelDataBase.Instance.LevelDictionary.TryGetValue(GetCurrentLevel().PreviousLevel, out var levelDefine);
        return levelDefine;
    }
    public LevelDefine GetNextLevel()
    {
        LevelDataBase.Instance.LevelDictionary.TryGetValue(GetCurrentLevel().NextLevel, out var levelDefine);
        return levelDefine;
    }
    public void Advance()
    {
        Level.Advance();
        LevelStart();
    }
    public void Backward()
    {
        Level.Backward();
        LevelStart();
    }
    //重新开始
    public void Restart()
    {
        Level.FirstWave();
        LevelStart();
    }
    //新的一关正式开始
    public void LevelStart()
    {
        AudioManager.Instance.SceneAudioPlay();
        CardViewSystem.Instance.Show(GetCurrentLevel());
    }
}