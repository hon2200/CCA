using Common.Data;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
public class Character : MonoBehaviour, IPointerEnterHandler
{

    public CharacterDefine character;
    public AIMove move;

    public int ID;
    public delegate void OnTargetChoose(int id);
    public static event OnTargetChoose TargetChoose;

    public TextMeshProUGUI Name;
    public TextMeshProUGUI Hp;
    public TextMeshProUGUI Buttles;
    public TextMeshProUGUI Swords;
    public RectTransform targetRect;

    public void NewSet(int Id,string name)
    {
        ID = Id;
        if(Name!=null) Name.text = name;
        if (Id < 0)
        {
            move = gameObject.AddComponent<AIMove>();
            move.owner = this;
            move.Set(Id);
        }
    }
    public void Show(int hp,int Maxhp,int buttle,int sword,int swordcd )
    {
        if (BattleManager.Instance.IsDead(ID))
        {
            Destroy(this);
        }
        Hp.text = hp.ToString()+"/"+Maxhp.ToString();
        Buttles.text = buttle.ToString();
        Swords.text = sword.ToString()+"/"+(swordcd+ sword).ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //this.detial.gameObject.SetActive(true);
        StartCoroutine(up());
    }

    public IEnumerator up()//多线程调用
    {
        while (true) {
            bool isMouseInside = RectTransformUtility.RectangleContainsScreenPoint(
            targetRect,
            Input.mousePosition,
            null
            );
            if (!isMouseInside)
            {
                //this.detial.gameObject.SetActive(false);
                break;
            }else if (Input.GetMouseButtonDown(0))
            {
                TargetChoose?.Invoke(this.ID);
                break;
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
}
