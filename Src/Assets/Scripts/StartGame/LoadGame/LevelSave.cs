using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelSave : MonoBehaviour
{
    //��0�£�����-����boss��1���ΰ��٣�2���ɾ���3����ͷȮ��
    public static int chapter = 0;
    //ÿ�¸����ؿ�
    public static int level = 1;
    //������Ĺ��жನ�����磬���Ĺأ�����أ�����Щ�ؿ���wave=1,2,3...��������wave=0
    public static int wave = 0;
    public static string LevelPath = "Assets/LevelFiles/LevelSavings/LevelSavings.json";
    //�ȼ�ª�ؼ�һ��
    public static int defeated = 0;
}

public class LevelSaving
{
    public string name;
    public int currentChapter = 0;
    public int currentLevel = 1;
    public int currentWave = 0;
}
