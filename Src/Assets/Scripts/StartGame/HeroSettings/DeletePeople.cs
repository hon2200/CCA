using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeletePeople : MonoBehaviour
{
    // Start is called before the first frame update
    public void DeleteOnePeople()
    {
        GameSetting.Instance.HeroIDDictionary.RemoveAt(GameSetting.Instance.HeroIDDictionary.Count - 1);
        GameSetting.Instance.RefreshText();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameSetting.Instance.HeroIDDictionary.Count == 0)
            GetComponent<Button>().interactable = false;
        else
            GetComponent<Button>().interactable = true; 
    }
}
