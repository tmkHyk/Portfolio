using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class Distance : MonoBehaviour
{
    //アクティブなセトラスを格納するリスト
    public List<SettlusStatePresenter> settlusList = new List<SettlusStatePresenter>();

    //プレイヤーオブジェクト
    GameObject player;
    //最も速いセトラスオブジェクト
    GameObject fastSettlus;

    //各スライダー
    Slider playerSlider, settlusSlider;

    //// Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerSlider = GameObject.Find("playerSlider").GetComponent<Slider>();
        settlusSlider = GameObject.Find("settlusSlider").GetComponent<Slider>();

        //各スライダーのMAX値(総距離)を指定
        playerSlider.maxValue = 100;
        settlusSlider.maxValue = 100;
        //各スライダーの初期値を0に
        playerSlider.value = 0;
        settlusSlider.value = 0;

        //アクティブなセトラスを全て格納
        foreach (var settlus in FindObjectsOfType<SettlusStatePresenter>())
        {
            settlusList.Add(settlus);
        }

        Observable.EveryUpdate()
            .Subscribe(_ =>
            {
                //プレイヤーの現在位置をスライダーの値として代入
                playerSlider.value = Mathf.Clamp(player.transform.position.y * -1, 5, 100);

                //最も速いセトラスを取得
                GetFastSettlus(settlusList);
                //最も速いセトラスの現在位置をスライダーの値として代入
                settlusSlider.value = Mathf.Clamp(fastSettlus.transform.position.y * -1, 0, 100);
            }).AddTo(this);

        foreach (var settlus in settlusList)
        {
            //セトラスが死んだら
            settlus.OnDead.Subscribe(_ => 
            {
                //リストから削除
                settlusList.Remove(settlus);
            });
        }
    }

    /// <summary>
    /// セトラスリストの読み込み
    /// </summary>
    void GetSettluses()
    {
        //リストを一度クリア
        settlusList.Clear();
        //アクティブなセトラスを格納
        foreach(var settlus in FindObjectsOfType<SettlusStatePresenter>())
        {
            settlusList.Add(settlus);
        }
    }

    /// <summary>
    /// 最も速いセトラスオブジェクトを取得
    /// </summary>
    /// <param name="settluses"></param>
    void GetFastSettlus(List<SettlusStatePresenter> settluses)
    {
        foreach (var settlus in settluses)
        {
            //現在位置を取得し、最も下にいるセトラスをfastSettlusに
            if (fastSettlus == null || fastSettlus.transform.position.y > settlus.transform.position.y)
                fastSettlus = settlus.gameObject;
        }
    }
}