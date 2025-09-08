using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Emotion : ObservableAttribute<float>
{
    public EmotionType emotionType;
    public void Set(float amount) => SetValue(amount, "Set");
    public void ChangeBy(float amount) => SetValue(Value + amount, "Change");
    public void ChangeTo(float amount) => SetValue(amount, "Change");

    public List<int> GetTendency()
    {
        EmotionDataBase.Instance.EmotionDictionary.TryGetValue(emotionType, out var emotionDefine);
        if (emotionDefine != null)
            return emotionDefine.ActionTendency;
        else
        {
            Debug.Assert(false, "Can't find such an emotion");
            return null;
        }
    }
}


public enum EmotionType
{
    Angry = 1,
    Peaceful = 2,
    Afraid = 3,
}