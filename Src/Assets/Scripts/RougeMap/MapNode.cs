using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class MapNode
{
    public string id;
    public string name;
    //节点值，一行的节点值必须是0
    public int value;                                                                                                                                                                                                                                                                    
    //生成这个节点的概率
    public int possibility;
    //图像
    public Sprite image;
}

