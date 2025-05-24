using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AvailableSwordAttribute : ObservableAttribute<int>
{
    public void Set(int amount) => SetValue(amount, "Set");
    public void CoolDown(int number) => SetValue(number, "CoolDown");
    public void Use(int number) => SetValue(Value - number, "Use");
    public void Get(int number) => SetValue(Value + number, "Get");
}
