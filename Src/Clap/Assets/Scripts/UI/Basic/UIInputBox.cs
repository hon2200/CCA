using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class UIInputBox : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI message;
    public TextMeshProUGUI tips;
    public Button buttonYes;
    public Button buttonNo;
    public TMP_InputField input;

    public TextMeshProUGUI buttonYesTitle;
    public TextMeshProUGUI buttonNoTitle;

    public delegate bool SubmitHandler(string inputText, out string tips);
    public event SubmitHandler OnSubmit;
    public UnityAction OnCancel;

    public string emptyTips;

    public void Init(string title, string message, string btnOK = " ", string btnCanel = " ", string emptyTips = "")
    {
        if (!string.IsNullOrEmpty(title)) this.title.text = title;
        this.message.text = message;
        this.tips.text = null;
        this.OnSubmit = null;
        this.emptyTips = emptyTips;

        if (!string.IsNullOrEmpty(btnOK)) this.buttonYesTitle.text = btnOK;
        if (!string.IsNullOrEmpty(btnCanel)) this.buttonNoTitle.text = btnCanel;

        this.buttonYes.onClick.AddListener(OnClickYes);
        this.buttonNo.onClick.AddListener(OnClickNo);
    }

    void OnClickYes()
    {
        this.tips.text = "";
        if (string.IsNullOrEmpty(input.text))
        {
            this.tips.text = this.emptyTips;
            return;
        }
        if (OnSubmit != null)
        {
            string tips;
            if (!OnSubmit(this.input.text, out tips))
            {
                this.tips.text = tips;
                return;
            }
        }
        Destroy(this.gameObject);
    }

    void OnClickNo()
    {
        Destroy(this.gameObject);
        if (this.OnCancel != null)
            this.OnCancel();
    }
}
