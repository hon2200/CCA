using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public Scene currentScene = 0;

    public enum Scene
    {
        UILogin=0,
    }

    class UIElent
    {
        public string Resources;
        public bool Cache;
        public GameObject Instance;
    }

    private Dictionary<Type, UIElent> UIResources = new Dictionary<Type, UIElent>();

    public UIManager()
    {//所有需要的预制界面
        this.UIResources.Add(typeof(UISetting), new UIElent() { Resources = "UI/UISetting", Cache = true });
        this.UIResources.Add(typeof(UIDeveloper), new UIElent() { Resources = "UI/UIDeveloper", Cache = false });
        this.UIResources.Add(typeof(UIBook), new UIElent() { Resources = "UI/UIBook", Cache = false });
    }

    ~UIManager() { }

    public  T Show<T>()
    {
        //SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Open);音效
        Type type = typeof(T);
        if (this.UIResources.ContainsKey(type))
        {
            UIElent info = this.UIResources[type];
            if (info.Instance != null)
            {
                info.Instance.SetActive(true);
            }
            else
            {
                UnityEngine.Object prefab = Resources.Load(info.Resources);
                if (prefab == null)
                {
                    return default(T);
                }
                info.Instance = (GameObject)GameObject.Instantiate(prefab);
            }
            return info.Instance.GetComponent<T>();
        }
        return default(T);
    }
    public void Close(Type type)
    {
        if (this.UIResources.ContainsKey(type))
        {
            UIElent info = this.UIResources[type];
            if (info.Cache)
            {
                info.Instance.SetActive(false);
            }
            else
            {
                GameObject.Destroy(info.Instance);
                info.Instance = null;
            }
        }
    }

    public void Close<T>()
    {
        this.Close(typeof(T));
    }
}
