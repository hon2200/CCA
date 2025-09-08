using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//DataBase�ܶ࣬���Ǵ������һ�����������Լ�
public class HeroDataBase : MonoSingleton<HeroDataBase>
{
    public string path;
    // ����ֵ䣬��������������ݿ�
    public Dictionary<string, HeroDefine> HeroDictionary { get; set; }
    private new void Awake()
    {
        base.Awake();
        LoadingHeroes();
    }
    //�����������
    public void LoadingHeroes()
    {
        path = Path.Combine(Application.dataPath, "Common/Tables/Data/Hero/Hero.json");
        HeroDictionary = JsonLoader.DeserializeObject<Dictionary<string, HeroDefine>>(path);
        //��ӡ�ж��ൽ��־
        MyLog.PrintLoadedDictionary(HeroDictionary,"Log/Loading/Heroes.txt");
    }
}
