using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Move������player���ж���player������һ�����ж���ÿһ����������������
//catagory:�ж�����:1.������2.�ӵ��๥����3.�����๥����4.������5.������6.����
//move:�ж����:�������Read in Moves��ʵ���е�����д
//at:�ж�Ŀ��
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




