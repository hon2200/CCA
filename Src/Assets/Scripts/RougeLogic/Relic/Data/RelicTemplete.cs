using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RelicTemplete", menuName = "ScriptableObjects/RelicTemplete", order = 1)]
public class RelicTemplete : ScriptableObject
{
    public string ID;
    public Sprite Image;
    public List<int> counts = new List<int>();
}
