using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeletePeople : MonoBehaviour
{
    // Start is called before the first frame update
    public void DeleteOnePeople()
    {
        HeroControlPanel.Instance.HeroIDDictionary.RemoveAt(HeroControlPanel.Instance.HeroIDDictionary.Count - 1);
        HeroControlPanel.Instance.RefreshText();
    }

    // Update is called once per frame
    void Update()
    {
        if (HeroControlPanel.Instance.HeroIDDictionary.Count == 0)
            GetComponent<Button>().interactable = false;
        else
            GetComponent<Button>().interactable = true; 
    }
}
