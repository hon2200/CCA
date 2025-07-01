using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SwordAttribute : ObservableAttribute<int>
{
    public void Set(int amount) => SetValue(amount, "Set");
    public void Use(int number) => SetValue(Value - number, "Use");
    public void Get(int number) => SetValue(Value + number, "Get");
}
