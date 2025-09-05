using UnityEngine.UI;
using UnityEngine;

public class SliderInitial : MonoBehaviour
{
    // Start is called before the first frame update
    public Scrollbar s;
    public void Start()
    {
        s.value = 1;
    }
}
