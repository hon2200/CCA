using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ReadinMove : MonoBehaviour
{
    //����������ڰѰ�ť�İ�����Ϣ�͸�Player1��
    //�����Move_of_Player1 ���ֶ���ֵ�ģ���Ҫע�⡣
    //�˴��õ��˰�ť��������Ϣ��Ҫע�⣬��Ϊ����������Ļ�û�취д��ͬһ���ű��������ܳ���Һ��鷳��
    public Move move1;
    //���ڰ�ť����
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
            //��������at��λ�ø�����ѡ��Ŀ�����ӡ�
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
    //����Player��status����,ѡ��Ŀ��ʱbutton��onclick��Ӧ
    //����ѡ��Ŀ��Ҫ�õ�Player�ı��
    public void SignalToAt()
    {
        int playerNumber = gameObject.transform.parent.GetComponent<PlayerNumber>().number;
        move1.at.Add(playerNumber);
    }
}
