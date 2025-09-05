using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class WorkingOn : MonoSingleton<WorkingOn>
{
    public GameObject loadScreen;
    public Slider slider;   //½ø¶ÈÌõ
    public TextMeshProUGUI Text;
    public void LoadScene(string SceneName)
    {
        StartCoroutine(Loading(SceneName));
    }
    IEnumerator Loading(string SceneName)
    {
        loadScreen.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneName);
        operation.allowSceneActivation = true;

        while (!operation.isDone)
        {
            slider.value = operation.progress;
            Text.text = operation.progress * 100 + "%";

            if (operation.progress >= 0.9f)
            {
                slider.value = 1;
                Text.text = 100 + "%";
            }
            yield return null;

        }
    }
}
