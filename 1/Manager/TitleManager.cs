using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;

public class TitleManager : MonoBehaviour
{
    Loading loadObj;

    private IObservable<Unit> TouchObserver => this.UpdateAsObservable();

    [Header("タイトルコール")]
    [SerializeField]
    private AudioClip[] titleCallList;
    private AudioSource audio => GetComponent<AudioSource>();
    private Subject<Unit> titleCall = new Subject<Unit>();

    void Start()
    {
        loadObj = GameObject.Find("FadeCanvas").GetComponent<Loading>();
        loadObj.isChange.OnNext(false);

        //StartCoroutine(TitleLoad());

        TouchObserver.Subscribe(_ => {
            StartCoroutine(StartGame());
        });

        titleCall.First().Subscribe(_ =>
        {
            int rndTitleCall = UnityEngine.Random.Range(0, titleCallList.Length);
            audio.PlayOneShot(titleCallList[rndTitleCall]);
            Debug.Log("titleNum = " + rndTitleCall);
        });
    }

    /// <summary>
    /// ロード処理
    /// </summary>
    /// <returns></returns>
    IEnumerator TitleLoad()
    {
        //SoundManagerを取得、ロード中の再生を防ぐため非アクティブ化
        var soundManager = GameObject.FindObjectOfType<SoundManager>();
        soundManager.enabled = false;

        yield return null;

        //ロードが完了したらSoundManagerをアクティブ化
        soundManager.enabled = true;
    }

    /// <summary>
    /// ゲーム開始処理
    /// </summary>
    public IEnumerator StartGame()
    {
        var anim = GameObject.Find("Tap").GetComponent<Animator>();

        if (Input.touchCount > 0 || Input.GetMouseButton(0))
        {
            anim.SetTrigger("IsStart");
            yield return null;
            titleCall.OnNext(Unit.Default);
            loadObj.nextSceneNum.OnNext((int)Loading.SceneName.Home);
            yield break;
        }
    }
}
