using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MuteUnmute : MonoBehaviour
{
    public GameObject Soundmute;
    public GameObject Soundunmute;
    public Slider volumneSlider;
    public void Mute()
    {

        Soundunmute.SetActive(true);
        gameObject.SetActive(false);
        volumneSlider.value = -40;
    }

    public void Unmute()
    {
        Soundmute.SetActive(true);
        gameObject.SetActive(false);
        volumneSlider.value = 10;

    }
    // Start is called before the first frame update
    void Start()
    {
        if (volumneSlider.value == -40)
        {
            Soundunmute.SetActive(true);
            Soundmute.SetActive(false);
        }
        else if (volumneSlider.value == 10)
        {
            Soundmute.SetActive(true);
            Soundunmute.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (volumneSlider.value == -40)
        {
            Soundunmute.SetActive(true);
            Soundmute.SetActive(false);
        }
        else if (volumneSlider.value == 10)
        {
            Soundmute.SetActive(true);
            Soundunmute.SetActive(false);
        }
    }
}
