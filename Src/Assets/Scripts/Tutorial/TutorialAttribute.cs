using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TutorialAttribute : ObservableString
{
    public string ID { get; private set; }
    public string Name { get; private set; }
    public string savePath = Path.Combine(Application.streamingAssetsPath, "Common/Tables/Data/CurrentTutorial.json");
    public TutorialAttribute()
    {
        LoadLevel();
        SetLevel();
    }
    public void SetLevel() => SetValue(ID + " " + Name, "SetLevel");
    public bool Advance()
    {
        TutorialDatabase.Instance.TutorialDictionary.TryGetValue(ID, out var levelData);
        TutorialDatabase.Instance.TutorialDictionary.TryGetValue(levelData.NextLevel, out var newLevelData);
        if (newLevelData == null)
            return false;
        ID = newLevelData.ID;
        Name = newLevelData.Name;
        SetLevel();
        SaveLevel();
        return true;
    }
    public void Backward()
    {
        TutorialDatabase.Instance.TutorialDictionary.TryGetValue(ID, out var levelData);
        ID = levelData.PreviousLevel;
        TutorialDatabase.Instance.TutorialDictionary.TryGetValue(levelData.PreviousLevel, out var newLevelData);
        if(newLevelData == null) return;
        Name = newLevelData.Name;
        SetLevel();
        SaveLevel();
    }
    // 加载关卡数据
    public void LoadLevel()
    {
        if (File.Exists(savePath))
        {
            TutorialSaveData data = JsonUtility.FromJson<TutorialSaveData>(File.ReadAllText(savePath));
            ID = data.ID;
            Name = data.Name;
            return;
        }
        SetDefaultLevel();
    }
    // 保存关卡数据
    public void SaveLevel()
    {
        TutorialSaveData data = new TutorialSaveData
        {
            ID = this.ID,
            Name = this.Name
        };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }
    // 设置默认关卡数据
    private void SetDefaultLevel()
    {
        TutorialDatabase.Instance.TutorialDictionary.TryGetValue("1-1", out var levelData);
        ID = levelData.ID;
        Name = levelData.Name;
    }

    [Serializable]
    public class TutorialSaveData
    {
        public string ID;
        public string Name;
    }

}
