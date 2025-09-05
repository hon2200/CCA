using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonJump : MonoSingleton<ButtonJump>
{
    public void JumpToInitialSettings()
    {
        WorkingOn.Instance.LoadScene("Initial Settings");
    }
    public void JumpToMainMenu()
    {
        WorkingOn.Instance.LoadScene("Main Menu");
    }
    public void JumpToFreeGame()
    {
        WorkingOn.Instance.LoadScene("Free Game");
    }
    public void Exit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
