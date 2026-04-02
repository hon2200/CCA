using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoSingleton<TutorialManager>
{
    public TutorialAttribute tutorial;
    public TextMeshPro Text;

    public Button AdvanceButton; // AdvanceButton should be within the winning panel
    public Button RestartButton; // RestartButton should be within the defeat panel
    public void Start()
    {
        tutorial = new();
        tutorial.OnValueChanged += (oldVal, newVal, message) =>
        {
            Text.text = tutorial.Value;
        };
        Text.text = tutorial.Value;
        BattleManager.Instance.OnDefeated += () =>
        {
            RestartButton.gameObject.SetActive(true);
        };
        BattleManager.Instance.OnWinning += () =>
        {
            AdvanceButton.gameObject.SetActive(true);
        };
        RestartButton.onClick.AddListener(Restart);
        AdvanceButton.onClick.AddListener(Advance);

        LevelStart();
    }
    public TutorialDefine GetCurrentLevel()
    {
        TutorialDatabase.Instance.TutorialDictionary.TryGetValue(tutorial.ID, out var levelDefine);
        return levelDefine;
    }
    public TutorialDefine GetPreviousLevel()
    {
        TutorialDatabase.Instance.TutorialDictionary.TryGetValue(GetCurrentLevel().PreviousLevel, out var levelDefine);
        return levelDefine;
    }
    public TutorialDefine GetNextLevel()
    {
        TutorialDatabase.Instance.TutorialDictionary.TryGetValue(GetCurrentLevel().NextLevel, out var levelDefine);
        return levelDefine;
    }
    public void Advance()
    {
        if(tutorial.Advance())
        {
            AdvanceButton.gameObject.SetActive(false);
            LevelStart();
        }
        else
        {
            Debug.Log("Tutorial Completed");
        }
    }
    public void Backward()
    {
        tutorial.Backward();
        LevelStart();
    }
    //重新开始
    public void Restart()
    {
        tutorial.SetLevel();
        RestartButton.gameObject.SetActive(false);
        LevelStart();
    }
    //新的一关正式开始
    public void LevelStart()
    {
        //AudioManager.Instance.SceneAudioPlay();
        //CardViewSystem.Instance.Show(GetCurrentLevel());
        PlayerManager.Instance.CreatingPlayers_BasedOnLevels(GetCurrentLevel());
        BattleManager.Instance.StartGame();
    }
}