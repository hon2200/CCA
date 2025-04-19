using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIMessageBox : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI message;
    public Image[] icons;
    public Button buttonYes;
    public Button buttonNo;
    public Button buttonClose;

    public TextMeshProUGUI buttonYesTitle;
    public TextMeshProUGUI buttonNoTitle;

    public UnityAction OnYes;
    public UnityAction OnNo;

    public void Init(string title, string message, MessageBoxType type = MessageBoxType.Information, string btnOK = "", string btnCancel = "")
    {
        if (!string.IsNullOrEmpty(title)) this.title.text = title;
        this.message.text = message;
        //this.icons[0].enabled = type == MessageBoxType.Information;
        //this.icons[1].enabled = type == MessageBoxType.Confirm;
        //this.icons[2].enabled = type == MessageBoxType.Error;
        if (string.IsNullOrEmpty(btnCancel)) this.buttonNoTitle.gameObject.SetActive(false);
        if (!string.IsNullOrEmpty(btnOK)) this.buttonYesTitle.text = btnOK;
        if (!string.IsNullOrEmpty(btnCancel)) this.buttonNoTitle.text = btnCancel;

        this.buttonYes.onClick.AddListener(OnClickYes);
        this.buttonNo.onClick.AddListener(OnClickNo);

        this.buttonNo.gameObject.SetActive(type == MessageBoxType.Confirm);

        //if (type == MessageBoxType.Error)
        //    SoundManager.Instance.PlaySound(SoundDefine.SFX_Message_Error);
        //else
        //    SoundManager.Instance.PlaySound(SoundDefine.SFX_Message_Info);


    }

    void OnClickYes()
    {
        //SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Confirm);
        Destroy(this.gameObject);
        if (this.OnYes != null)
            this.OnYes();
    }

    void OnClickNo()
    {
        //SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Close);
        Destroy(this.gameObject);
        if (this.OnNo != null)
            this.OnNo();
    }
}
