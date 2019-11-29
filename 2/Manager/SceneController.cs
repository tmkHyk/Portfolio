using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;
using System;

public class SceneController : MonoBehaviour
{
    //BuildIndex
    enum SceneName
    {
        HOME = 0,
        TIMER,
        DRESSER
    }

    //Fadeを格納
    private Fade fade;
    //タッチされているオブジェクトを格納
    private GameObject target;

    private void Awake()
    {
        //Fadeを
        fade = FindObjectOfType<Fade>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //フェードアウト
        FadeOut();

        //タッチまたはクリックされたら更新
        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonDown(0) || ( Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            .Subscribe(_ => 
            {
                Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(Input.mousePosition));
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction*10, 10);
                if (hit)
                {
                    target = hit.transform.gameObject;

                    //衝突したオブジェクト名がClockならTimerシーンへ移行
                    if (target.name == "Clock")
                        FadeIn((int)SceneName.TIMER);
                    //衝突したオブジェクト名がDresserならDresserオブジェクトをアクティブ化
                    if (target.name == "Dresser")
                        FindObjectOfType<HomeManager>().isActiveDresser.Value = true;
                }
            });
    }

    /// <summary>
    /// フェードイン
    /// </summary>
    [EnumAction(typeof(SceneName))]
    public void FadeIn(int nextLoadSceneIndex)
    {
        fade.FadeIn(0.5f, () => StartCoroutine(Load(nextLoadSceneIndex)));
    }

    /// <summary>
    /// フェードアウト
    /// </summary>
    public void FadeOut()
    {
        fade.FadeOut(0.5f, () => Debug.Log(""));
    }

    /// <summary>
    /// 次のシーンをロード
    /// </summary>
    /// <param name="buildIndex"></param>
    /// <returns></returns>
    IEnumerator Load(int buildIndex)
    {
        var asyn = SceneManager.LoadSceneAsync(buildIndex);

        while (!asyn.isDone)
        {
            yield return null;
        }
        yield break;
    }
}
