using UnityEngine;

// ScriptableObject for enemy display data; use with EnemyLiberary for lookup by ID.
[CreateAssetMenu(fileName = "EnemyTemplete", menuName = "ScriptableObjects/EnemyTemplete", order = 3)]
public class EnemyTemplete : ScriptableObject
{
    public string ID;
    public Sprite image;
}
