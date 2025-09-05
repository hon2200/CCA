using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//目前英雄模式的AI没有转入这些逻辑，这里的AI专门是那些有情感的家伙
public class AIPlayer : Player
{
    public AIDefine AIDefine { get; set; }
    public CharacterDefine CharacterDefine { get; set; }
    public void Initialize(int ID_inGame,AIDefine aIDefine)
    {
        HeroDataBase.Instance.HeroDictionary.TryGetValue("Blank", out var heroDefine);
        base.Initialize(ID_inGame, PlayerType.AI, heroDefine);
        //重新修改最大生命值
        this.status = new(aIDefine.maxHP, new() { 0, 0, 0 });
        //赋值
        AIDefine = aIDefine;
        CharacterDataBase.Instance.CharacterDictionary.TryGetValue(aIDefine.ID, out var characterDefine);
        CharacterDefine = characterDefine;
    }
}
