using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoSingleton<AudioManager>
{

    [Header("BGM设置")]
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioClip defaultBGM;
    [Range(0f, 1f)] public float bgmVolume = 0.8f;

    [Header("UI点击音效设置")]
    [SerializeField] private AudioClip uiClickSound;
    [Range(0.01f, 1f)] public float uiSoundVolume = 1f;

    [Header("UI悬浮音效设置")]
    [SerializeField] private AudioClip uiHoverSound;
    [Range(0.01f, 1f)] public float uiHoverVolume = 1f;

    [Header("战斗音效设置")]
    [SerializeField] public AudioClip battleSound;
    [Range(0.01f, 1f)] public float battleVolume = 1f;


    private float _actualUIVolume;
    private float _actualHoverVolume;
    // 新增：标记是否是首次加载场景
    private bool _isFirstSceneLoaded = false;

    private void Awake()
    {
        _actualUIVolume = uiSoundVolume;
        _actualHoverVolume = uiHoverVolume;

        // 注册场景加载事件
        SceneManager.sceneLoaded += OnSceneLoaded;

        // BGM初始化
        if (bgmAudioSource == null)
        {
            bgmAudioSource = gameObject.AddComponent<AudioSource>();
            bgmAudioSource.playOnAwake = true;
            bgmAudioSource.loop = true;
            bgmAudioSource.volume = bgmVolume;
            if (defaultBGM != null)
            {
                bgmAudioSource.clip = defaultBGM;
                bgmAudioSource.Play();
            }
        }

        // 预加载音效
        if (uiClickSound != null) uiClickSound.LoadAudioData();
        if (uiHoverSound != null) uiHoverSound.LoadAudioData();
    }

    // 修改：仅在非首次场景加载时停止BGM
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 首次加载场景：标记为已加载，不停止BGM
        if (!_isFirstSceneLoaded)
        {
            _isFirstSceneLoaded = true;
            Debug.Log("首次加载场景，保留BGM播放");
            return;
        }

        // 非首次（场景切换）：停止BGM
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
            Debug.Log($"切换到场景{scene.name}，停止BGM播放");
        }
    }

    // 原有方法完全保留（无需修改）
    public void PlayUIClickSound()
    {
        if (uiClickSound == null)
        {
            Debug.LogError("请拖入UI点击音效文件！");
            return;
        }
        _actualUIVolume = Mathf.Clamp(_actualUIVolume, 0.01f, 1f);
        AudioSource.PlayClipAtPoint(uiClickSound, Vector3.zero, _actualUIVolume);
        //Debug.Log($"当前UI点击音效音量：{_actualUIVolume}");
    }

    public void PlayUIHoverSound()
    {
        if (uiHoverSound == null)
        {
            Debug.LogError("请拖入UI悬浮音效文件！");
            return;
        }
        _actualHoverVolume = Mathf.Clamp(_actualHoverVolume, 0.01f, 1f);
        AudioSource.PlayClipAtPoint(uiHoverSound, Vector3.zero, _actualHoverVolume);
        Debug.Log($"当前UI悬浮音效音量：{_actualHoverVolume}");
    }

    public void SetUISoundVolume(float volume)
    {
        _actualUIVolume = Mathf.Clamp(volume, 0.01f, 1f);
        uiSoundVolume = _actualUIVolume;
    }

    public void SetUIHoverVolume(float volume)
    {
        _actualHoverVolume = Mathf.Clamp(volume, 0.01f, 1f);
        uiHoverVolume = _actualHoverVolume;
    }

    public void PlayBGM(AudioClip bgmClip)
    {
        if (bgmClip == null || bgmAudioSource == null) return;
        bgmAudioSource.clip = bgmClip;
        bgmAudioSource.Play();
    }

    public void ToggleBGM(bool isPlay)
    {
        if (bgmAudioSource == null) return;
        if (isPlay) bgmAudioSource.Play();
        else bgmAudioSource.Pause();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}