using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//��������װ����AI�Ķ���
//Json�ļ�->AIDataBase->ÿ��Player��PlayerDefine��
public class AIDataBase : Singleton<AIDataBase>
{
    public string path;
    public Dictionary<int, AIDefine> AIDictionary { get; set; }
    //�����������
    public void LoadingAI()
    {
        path = Path.Combine(Application.dataPath, "Common/Tables/Data/Levels/AI.json");
        AIDictionary = JsonLoader.DeserializeObject<Dictionary<int, AIDefine>>(path);
        //��ӡ�ж��ൽ��־
        Log.PrintLoadedDictionary(AIDictionary,"Log/Loading/AILog.txt");
    }
}
