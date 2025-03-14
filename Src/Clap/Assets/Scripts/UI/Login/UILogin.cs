using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILogin : MonoBehaviour
{
    public UIManager Manager;

    [Header("Button References")]
    public Button SingleBtn;
    public Button OnlineBtn;
    public Button HelpBtn;
    public Button llustratedBtn;
    public Button DeveloperBtn;
    public Button SettingsBtn;
    public Button QuitBtn;

    [Header("UI Panels")] 
    public GameObject settingsPanel;
    public GameObject creditsPanel;
    public string currentSence = "UILogin";

    void Start()
    {

    }

    #region ��ť�¼�ʵ��
    public void OnSingle()
    {
        SceneManager.Instance.LoadScene("CharSelect");
    }

    public void OnOnline()
    {
        SceneManager.Instance.LoadScene("CharSelect");
    }

    public void OnHelp()
    {
        //SceneManager.LoadScene("TutorialScene");
    }

    public void Onllustrated()
    {
        BookManager.Instance.ShowBook();
    }

    public void OnDeveloper()
    {
        UIManager.Instance.Show<UIDeveloper>();
    }

    public void OnSetting()
    {
        UIManager.Instance.Show<UISetting>();
    }

    public void OnQuit()
    {
        Application.Quit();
    }
    #endregion

    #region ��������
    // ����չ�������ơ��������õȹ���
    public void SetMasterVolume(float value)
    {
        AudioListener.volume = value;
    }
    
    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }
    #endregion
}