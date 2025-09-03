using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HeroDataBase : Singleton<HeroDataBase>
{
    public string path;
    // ����ֵ䣬��������������ݿ�
    public Dictionary<int, HeroDefine> HeroDictionary { get; set; }
    //�����������
    public void LoadingHeroes()
    {
        path = Path.Combine(Application.dataPath, "Common/Tables/Data/Hero/Hero.json");
        HeroDictionary = JsonLoader.DeserializeObject<Dictionary<int, HeroDefine>>(path);
        //��ӡ�ж��ൽ��־
        Log.PrintLoadedDictionary(HeroDictionary,"Log/Loading/Heroes.txt");
    }
}
