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
    public string savePath = Path.Combine(Application.streamingAssetsPath, "Common/Tables/Data/CurrentLevel.json");
    public LevelAttribute()
    {
        LoadLevel();
        SetLevel();
    }
    public void SetLevel() => SetValue(ID + " " + Name + "wave" + (Wave + 1), "SetLevel");
    public void Advance()
    {
        LevelDataBase.Instance.LevelDictionary.TryGetValue(ID, out var levelData);
        //过关大吉
        LevelDataBase.Instance.LevelDictionary.TryGetValue(levelData.NextLevel, out var newLevelData);
        ID = newLevelData.ID;
        Wave = 0;
        Name = newLevelData.Name;
        SetLevel();
        SaveLevel();
        Debug.Log("New Level");
    }
    public void NewWave()
    {
        Wave++;
        SetLevel();
        Debug.Log("New Wave");
    }
    public bool IsLastWave()
    {
        LevelDataBase.Instance.LevelDictionary.TryGetValue(ID, out var levelData);
        if (Wave < levelData.EnemyList.Count - 1)
            return false;
        else
            return true;
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
    public void FirstWave()
    {
        Wave = 0;
        SetLevel();
        SaveLevel();
    }
    // 加载关卡数据
    public void LoadLevel()
    {
        if (File.Exists(savePath))
        {
            LevelSaveData data = JsonUtility.FromJson<LevelSaveData>(File.ReadAllText(savePath));
            ID = data.ID;
            Wave = data.Wave;
            Name = data.Name;
            return;
        }
        SetDefaultLevel();
    }
    // 保存关卡数据
    public void SaveLevel()
    {
        LevelSaveData data = new LevelSaveData
        {
            ID = this.ID,
            Wave = this.Wave,
            Name = this.Name
        };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }
    // 设置默认关卡数据
    private void SetDefaultLevel()
    {
        LevelDataBase.Instance.LevelDictionary.TryGetValue("1-1", out var levelData);
        ID = levelData.ID;
        Name = levelData.Name;
        Wave = 0;
    }

    [Serializable]
    public class LevelSaveData
    {
        public string ID;
        public int Wave;
        public string Name;
    }

}
