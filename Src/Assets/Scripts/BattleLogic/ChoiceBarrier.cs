using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//在进入下一个阶段前，需要所有决定都已经被发送
public class ChoiceBarrier : MonoSingleton<ChoiceBarrier>
{

    public int pending { get; private set; }

    public bool IsComplete => pending <= 0;

    public void Add(int count = 1)
    {
        pending += count;
    }

    public void Resolve()
    {
        pending--;
    }
}
