using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ReadyAttribute : ObservableAttribute<bool>
{
    public void ReadyUp() => SetValue(true, "ReadyUp");
    public void Cancel() => SetValue(false, "Cancel");
}
