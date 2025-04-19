using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class User:Singleton<User>
{
    public int ID { get; set; }
    public string Name { get; set; }

    public void UserSet(int Id,string name)
    {
        this.ID = Id;
        this.Name = name;
        Debug.Log(ID+":"+ Name);
        Character cha=new Character()
        {
            character = new CharacterDefine()
            {
                ID = Id,
                Name = name,
                Description = null
            }
        };
        CharacterManager.Instance.Characters.Add(Id, cha);
    }



}
