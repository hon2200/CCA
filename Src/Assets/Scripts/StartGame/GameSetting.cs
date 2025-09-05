using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

public class GameSetting : MonoSingleton<GameSetting>
{
    public TMP_Text SelectedHeroText;
    public List<string> HeroIDDictionary { get; set; }
    private new void Awake()
    {
        base.Awake();
        HeroIDDictionary = new();
        RefreshText();
    }
    public void RefreshText()
    {
        SelectedHeroText.text = "";
        if (HeroIDDictionary.Count == 0)
            SelectedHeroText.text = "No Heroes";
        else if (HeroIDDictionary.Count >= 5)
            SelectedHeroText.text = "Too Many";
        else
        {
            foreach (var ID in HeroIDDictionary)
            {
                HeroDataBase.Instance.HeroDictionary.TryGetValue(ID, out var heroDefine);
                if (heroDefine != null)
                    SelectedHeroText.text += heroDefine.Name;
                else
                    SelectedHeroText.text += "Can't find Hero";
                SelectedHeroText.text += "\n";
            }
        }

    }
}
