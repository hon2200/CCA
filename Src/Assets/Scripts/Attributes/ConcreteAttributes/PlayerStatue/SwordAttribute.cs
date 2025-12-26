using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Sword 是一个Observable，我需要做的事情是，在Sword数或Available Sword改变时，
// 调用UI通知，根据类别调用动画，这个会通过OnValueChanged去调用。
// 调用技能效果，这个也需要通过OnValueChanged来调用。
public class SwordAttribute : ObservableAttribute<int>
{
    public bool Used { get; set; }
    public AvailableSwordAttribute AvailableSword { get; private set; }
    public SwordAttribute()
    {
        Used = false;
        AvailableSword = new AvailableSwordAttribute();
    }
    public void Set(int amount)
    {
        SetValue(amount, "Set");
        AvailableSword.Set(amount);
    }
    public void Get(Player thisPlayer, int number)
    {
        SetValue(Value + number, "Get");
        if (onObserve && number > 0)
            EffectManager.Instance.PlaySpotEffect(false, "SwordSupply", thisPlayer.gameObject, number);
        AvailableSword.Get(number);
    }
    public void Use(int number)
    {
        AvailableSword.Use(number);
        if (number > 0)
            Used = true;
    }
    public void Lost(int number)
    {
        if (number > Value)
        {
            SetValue(0, "Lost");
            AvailableSword.Lost(AvailableSword.Value);
        }
        else
        {
            //不可用剑比需要失去的少
            if (Value - AvailableSword.Value < number)
                AvailableSword.Lost(number - (Value - AvailableSword.Value));
            SetValue(Value - number, "Lost");
        }


    }
    public void ForcedCD(int number)
    {
        AvailableSword.Lost(number);
    }
    public void CoolDown()
    {
        if(!Used)
            AvailableSword.CoolDown(Value);
    }
    public void OnNewTurn() => Used = false;
    public override void Save()
    {
        base.Save();
        AvailableSword.Save();
    }
    public override void Load()
    {
        base.Load();
        AvailableSword.Load();
    }
}


public class AvailableSwordAttribute : ObservableAttribute<int>
{
    public void Set(int amount) => SetValue(amount, "Set");
    public void CoolDown(int number) => SetValue(number, "CoolDown");
    public void Use(int number) => SetValue(Value - number, "Use");
    public void Lost(int number) => SetValue(Value - number, "Lost");
    public void Get(int number) => SetValue(Value + number, "Get");
}