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
        string DisplayText = $"�� {current} �غ�\n�ж���\n";
        foreach (var item in Room.Instance.RoundMoves[current])
        {
                switch (item.type)
                {
                    case ItemType.����:
                        DisplayText += $"���{item.id}ʹ����{item.Name}���Լ����в���\n";
                        break;
                    case ItemType.��ս:
                    case ItemType.Զ��:
                        DisplayText += $"���{item.id}ʹ����{item.Name}+���������{string.Join("��", item.targetid) }\n";
                        break;
                    case ItemType.����:
                    case ItemType.����:
                        DisplayText += $"���{item.id}ʹ����{item.Name}����{item.type}\n";
                        break;
                    case ItemType.��Ч:
                        DisplayText += $"���{item.id}�����{string.Join("��", item.targetid)}ʹ����{item.Name}\n";
                        break;
                    default:
                        break;
            }
        }
        DisplayText += $"���㣺\n";
        foreach (var kss in Room.Instance.RoundChangeds[current])
        {
            if (kss.HpChanged != 0)
            {
                string s = kss.HpChanged>0? "+" : "";
                DisplayText += $"���{kss.id}��Ѫ��{s}{kss.HpChanged}\n";
            }
            if (kss.BulletChanged != 0)
            {
                string s = kss.BulletChanged>0? "+" : "";
                DisplayText += $"���{kss.id}���ӵ���{s}{kss.BulletChanged}\n";
            }
            if (kss.SwardChanged != 0)
            {
                string s = kss.SwardChanged>0 ? "+" : "";
                DisplayText += $"���{kss.id}�Ľ���{s}{kss.SwardChanged}\n";
            }
            if (kss.InCDChanged != 0)
            {
                DisplayText += $"���{kss.id}��{kss.InCDChanged}�ѽ�����CD\n";
            }
            if (kss.MaxHpChanged != 0)
            {
                string s = kss.MaxHpChanged>0?"+":"";
                DisplayText += $"���{kss.id}�����Ѫ��{s}{kss.MaxHpChanged}\n";
            }
            if (kss.buffs!=null)
            {
                DisplayText += $"���{kss.id}�����buff:";
                foreach (var buff in kss.buffs.Values)
                {
                    DisplayText += $"\"{buff.Name}\" ";
                }
            }
        }
        HDTText.text += DisplayText;
    }
}
