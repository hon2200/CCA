using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LevelAttribute : ObservableString
{
    public string ID { get; private set; }
    public int Wave { get; private set; }
    public string Name { get; private set; }
    private string savePath = "Assets/Common/Tables/Data/CurrentLevel.json";
    public LevelAttribute()
    {
        LoadLevel();
        base.Value = new("");
        SetLevel();
    }
    public void SetLevel() => SetValue(ID + " " + Name + "wave" + (Wave + 1), "SetLevel");
    
    public void Advance()
    {
        LevelDataBase.Instance.LevelDictionary.TryGetValue(ID, out var levelData);
        //如果还没过关
        if (Wave < levelData.EnemyList.Count - 1)
        {
            Wave++;
            SetLevel();
            LevelManager.Instance.AdvanceButton.SetActive(false);
            PlayerManager.Instance.CreateCurrentLevelWave();
            BattleManager.Instance.OnNewWave?.Invoke();
        }

        //过关大吉
        else
        {
            LevelDataBase.Instance.LevelDictionary.TryGetValue(levelData.NextLevel, out var newLevelData);
            ID = newLevelData.ID;
            Wave = 0;
            Name = newLevelData.Name;
            SetLevel();
            PlayerManager.Instance.CreateCurrentLevelWave();
            BattleManager.Instance.OnRestarting?.Invoke();
        }
        SaveLevel ();
        
    }
    public void Backward()
    {
        LevelDataBase.Instance.LevelDictionary.TryGetValue(ID, out var levelData);
        ID = levelData.PreviousLevel;
        Wave = 0;
        LevelDataBase.Instance.LevelDictionary.TryGetValue(levelData.PreviousLevel, out var newLevelData);
        Name = newLevelData.Name;
        SetLevel();
        SaveLevel();
    }
    // 加载关卡数据
    public void LoadLevel()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            LevelSaveData data = JsonUtility.FromJson<LevelSaveData>(json);

            ID = data.LevelID;
            Wave = data.WaveIndex;
            Name = data.LevelName;

            Debug.Log("关卡数据加载成功");
        }
        else
        {
            Debug.LogWarning("未找到存档文件，使用默认关卡数据");
            SetDefaultLevel();
        }
    }

    // 保存关卡数据
    public void SaveLevel()
    {
        LevelSaveData saveData = new LevelSaveData
        {
            LevelID = ID,
            WaveIndex = 0,
            LevelName = Name
        };

        string json = JsonUtility.ToJson(saveData, true);

        // 确保目录存在
        string directory = Path.GetDirectoryName(savePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(savePath, json);
        Debug.Log("关卡数据保存成功");
    }
    // 设置默认关卡数据
    private void SetDefaultLevel()
    {
        LevelDataBase.Instance.LevelDictionary.TryGetValue("1-1", out var levelData);
        ID = levelData.ID;
        Name = levelData.Name;
        Wave = 0;
    }

    // 保存数据类
    [System.Serializable]
    private class LevelSaveData
    {
        public string LevelID;
        public int WaveIndex;
        public string LevelName;
    }
}
