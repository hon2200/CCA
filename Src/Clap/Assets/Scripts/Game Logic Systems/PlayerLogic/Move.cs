using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Move是描述player的行动，player可能有一连串行动，每一个用三个变量描述
//catagory:行动类型:1.补给，2.子弹类攻击，3.刀剑类攻击，4.防御，5.反弹，6.其他
//move:行动编号:具体见“Read in Moves”实在有点懒的写
//at:行动目标
public class Move : MonoBehaviour
{
    public List<int> catagory = new();
    public List<int> move = new();
    public List<int> at = new();
    public void ClearMoves()
    {
        catagory.Clear();
        move.Clear();
        at.Clear();
    }
}




