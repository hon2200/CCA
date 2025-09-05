using UnityEngine;
using UnityEngine.UI;

public class PreviousPage : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Previous;
    public AudioSource Click;
    public Button PreviousPageButton;
    public void TurntoPreviousPage()
    {
        Click.Play();
        gameObject.SetActive(false);
        Previous.SetActive(true);
    }
    void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        if (Previous == null)
            PreviousPageButton.interactable = false;
        else
            PreviousPageButton.interactable = true;
    }
}
