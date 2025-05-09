using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

public class TurnAttribute : ObservableAttribute<int>
{
    public void Clear() => SetValue(0, "Clear");
    public void Advance() => SetValue(Value + 1, "Advance");
}

