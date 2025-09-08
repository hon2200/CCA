using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Honesty : ObservableAttribute<float>
{
    public void Set(float amount) => SetValue(amount, "Set");
    public void ChangeBy(float amount) => SetValue(Value + amount, "Change");
    public void ChangeTo(float amount) => SetValue(amount, "Change");
}