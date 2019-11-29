using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
using System;

using DG.Tweening;

public class Loading : MonoBehaviour
{
    private Slider m_slider;
    private Text m_text;
    private CanvasGroup m_canvasGroup;

    //これからシーン移行をするか
    //trueのときFadeIn falseのときFadeOut
    private BoolReactiveProperty isSceneChange = new BoolReactiveProperty(false);
    public Subject<bool> isChange = new Subject<bool>();
    public Subject<int> nextSceneNum = new Subject<int>();

    public BoolReactiveProperty isLoading = new BoolReactiveProperty(false);

    public enum SceneName
    {
        Title = 0,
        Home,
        Game
    }
    
    private int nextSceneIndex;

    // Start is called before the first frame update
    void Start()
    {
        isSceneChange.Value = false;
        m_slider = transform.GetComponentInChildren<Slider>();
        m_text = transform.GetComponentInChildren<Text>();
        m_canvasGroup = transform.GetComponentInChildren<CanvasGroup>();

        m_slider.gameObject.SetActive(false);
        m_text.gameObject.SetActive(false);

        //フェードアウト
        isSceneChange.Where(_ => _ == false)
            .Subscribe(_ => {
                StartCoroutine(FadeOut(m_canvasGroup));
            });

        //フェードイン
        isSceneChange.Where(_ => _ == true)
            .Subscribe(_ => {
                StartCoroutine(SceneChange(m_canvasGroup));
            });

        //外部でシーン切り替えを取得
        isChange.Subscribe(isChange =>
        {
            isSceneChange.Value = isChange;
        });

        nextSceneNum.Delay(TimeSpan.FromSeconds(1)).Subscribe(num => 
        {
            nextSceneIndex = num;
            isChange.OnNext(true);
        });
    }

    /// <summary>
    /// フェードインとシーンのロード
    /// </summary>
    /// <param name="canvasGroup"></param>
    /// <returns></returns>
    private IEnumerator SceneChange(CanvasGroup canvasGroup)
    {
        yield return StartCoroutine(FadeIn(canvasGroup));
        yield return new WaitUntil(() => canvasGroup.alpha >= 1);
        yield return StartCoroutine(Load());
        yield return new WaitUntil(() => m_slider.IsActive() == false);
        Destroy(this.gameObject);
        yield break;
    }

    /// <summary>
    /// ロード処理
    /// </summary>
    /// <returns></returns>
    IEnumerator Load()
    {
        isLoading.Value = true;

        m_slider.gameObject.SetActive(true);
        m_text.gameObject.SetActive(true);
        yield return null;

        //ProtoTypeシーンの読み込み
        var async = SceneManager.LoadSceneAsync(nextSceneIndex);

        //読み込み中のとき
        while (!async.isDone)
        {
            //ロード率をスライダーで表示
            m_slider.value = Mathf.Clamp01(async.progress / 0.9f);
            yield return null;
        }
        yield return null;
        m_slider.gameObject.SetActive(false);
        m_text.gameObject.SetActive(false);

        isLoading.Value = false;
        yield break;
    }

    /// <summary>
    /// フェードイン
    /// </summary>
    /// <param name="canvasGroup"></param>
    public IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        m_canvasGroup.gameObject.SetActive(true);
        yield return null;
        canvasGroup.DOFade(1.0f, 1.0f);
        yield break;
    }

    /// <summary>
    /// フェードアウト
    /// </summary>
    /// <param name="canvasGroup"></param>
    public IEnumerator FadeOut(CanvasGroup canvasGroup)
    {
        canvasGroup.DOFade(0.0f, 1.0f);
        yield return new WaitUntil(() => canvasGroup.alpha <= 0);
        m_canvasGroup.gameObject.SetActive(false);
        yield break;
    }
}