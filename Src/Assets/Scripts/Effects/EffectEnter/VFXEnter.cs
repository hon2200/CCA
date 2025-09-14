using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public static class VFXEnter
{
    public static void Shot(this GameObject game,string name)
    {
        Debug.Log($"use{name}");
    }
}
