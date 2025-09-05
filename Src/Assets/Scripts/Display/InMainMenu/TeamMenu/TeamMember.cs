using UnityEngine;

public class TeamMember : MonoSingleton<TeamMember>
{
    public AudioSource Click;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Sound()
    {
        Click.Play();

    }
    public void Quit1()
    {
        Click.Play();
        gameObject.SetActive(false);
    }

}
