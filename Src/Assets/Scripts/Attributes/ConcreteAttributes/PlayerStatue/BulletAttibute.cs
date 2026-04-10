using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BulletAttibute : ObservableAttribute<int>
{
    public Player Owner { get; set; }
    public void Set(int amount) => SetValue(amount, "Set");
    public void Use(int number) => SetValue(Value - number, "Use");
    public void Get(int number)
    {
        SetValue(Value + number, "Get");
        if (onObserve && number > 0)
            EffectManager.Instance.PlaySpotEffect(false, "BulletSupply", Owner.gameObject, number);
    }
    public void Lost(int number)
    {
        SetValue(Math.Max(Value - number, 0), "Lost");
        if (onObserve && number > 0)
            EffectManager.Instance.PlaySpotEffect(false, "BulletLose", Owner.gameObject, number);
    }
}
