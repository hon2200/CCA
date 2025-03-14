using UnityEngine;
internal class InputBox
{
    static Object cacheObject = null;

    public static UIInputBox Show(string message, string title = "", string btnOK = "确定", string btnCanel = "取消", string emptyTips = "输入不能为空")
    {
        if (cacheObject == null)
        {
            cacheObject = Resloader.Load<Object>("UI/UIInputBox");
        }

        GameObject go = (GameObject)GameObject.Instantiate(cacheObject);
        UIInputBox InputBox = go.GetComponent<UIInputBox>();
        InputBox.Init(title, message, btnOK, btnCanel, emptyTips);
        return InputBox;
    }
}
