using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtonJump : MonoBehaviour
{
    public GameObject MainPanel;
    public GameObject SingleModePanel;
    public GameObject OnlineModePanel;
    public GameObject optionsPanel;
    public GameObject classicalModePanel;
    public GameObject TeamMenu;
    public GameObject RuleMenu;
    public GameObject image;   //加载界面
    public GameObject Menu;
    public Slider slider;   //进度条
    public Text text;      //加载进度文本
    public GameObject Tutorial;
    //控制按钮点击的声音
    public AudioSource Click;
    public void LoadNextLeaver()
    {
        image.SetActive(true);
        Menu.SetActive(false);
        StartCoroutine(LoadLeaver());
    }
    IEnumerator LoadLeaver()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("FreeGame"); //获取当前场景并加一
                                                                            //operation.allowSceneActivation = false;
        while (!operation.isDone)   //当场景没有加载完毕
        {
            slider.value = operation.progress;  //进度条与场景加载进度对应
            text.text = (operation.progress * 100).ToString() + "%";
            yield return null;
        }
    }
    public void ClickSoundTrigger()
    {
        Click.Play();
    }
    public void OpenSingleMode()
    {
        MainPanel.SetActive(false);
        SingleModePanel.SetActive(true);
    }
    public void OpenOnlineMode()
    {
        MainPanel.SetActive(false);
        OnlineModePanel.SetActive(true);
    }
    public void OpenClassicalMode()
    {
        SingleModePanel.SetActive(false);
        MainPanel.SetActive(false);
        classicalModePanel.SetActive(true);
    }
    public void ReturnToMainPanel()
    {
        SingleModePanel.SetActive(false);
        OnlineModePanel.SetActive(false);
        classicalModePanel.SetActive(false);
        MainPanel.SetActive(true);
    }
    public void JumpToInitialSettings()
    {
        SceneManager.LoadScene("Initial Settings");
    }
    public void JumpToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void Setting()
    {
        optionsPanel.SetActive(true);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void Team()
    {
        TeamMenu.SetActive(true);
    }

    public void Rule()
    {
        RuleMenu.SetActive(true);
    }

    public void JumpToTutorial_Map()
    {
        Tutorial.SetActive(true);
    }
}
