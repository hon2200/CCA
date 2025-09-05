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
    public GameObject image;   //���ؽ���
    public GameObject Menu;
    public Slider slider;   //������
    public Text text;      //���ؽ����ı�
    public GameObject Tutorial;
    //���ư�ť���������
    public AudioSource Click;
    public void LoadNextLeaver()
    {
        image.SetActive(true);
        Menu.SetActive(false);
        StartCoroutine(LoadLeaver());
    }
    IEnumerator LoadLeaver()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("FreeGame"); //��ȡ��ǰ��������һ
                                                                            //operation.allowSceneActivation = false;
        while (!operation.isDone)   //������û�м������
        {
            slider.value = operation.progress;  //�������볡�����ؽ��ȶ�Ӧ
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
