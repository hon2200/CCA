using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ReadinMove : MonoBehaviour
{
    //这个代码用于把按钮的按下信息送给Player1。
    //这里的Move_of_Player1 是手动赋值的，需要注意。
    //此处用到了按钮的名字信息需要注意，因为如果不这样的话没办法写在同一个脚本里，这样会很丑而且很麻烦。
    public Move move1;
    //挂在按钮上面
    public void SignalToMove()
    {
        if (gameObject.name == "Bullet")
        {
            move1.catagory.Add(1);
            move1.move.Add(1);
            move1.at.Add(0);
        }
        else if (gameObject.name == "Sword")
        {
            move1.catagory.Add(1);
            move1.move.Add(2);
            move1.at.Add(0);
        }


        else if (gameObject.name == "Shoot")
        {
            move1.catagory.Add(2);
            move1.move.Add(1);
            //这里留个at的位置给后面选择目标来加。
        }
        else if (gameObject.name == "Double Shoot")
        {
            move1.catagory.Add(2);
            move1.move.Add(2);
        }
        else if (gameObject.name == "Lasergun")
        {
            move1.catagory.Add(2);
            move1.move.Add(3);
        }
        else if (gameObject.name == "Lasercannon")
        {
            move1.catagory.Add(2);
            move1.move.Add(4);
        }
        else if (gameObject.name == "RPG")
        {
            move1.catagory.Add(2);
            move1.move.Add(5);
        }
        else if (gameObject.name == "Double RPG")
        {
            move1.catagory.Add(2);
            move1.move.Add(6);
        }
        else if (gameObject.name == "Nuclear Bomb")
        {
            move1.catagory.Add(2);
            move1.move.Add(7);
        }


        else if (gameObject.name == "Stab")
        {
            move1.catagory.Add(3);
            move1.move.Add(1);
        }
        else if (gameObject.name == "Chop")
        {
            move1.catagory.Add(3);
            move1.move.Add(2);
        }
        else if (gameObject.name == "Lightsaber Stab")
        {
            move1.catagory.Add(3);
            move1.move.Add(3);
        }

        else if (gameObject.name == "Lightsaber Chop")
        {
            move1.catagory.Add(3);
            move1.move.Add(4);
        }
        else if (gameObject.name == "Double Chop")
        {
            move1.catagory.Add(3);
            move1.move.Add(5);
        }
        else if (gameObject.name == "Ghost Chop")
        {
            move1.catagory.Add(3);
            move1.move.Add(6);
        }
        else if (gameObject.name == "Nock-end Chop")
        {
            move1.catagory.Add(3);
            move1.move.Add(7);
        }


        else if (gameObject.name == "Block")
        {
            move1.catagory.Add(4);
            move1.move.Add(1);
            move1.at.Add(0);
        }
        else if (gameObject.name == "Double Block")
        {
            move1.catagory.Add(4);
            move1.move.Add(2);
            move1.at.Add(0);
        }
        else if(gameObject.name == "Heavy Block")
        {
            move1.catagory.Add(4);
            move1.move.Add(3);
            move1.at.Add(0);
        }
        else if (gameObject.name == "Giant Shield")
        {
            move1.catagory.Add(4);
            move1.move.Add(4);
            move1.at.Add(0);
        }


        else if (gameObject.name == "Rebounce")
        {
            move1.catagory.Add(5);
            move1.move.Add(1);
            move1.at.Add(0);
        }
        else if (gameObject.name == "Brandish")
        {
            move1.catagory.Add(5);
            move1.move.Add(2);
            move1.at.Add(0);
        }
        else if (gameObject.name == "Disarm")
        {
            move1.catagory.Add(5);
            move1.move.Add(3);
            move1.at.Add(0);
        }
        else if (gameObject.name == "Specular Reflection")
        {
            move1.catagory.Add(5);
            move1.move.Add(4);
            move1.at.Add(0);
        }
        else if (gameObject.name == "Provoke")
        {
            move1.catagory.Add(6);
            move1.move.Add(1);
        }

        else if (gameObject.name == "Come on")
        {
            move1.catagory.Add(6);
            move1.move.Add(2);
            move1.at.Add(0);
        }
    }
    //挂在Player的status上面,选择目标时button的onclick响应
    //这里选择目标要用到Player的编号
    public void SignalToAt()
    {
        int playerNumber = gameObject.transform.parent.GetComponent<PlayerNumber>().number;
        move1.at.Add(playerNumber);
    }
}
