using UnityEngine;
using UnityEngine.UI;

public class NextPage : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Next;
    public AudioSource Click;
    public Button NextPageButton;
    public void TurntoNextPage()
    {
        Click.Play();
        Next.SetActive(true);
        gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Next == null)
            NextPageButton.interactable = false;
        else
            NextPageButton.interactable = true;
    }
}
