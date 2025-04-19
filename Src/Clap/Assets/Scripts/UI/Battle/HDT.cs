using Common.Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;
using static UnityEngine.GraphicsBuffer;

public class HDT : MonoBehaviour
{
    public TextMeshProUGUI HDTText;
    public Image Image;
    bool hid =true;
    // Start is called before the first frame update
    void Start()
    {
        Rule.OnRoundChanged+=UpdateHUD;
        Image.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Rule.OnRoundChanged -= UpdateHUD;
    }

    public void changed()
    {
        if (hid)
        {
            Image.gameObject.SetActive(true);
            hid = false;
        }
        else
        {
            Image.gameObject.SetActive(false);
            hid = true;
        }
    }

    // Update is called once per frame
    void UpdateHUD()
    {
        int current = Battle.Instance.currentRound-1;
        string DisplayText = $"第 {current} 回合\n行动：\n";
        foreach (var item in Room.Instance.RoundMoves[current])
        {
                switch (item.type)
                {
                    case ItemType.补给:
                        DisplayText += $"玩家{item.id}使用了{item.Name}对自己进行补给\n";
                        break;
                    case ItemType.近战:
                    case ItemType.远程:
                        DisplayText += $"玩家{item.id}使用了{item.Name}+攻击了玩家{string.Join("和", item.targetid) }\n";
                        break;
                    case ItemType.防御:
                    case ItemType.反制:
                        DisplayText += $"玩家{item.id}使用了{item.Name}进行{item.type}\n";
                        break;
                    case ItemType.特效:
                        DisplayText += $"玩家{item.id}对玩家{string.Join("和", item.targetid)}使用了{item.Name}\n";
                        break;
                    default:
                        break;
            }
        }
        DisplayText += $"结算：\n";
        foreach (var kss in Room.Instance.RoundChangeds[current])
        {
            if (kss.HpChanged != 0)
            {
                string s = kss.HpChanged>0? "+" : "";
                DisplayText += $"玩家{kss.id}的血量{s}{kss.HpChanged}\n";
            }
            if (kss.BulletChanged != 0)
            {
                string s = kss.BulletChanged>0? "+" : "";
                DisplayText += $"玩家{kss.id}的子弹数{s}{kss.BulletChanged}\n";
            }
            if (kss.SwardChanged != 0)
            {
                string s = kss.SwardChanged>0 ? "+" : "";
                DisplayText += $"玩家{kss.id}的剑数{s}{kss.SwardChanged}\n";
            }
            if (kss.InCDChanged != 0)
            {
                DisplayText += $"玩家{kss.id}的{kss.InCDChanged}把剑进入CD\n";
            }
            if (kss.MaxHpChanged != 0)
            {
                string s = kss.MaxHpChanged>0?"+":"";
                DisplayText += $"玩家{kss.id}的最大血量{s}{kss.MaxHpChanged}\n";
            }
            if (kss.buffs!=null)
            {
                DisplayText += $"玩家{kss.id}获得了buff:";
                foreach (var buff in kss.buffs.Values)
                {
                    DisplayText += $"\"{buff.Name}\" ";
                }
            }
        }
        HDTText.text += DisplayText;
    }
}
