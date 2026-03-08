using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Michsky.UI.Dark
{
    [DisallowMultipleComponent]
    public class SplashScreenManager : MonoBehaviour
    {
        // Content
        public List<SplashScreenTitle> splashScreenTitles = new List<SplashScreenTitle>();

        // Resources
        public GameObject splashScreen;
        public GameObject modalWindowParent;
        public GameObject mainPanelParent;
        public UIDissolveEffect transitionHelper;
        public MainPanelManager mainPanelManager;

        // Settings
        public bool disableSplashScreen;
        public bool showOnlyOnce = true;
        public bool skipOnAnyKeyPress = false;
        public float disableTimer = 0;
        [Range(0, 5)] public float startDelay = 0.5f;
        public UnityEvent onSplashScreenEnd;    

        GameObject currentTitleObj;
        int currentTitleIndex;
        float currentTitleDuration;
        private float originalMusicVolume; // 记录原始音乐音量，用于淡出
        private bool isLastTitle = false; // 标记是否是最后一个标题页




        // 【新增】音频相关配置
        [Header("音频设置")]
        public AudioSource splashMusicSource; // 播放启动界面音乐的AudioSource
        public AudioClip splashBackgroundMusic; // 启动界面背景音乐
        public AudioClip transitionSound; // 界面过渡音效（可选）
        [Range(0, 1)] public float musicVolume = 0.8f; // 音乐音量
        [Range(0, 1)] public float soundVolume = 1f; // 音效音量
        public bool loopBackgroundMusic = true; // 是否循环播放背景音乐
        public bool fadeOutMusicOnEnd = true; // 过渡时是否淡出音乐
        [Range(0.5f, 3f)] public float fadeOutDuration = 1f; // 音乐淡出时长



        void OnEnable()
        {

            // 初始化音频源（自动创建/配置）
            InitAudioSource();

            if (showOnlyOnce && GameObject.Find("[Dark UI - Splash Screen Helper]") != null) 
            { 
                disableSplashScreen = true; 
            }

            if (disableSplashScreen)
            {
                splashScreen.SetActive(false);
                modalWindowParent.SetActive(true);

                mainPanelParent.gameObject.SetActive(true);
                transitionHelper.gameObject.SetActive(true);

                mainPanelManager.EnableFirstPanel();

                transitionHelper.location = 0;
                transitionHelper.DissolveOut();

                onSplashScreenEnd.Invoke();
            }

            else
            {

                // 启动界面激活：立即播放背景音乐
                PlaySplashMusicOnStart();

                splashScreen.SetActive(true);
                modalWindowParent.SetActive(false);

                mainPanelParent.gameObject.SetActive(false);
                transitionHelper.gameObject.SetActive(false);

                InitializeTitles();         
            }

            if (showOnlyOnce)
            {
                GameObject tempHelper = new GameObject();
                tempHelper.name = "[Dark UI - Splash Screen Helper]";
                DontDestroyOnLoad(tempHelper);
            }
        }

        void Update()
        {
            if (!skipOnAnyKeyPress)
                return;

            if ((UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.anyKey.wasPressedThisFrame)
                || (UnityEngine.InputSystem.Mouse.current != null && UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
                || (UnityEngine.InputSystem.Gamepad.current != null && UnityEngine.InputSystem.Gamepad.current.buttonSouth.wasPressedThisFrame)
                || (UnityEngine.InputSystem.Touchscreen.current != null && UnityEngine.InputSystem.Touchscreen.current.press.wasPressedThisFrame))
            {
                skipOnAnyKeyPress = false;
                SkipSplashScreen();
            }
        }


        // 初始化音频源（自动创建，避免手动添加）
        private void InitAudioSource()
        {
            if (splashMusicSource == null)
            {
                splashMusicSource = GetComponent<AudioSource>();
                if (splashMusicSource == null)
                {
                    splashMusicSource = gameObject.AddComponent<AudioSource>();
                }
            }

            // 核心配置：不自动播放、设置音量、循环
            splashMusicSource.volume = musicVolume;
            splashMusicSource.loop = loopBackgroundMusic;
            splashMusicSource.playOnAwake = false;
            splashMusicSource.clip = splashBackgroundMusic;
            originalMusicVolume = musicVolume;
        }

        // 启动界面开始时立即播放音乐
        private void PlaySplashMusicOnStart()
        {
            if (splashBackgroundMusic != null && splashMusicSource != null)
            {
                splashMusicSource.Play();
                Debug.Log("启动界面背景音乐开始播放");
            }
        }

        // 最后一个标题页结束时停止音乐（支持淡出）
        private IEnumerator StopMusicOnLastTitleEnd()
        {
            if (splashMusicSource == null || !splashMusicSource.isPlaying)
            {
                yield break;
            }

            // 淡出音乐（更自然）
            if (fadeOutMusicOnEnd)
            {
                float elapsedTime = 0f;
                while (elapsedTime < fadeOutDuration)
                {
                    splashMusicSource.volume = Mathf.Lerp(originalMusicVolume, 0, elapsedTime / fadeOutDuration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
            }

            // 停止音乐并恢复音量（方便后续复用）
            splashMusicSource.Stop();
            splashMusicSource.volume = originalMusicVolume;
            Debug.Log("最后一个启动界面消失，背景音乐停止");
        }
        public void SkipSplashScreen()
        {
            if (!splashScreen.activeInHierarchy)
                return;

            StopCoroutine("DisableSplashScreen");
            StopCoroutine("InitializeTitleDuration");
            StopCoroutine("ProcessStartDelay");

            disableTimer = 0;

            StartCoroutine("DisableSplashScreen");
        }

        public void InitializeTitles()
        {
            if (splashScreenTitles.Count != 0)
            {
                for (int i = 0; i < splashScreenTitles.Count; ++i)
                    disableTimer = disableTimer + splashScreenTitles[i].screenTime;

                foreach (Transform child in splashScreenTitles[0].gameObject.transform.parent)
                    child.gameObject.SetActive(false);

                currentTitleIndex = 0;
                currentTitleDuration = splashScreenTitles[currentTitleIndex].screenTime;
                currentTitleObj = splashScreenTitles[currentTitleIndex].gameObject;

                if (startDelay == 0)
                {
                    currentTitleObj.SetActive(true);
                    EnableTransition();
                }

                else
                {
                    StartCoroutine("ProcessStartDelay");
                }
            }
        }

        public void EnableTransition()
        {
            StartCoroutine("DisableSplashScreen");
            StartCoroutine("InitializeTitleDuration");
        }





        IEnumerator ProcessStartDelay()
        {
            yield return new WaitForSecondsRealtime(startDelay);
        
            currentTitleObj.SetActive(true);

            StopCoroutine("ProcessStartDelay");
            EnableTransition();
        }

        IEnumerator InitializeTitleDuration()
        {
            yield return new WaitForSecondsRealtime(currentTitleDuration);
           
            currentTitleObj.SetActive(false);
            currentTitleIndex++;
            
            try
            {
                currentTitleDuration = splashScreenTitles[currentTitleIndex].screenTime;
                currentTitleObj = splashScreenTitles[currentTitleIndex].gameObject;
                currentTitleObj.SetActive(true);
                StartCoroutine("InitializeTitleDuration");
            }

            catch 
            {
                StopCoroutine("InitializeTitleDuration");
            }
        }

        IEnumerator DisableSplashScreen()
        {
            yield return new WaitForSecondsRealtime(disableTimer);

             // 最后一个标题页消失时，停止背景音乐
            StartCoroutine(StopMusicOnLastTitleEnd());


            splashScreen.SetActive(false);
            modalWindowParent.SetActive(true);

            mainPanelParent.gameObject.SetActive(true);
            transitionHelper.gameObject.SetActive(true);

            mainPanelManager.EnableFirstPanel();

            transitionHelper.location = 0;
            transitionHelper.DissolveOut();

            onSplashScreenEnd.Invoke();

            StopCoroutine("StartTransition");
        }
    }
}