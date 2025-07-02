using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class HPAttribute : ObservableAttribute<int>
{
    public void Set(int amount) => SetValue(amount, "Set");
    public void Heal(int amount) => SetValue(Value + amount, "Heal");
    public void Damage(int amount) => SetValue(Value - amount, "Damage");
    public void Drain(int amount) => SetValue(Value - amount, "Drain");
}
