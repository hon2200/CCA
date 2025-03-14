using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//没有实现的功能：Restore of Swords，需要调用Attack History
public class Status : MonoBehaviour
{
    //需要知道Player的Move：资源的消耗在这里直接结算，资源的补给在大转盘上结算，因为涉及到过来
    public Move move;
    //需要知道Player的instant
    public Instant instant;
    //四个状态值
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
                    //懒的写default了，这之后都不写了罢
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
                        bullet -= 0;//激光炮的消耗需要在大转盘里结算
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
