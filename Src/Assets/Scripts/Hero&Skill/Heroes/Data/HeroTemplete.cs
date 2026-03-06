using UnityEngine;

// ScriptableObject for hero display data; use with HeroLiberary for lookup by ID.
[CreateAssetMenu(fileName = "HeroTemplete", menuName = "ScriptableObjects/HeroTemplete", order = 2)]
public class HeroTemplete : ScriptableObject
{
    public string ID;
    public Sprite image;
}
