using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class IsAliveAttribute : ObservableAttribute<bool>
{
    public void Born() => SetValue(true, "Born");

    public void Die() => SetValue(false, "Die");
}
