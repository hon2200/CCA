using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


public class Setting : MonoBehaviour
{
    // Start is called before the first frame update

    public Slider volumneSlider;
    public AudioMixer BGMaudioMixer;
    public AudioSource Click;
    public AudioSource ClOSE;

    
    void Start()
    {
        //volumneSlider.value=volumneSlider.maxValue;
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Escape)) { gameObject.SetActive(false); }
    }

    public void ClickSoundEffect()
    {
        Click.Play();
    }
    public void Close1()
    {
        ClOSE.Play();
        gameObject.SetActive(false);
    }


    public void SetVolume(float volume)
    {
        BGMaudioMixer.SetFloat("BGM", volume);
    }
}
