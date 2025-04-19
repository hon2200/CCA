using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class Attribute : MonoBehaviour
{
    public TextMeshProUGUI HP;
    public TextMeshProUGUI Bullet;
    public TextMeshProUGUI Sward;

    public pos root;

    public void Start()
    {
        Rule.OnRoundChanged += UpdateData;
        UpdateData();
    }
    public void UpdateData()
    {
        Character[] characters = root.GetComponentsInChildren<Character>(true);
        Character targetCharacter = characters.FirstOrDefault(c => c.ID == User.Instance.ID);
        HP.text = targetCharacter.Hp.text;
        Bullet.text = targetCharacter.Buttles.text;
        Sward.text = targetCharacter.Swords.text;
    }
}
