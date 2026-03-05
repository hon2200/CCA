using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeletePeople : MonoBehaviour
{
    // Start is called before the first frame update
    public void DeleteOnePeople()
    {
        if (PlayerManager.Instance != null)
            PlayerManager.Instance.RemoveAllHumanPlayers();
    }

    void Update()
    {
        var btn = GetComponent<Button>();
        if (btn != null)
            btn.interactable = PlayerManager.Instance != null && PlayerManager.Instance.HumanPlayers.Count > 0;
    }
}
