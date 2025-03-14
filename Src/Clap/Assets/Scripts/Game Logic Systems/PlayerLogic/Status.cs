using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//û��ʵ�ֵĹ��ܣ�Restore of Swords����Ҫ����Attack History
public class Status : MonoBehaviour
{
    //��Ҫ֪��Player��Move����Դ������������ֱ�ӽ��㣬��Դ�Ĳ����ڴ�ת���Ͻ��㣬��Ϊ�漰������
    public Move move;
    //��Ҫ֪��Player��instant
    public Instant instant;
    //�ĸ�״ֵ̬
    public int HP;
    public int bullet;
    public int sword;
    public int swordinCD;
    public void Consume(int order)
    {
        CoolDownSword(order);
        ConsumeBullets(order);
    }
    public void CoolDownSword(int order)
    {
        switch (move.catagory[order])
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                switch (move.move[order])
                {
                    case 1:
                        swordinCD++;
                        break;
                    case 2:
                        swordinCD += 2;
                        break;
                    case 3:
                        swordinCD += 3;
                        break;
                    case 4:
                        swordinCD += 5;
                        break;
                    case 5:
                        swordinCD += 4;
                        break;
                    case 6:
                        swordinCD += 6;
                        break;
                    case 7:
                        swordinCD += 10;
                        break;
                    //����дdefault�ˣ���֮�󶼲�д�˰�
                }
                break;
            case 4:
                break;
            case 5:
                switch (move.move[order])
                {
                    case 2:
                        swordinCD++;
                        break;
                    case 3:
                        swordinCD += 3;
                        break;
                    case 4:
                        swordinCD += 3;
                        break;
                }
                break;
            case 6:
                break;
        }

    }
    public void ConsumeBullets(int order)
    {
        switch (move.catagory[order])
        {
            case 1:
                break;
            case 2:
                switch (move.move[order])
                {
                    case 1:
                        bullet--;
                        break;
                    case 2:
                        bullet -= 2;
                        break;
                    case 3:
                        bullet -= 2;
                        break;
                    case 4:
                        bullet -= 0;//�����ڵ�������Ҫ�ڴ�ת�������
                        break;
                    case 5:
                        bullet -= 3;
                        break;
                    case 6:
                        bullet -= 6;
                        break;
                    case 7:
                        bullet -= 5;
                        break;
                }
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                switch (move.move[order])
                {
                    case 1:
                        bullet--;
                        break;
                }
                break;
            case 6:
                break;
        }
    }
}
