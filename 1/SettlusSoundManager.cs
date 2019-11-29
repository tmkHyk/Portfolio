using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class SettlusSoundManager : MonoBehaviour
{
    public struct Voice
    {
        public List<AudioClip> wind;  //風に吹かれたとき
        public List<AudioClip> lost;  //置いて行かれたとき
        public List<AudioClip> damage;  //トゲ
        public List<AudioClip> mono;  //ひとりごと
    }
    
    List<SettlusStatePresenter> settlusList = new List<SettlusStatePresenter>();
    AudioSource audioSource;

    Voice boyVoice;
    Voice maleVoice;
    Voice girlVoice;
    Voice femaleVoice;

    System.Timers.Timer timer = new System.Timers.Timer();
    [SerializeField,Range(1,20)]
    private float interval;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        boyVoice = LoadVoice("Boy");
        maleVoice = LoadVoice("Male");
        girlVoice = LoadVoice("Girl");
        femaleVoice = LoadVoice("Female");
        
        timer.Interval = interval * 1000;
    }

    // Start is called before the first frame update
    void Start()
    {
        timer.Start();

        //アクティブなセトラスの取得
        foreach (var settlus in FindObjectsOfType<SettlusStatePresenter>())
        {
            settlusList.Add(settlus);
        }

        foreach (var settlus in settlusList)
        {
            //セトラスが死んだら
            settlus.OnDead.Subscribe(_ =>
                {
                    //死亡ボイスを再生
                    var gender = settlus.GetComponent<SettlusCreate>().GetGender();
                    var caseOfDeath = settlus.GetCaseOfDeath();
                    var voice = GetVoice(gender);
                    //if(!audioSource.isPlaying)
                    PlayDead(voice, caseOfDeath);
                });

            //セトラスが風に流されたら
            settlus.OnWind.Subscribe(_ =>
            {
                //ボイスを再生
                var gender = settlus.GetComponent<SettlusCreate>().GetGender();
                var voice = GetVoice(gender);
                if (!audioSource.isPlaying)
                    PlayWind(voice);
            });
        }

        //Observable.EveryUpdate().Subscribe(_ => 
        //{
        //    if (settlusList.Count > 0)
        //    {
        //        timer.Elapsed += (sender, e) =>
        //        {
        //            Debug.Log("MONO再生");
        //            var rand = new System.Random();
        //            var index = rand.Next(0, settlusList.Count);

        //            var gender = settlusList[index].GetComponent<SettlusCreate>().GetGender();
        //            Debug.Log(settlusList[index]);
        //            var voice = GetVoice(gender);
        //            PlayMono(voice);
        //        };
        //    }
        //});
    }

    /// <summary>
    /// Resourcesフォルダからボイスをロード
    /// </summary>
    /// <param name="folderName"></param>
    /// <returns></returns>
    Voice LoadVoice(string folderName)
    {
        var clips = Resources.LoadAll<AudioClip>("Voice/" + folderName);

        Voice voice = new Voice();
        voice.wind = new List<AudioClip>();
        voice.lost = new List<AudioClip>();
        voice.damage = new List<AudioClip>();
        voice.mono = new List<AudioClip>();

        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i].name.Contains("Wind"))
                voice.wind.Add(clips[i]);
            if (clips[i].name.Contains("Lost"))
                voice.lost.Add(clips[i]);
            if (clips[i].name.Contains("Damage"))
                voice.damage.Add(clips[i]);
            if (clips[i].name.Contains("Mono"))
                voice.mono.Add(clips[i]);
        }
        return voice;
    }

    /// <summary>
    /// セトラスの年齢、性別からボイスを指定、取得
    /// </summary>
    /// <param name="gender"></param>
    /// <returns></returns>
    Voice GetVoice(SettlusCreate.GENDER gender)
    {
        switch (gender)
        {
            case SettlusCreate.GENDER.BOY:
                return boyVoice;
            case SettlusCreate.GENDER.MALE:
                return maleVoice;
            case SettlusCreate.GENDER.GIRL:
                return girlVoice;
            case SettlusCreate.GENDER.FEMALE:
                return femaleVoice;
        }
        return new Voice();
    }

    /// <summary>
    /// 死亡ボイスの再生
    /// </summary>
    /// <param name="voice"></param>
    /// <param name="caseOfDeath"></param>
    void PlayDead(Voice voice,SettlusStatePresenter.CaseOfDeath caseOfDeath)
    {
        //刺殺の時
        if (caseOfDeath == SettlusStatePresenter.CaseOfDeath.Stucking)
            audioSource.PlayOneShot(voice.damage[Random.Range(0,voice.damage.Count)]);
        //遭難のとき
        if (caseOfDeath == SettlusStatePresenter.CaseOfDeath.distress)
            audioSource.PlayOneShot(voice.lost[Random.Range(0, voice.damage.Count)]);
    }

    /// <summary>
    /// 風に吹かれたボイスの再生
    /// </summary>
    /// <param name="voice"></param>
    void PlayWind(Voice voice)
    {
        audioSource.PlayOneShot(voice.wind[Random.Range(0, voice.wind.Count)]);
    }

    void PlayMono(Voice voice)
    {
        audioSource.PlayOneShot(voice.mono[Random.Range(0, voice.mono.Count)]);
    }
}
