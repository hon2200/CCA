using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//��������װ����AI�Ķ���
//Json�ļ�->AIDataBase->ÿ��Player��PlayerDefine��
public class AIDataBase : MonoSingleton<AIDataBase>
{
    public string path;
    public Dictionary<string, AIDefine> AIDictionary { get; set; }
    public void Start()
    {
        LoadingAI();
    }
    //�����������
    public void LoadingAI()
    {
        path = Path.Combine(Application.dataPath, "Common/Tables/Data/Levels/AI.json");
        AIDictionary = JsonLoader.DeserializeObject<Dictionary<string, AIDefine>>(path);
        //��ӡ�ж��ൽ��־
        MyLog.PrintLoadedDictionary(AIDictionary,"Log/Loading/AILog.txt");
    }
}
