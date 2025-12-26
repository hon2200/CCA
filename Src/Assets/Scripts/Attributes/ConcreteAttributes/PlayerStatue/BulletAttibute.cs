using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BulletAttibute : ObservableAttribute<int>
{
    public void Set(int amount) => SetValue(amount, "Set");
    public void Use(int number) => SetValue(Value - number, "Use");
    public void Get(Player thisPlayer,int number)
    {
        SetValue(Value + number, "Get");
        if (onObserve && number > 0)
            EffectManager.Instance.PlaySpotEffect(false, "BulletSupply", thisPlayer.gameObject, number);
    }
    public void Lost(int number) => SetValue(Value - number, "Lost");
}
