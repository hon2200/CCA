using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Photon.Realtime;

public class BattleManager : MonoSingleton<BattleManager>
{
    public static event Action<int, string> DataChanged;
    public static void Trigger(int id, string fieldName)
        => DataChanged?.Invoke(id, fieldName);

    public Dictionary<int, CharacterData> Characters = new Dictionary<int, CharacterData>();
    public Dictionary<int, Character> CharacterObject = new Dictionary<int, Character>();
    public void BattleEnter()
    {
        foreach (var character in Characters) 
        {
            CharacterManager.Instance.Characters.TryGetValue(character.Value.ID, out Character cha);
            GameObject prefab = Resources.Load<GameObject>("UI/character");
            GameObject go = Instantiate(prefab);
            Character ks = go.GetComponent<Character>();
            ks.Show((int)character.Value.HP, (int)character.Value.MaxHP, character.Value.BulletCount, character.Value.SwordCount, character.Value.SwordCD);
            ks.NewSet(cha.character.ID, cha.character.Name);
            BattleItem.Instance.Set(ks);
            CharacterObject.Add(character.Key, ks);
        }
    }
    private void OnEnable()
    {
        Rule.OnRoundChanged += Show;
    }

    private void OnDisable()
    {
        Rule.OnRoundChanged -= Show;
    }
    public void Show()
    {
        foreach (var kv in CharacterObject)
        {
            Characters.TryGetValue(kv.Key,out CharacterData character);
            kv.Value.Show((int)character.HP, (int)character.MaxHP, character.BulletCount, character.SwordCount, character.SwordCD);
        }
    }

    internal void NewBattle()
    {
        Characters.Clear();
    }
    public bool IsDead(int id)
    {

        foreach (var kv in Characters.Values)
        {
            if (kv.ID == id) return kv.IsDead;
        }
        return false;
    }
}
