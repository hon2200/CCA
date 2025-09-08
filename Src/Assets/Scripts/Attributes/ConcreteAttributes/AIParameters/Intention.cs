using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Intention : ObservableAttribute<ActionType>
{
    public void Set(ActionType newIntention) => SetValue(newIntention, "Set");
}
