using Common.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : Singleton<CharacterManager>
{
    public Dictionary<int, Character> Characters = new Dictionary<int, Character>();

    public void Init()
    {

    }


}
