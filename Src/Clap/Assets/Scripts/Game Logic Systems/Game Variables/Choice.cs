using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Choice : MonoBehaviour
{
    public List<bool> choices;
    public Choice(List<bool> choices)
    {
        this.choices = choices;
    }
    public Choice()
    {
        choices = new List<bool>();
        for (int i = 0; i < 26; i++)
            choices.Add(true);
    }
}
