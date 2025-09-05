using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelSave : MonoBehaviour
{
    //第0章：序章-剑鬼boss，1，治安官，2，飞镜，3，三头犬。
    public static int chapter = 0;
    //每章各个关卡
    public static int level = 1;
    //极特殊的关有多波，比如，第四关，第五关，在这些关卡，wave=1,2,3...，其他关wave=0
    public static int wave = 0;
    public static string LevelPath = "Assets/LevelFiles/LevelSavings/LevelSavings.json";
    //先简陋地记一下
    public static int defeated = 0;
}

public class LevelSaving
{
    public string name;
    public int currentChapter = 0;
    public int currentLevel = 1;
    public int currentWave = 0;
}
