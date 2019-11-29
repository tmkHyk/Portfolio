using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (SoundManager)FindObjectOfType(typeof(SoundManager));

                if (instance == null)
                {
                    Debug.LogWarning(typeof(SoundManager) + "is nothing");
                }
            }

            return instance;
        }
    }
   

    AudioSource audioSource;
    [SerializeField,Tooltip("使用するBGM 0:Title 1:Game 2:Result")]
    AudioClip[] bgmClips = new AudioClip[3];

    //現在流れているBGM
    AudioClip nowPlaying;
    protected void Awake()
    {
        CheckInstance();
    }

    protected bool CheckInstance()
    {
        if (instance == null)
        {
            instance = (SoundManager)this;
            return true;
        }
        else if (Instance == this)
        {
            return true;
        }

        Destroy(this.gameObject);
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = transform.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //BGMの再生
        SetBGM();
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// BGMの指定
    /// </summary>
    void SetBGM()
    {
        Play(bgmClips[SceneManager.GetActiveScene().buildIndex]);
    }

    /// <summary>
    /// BGMの再生
    /// </summary>
    /// <param name="clip"></param>
    void Play(AudioClip clip)
    {
        //何も再生されていないとき
        if (!audioSource.isPlaying)
        {
            //再生可能
            audioSource.PlayOneShot(clip);
            //現在流れている曲を指定
            nowPlaying = clip;
        }
        //何か再生されているとき
        else if(nowPlaying != clip)
        {
            //停止
            audioSource.Stop();
        }
    }
}
