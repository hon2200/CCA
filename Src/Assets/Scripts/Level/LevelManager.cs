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
    public GameObject VictoryPanel;
    public GameObject AdvanceButton;
    public GameObject DefeatPanel;
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
            AdvanceButton.SetActive(true);
            AudioManager.Instance.VictoryAudioPlay();
        };
        BattleManager.Instance.OnStartGame += (string message) =>
        {
            if(message == "Level")
            {
                VictoryPanel.SetActive(false);
                DefeatPanel.SetActive(false);
                AdvanceButton.SetActive(false);
            }
        };
        BattleManager.Instance.OnNewWave += () =>
        {
            AdvanceButton.SetActive(false);
        };
    }
    public LevelDefine GetCurrentLevel()
    {
        LevelDataBase.Instance.LevelDictionary.TryGetValue(Level.ID, out var levelDefine);
        return levelDefine;
    }
    public void Advance()
    {
        Level.Advance();
    }
    public void Backward()
    {
        Level.Backward();
        BattleManager.Instance.OnStartGame?.Invoke("Level");
    }
    //重新开始
    public void Restart()
    {
        Level.FirstWave();
        BattleManager.Instance.OnStartGame?.Invoke("Level");
    }
}
