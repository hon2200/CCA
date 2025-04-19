using Common.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.TextCore.Text;

public class AIMessageCreat:Singleton<AIMessageCreat>
{//后续关卡或规则限定出现则拓展
    

    List<int> AIID= new List<int>();
    public void CustomizeCreate(List<CharacterSet> characters)
    {
        foreach (var kv in characters)
        {
            for (int i = 1; i <= kv.CurrentCount; i++)
            {
                int AIId;
                do
                {
                    AIId = -UnityEngine.Random.Range(1, 10001);
                } while (AIID.Contains(AIId));
                AIID.Add(AIId);
                Character cha = new Character()
                {
                    character = new CharacterDefine()
                    {
                        ID = AIId,
                        Name = "人机" + AIId,
                        Description = null,
                    }
                };
                CharacterManager.Instance.Characters.Add(AIId, cha);
                Room.Instance.Add(AIId, kv);
            }
        }
    }
}
