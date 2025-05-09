using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class CardUIText : MonoBehaviour
{
    public string ID;
    [SerializeField]
    public SerializedDictionary<CardUITextName, TextMeshProUGUI> UIText;
    public void Initialize()
    {
        ActionDataBase.Instance.ActionDictionary.TryGetValue(ID, out var actionDefine);
        if(actionDefine != null)
            foreach (var text in UIText)
            {
                switch (text.Key)
                {
                    case CardUITextName.Consume:
                        foreach(var cost in actionDefine.Costs)
                        {
                            if (cost != 0)
                                text.Value.text = cost.ToString();
                        }
                        break;
                    case CardUITextName.Supply:
                        if (actionDefine is SupplyDefine supplyDefine)
                            foreach(var supply in supplyDefine.SupplyNumber)
                            {
                                if (supply != 0)
                                    text.Value.text = supply.ToString();
                            }
                        break;
                    case CardUITextName.Level:
                        if(actionDefine is AttackDefine attackDefine)
                        {
                            text.Value.text = attackDefine.Level.ToString();
                        }
                        break;
                    case CardUITextName.Damage:
                        if (actionDefine is AttackDefine attackDefine_)
                        {
                            text.Value.text = attackDefine_.Damage.ToString();
                        }
                        break;
                    case CardUITextName.Discription:
                        text.Value.text = actionDefine.Description;
                        break;
                    case CardUITextName.Name:
                        text.Value.text = actionDefine.Name;
                        break;
                }
            }
    }
}


//目前只考虑单一补给、单一消耗，未来如果有多的补给可以考虑用文字表示？
public enum CardUITextName
{
    Name = 0,
    Consume = 1,
    Supply = 2,
    Level = 3,
    Damage = 4,
    Discription = 5
}