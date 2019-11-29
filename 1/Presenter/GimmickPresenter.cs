using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using View;
using Model;
using System;
using System.IO;

public class GimmickPresenter : MonoBehaviour
{
    [Header("View")]
    IGimmickView view;

    [Header("Model")]
    Gimmick gimmick = new Gimmick();

    private void Start()
    {
        //GetHit();
    }

    // Update is called once per frame
    void Update()
    {
        if (gimmick.GetHit())
            StartCoroutine(SetAnim());
    }
    
    ////動かない
    //void GetHit()
    //{
    //    //衝突したら
    //    gimmick.GetHit()
    //        .Where(isHit => isHit)
    //        .Subscribe(isHit =>
    //        {
    //            Observable.FromCoroutine(SetAnim)
    //            .Subscribe(x => Debug.Log("end"));
    //        });
    //}
    
    /// <summary>
    /// パラメータ設定
    /// </summary>
    /// <param name="target">衝突したギミック</param>
    /// <param name="isHit">衝突判定</param>
    public void SetParameter(GameObject target, bool isHit)
    {
        gimmick.SetTarget(target);
        gimmick.SetHit(isHit);
    }

    /// <summary>
    /// アニメーションを再生、削除、初期化
    /// </summary>
    /// <param name="target">アニメーションをするギミック</param>
    /// <returns></returns>
    IEnumerator SetAnim()
    {
        var target = gimmick.GetTarget();

        if (target == null)
            yield break;

        view = target.GetComponent<WindHole>() ? null : target.GetComponent<IGimmickView>();

        SetViewAction(view, () =>
         {
             //消滅アニメーションを開始
             StartCoroutine(view.PlayAnim(target));
             //MoveFloorViewを持っているギミックは移動を停止
             //ここでエラーが起きる
             if (target.GetComponent<MoveGimmickView>())
                 target.GetComponent<MoveGimmickView>().SetHit(true);
         });
        //対象のギミックのBodyとPointのコライダーを停止
        for (int i = 0; i < target.transform.childCount; i++)
        {
            if (target.transform.GetChild(i).gameObject.tag == "Body"
                || target.transform.GetChild(i).gameObject.tag == "Point")
            {
                target.transform.GetChild(i).gameObject.GetComponent<Collider2D>().enabled = false;
            }
        }
        yield return null;
        yield return new WaitForSeconds(0.3f);
        Destroy(target);
        yield break;
    }

    /// <summary>
    /// view削除後のエラーを防ぐためのメソッド
    /// </summary>
    /// <param name="view">削除対象のview</param>
    /// <param name="callback">処理</param>
    void SetViewAction(IGimmickView view, Action callback)
    {
        try
        {
            if (view != null)
            {
                callback();
            }
        }
        catch
        {
        }
    }
}