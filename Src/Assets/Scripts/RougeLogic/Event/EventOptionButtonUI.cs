using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class EventOptionButtonUI : MonoSingleton<EventOptionButtonUI>
{
    [SerializeField] private TMP_Text Title;
    [SerializeField]  private Button button;

    public void AddListener(UnityAction action)
    {
        button.onClick.AddListener(action);
    }

    public void UpdateTitle(string title)
    {
        Title.text = title;
    }
}
