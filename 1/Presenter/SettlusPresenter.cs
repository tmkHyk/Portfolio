using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System;
using UniRx;
using UniRx.Triggers;
using Model;
using View;
using System.Linq;
using UnityEngine.UI;

/// <summary>
/// 各Settlusの状態を監視するマネージャークラス
/// </summary>
public class SettlusPresenter : MonoBehaviour
{
    [SerializeField]
    CountDown m_countDown;

    [SerializeField, Range(0, 10), Tooltip("通常速度")]
    float fallSpeed_Default;
    [SerializeField, Range(0, 10), Tooltip("画面外速度")]
    float fallSpeed_Fast;
    /// <summary>
    /// 各settlusの状態を監視するリスト
    /// リストじゃないとCount=0の時の判定ができないので
    /// </summary>
    [SerializeField]
    private List<SettlusStatePresenter> settlusStateList;

    [SerializeField]
    GameObject settlusObj;

    [SerializeField]
    Image cutIn;

    [SerializeField]
    Sprite[] cutInSprite;

    private int[,] settlusPos = new int[4, 5]
    {
      { 1,0,1,0,1}, //1番下
      { 0,1,0,1,0},
      { 1,0,1,0,1},
      { 0,1,0,1,0}
    };

    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        var createList = new List<int>() { 0, 0, 1, 1, 2, 2, 3, 3, };
        createList.Add(Random.Range(0, 2));
        createList.Add(Random.Range(2, 4));

        //セトラスの複製
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                if (settlusPos[y, x] == 1)
                {
                    var gender = createList[Random.Range(0, createList.Count)];
                    createList.Remove(gender);
                    settlusObj.GetComponent<SettlusCreate>().SetGender((SettlusCreate.GENDER)gender);
                    var settlus = Instantiate(settlusObj, new Vector3(transform.position.x + x * 1.5f, transform.position.y + y * 2, 0), Quaternion.identity);
                    settlus.transform.parent = transform;
                    //リストに追加
                    settlusStateList.Add(settlus.GetComponent<SettlusStatePresenter>());
                }
            }
        }
    }

    private void Start()
    {
        m_countDown = FindObjectOfType<CountDown>();
        cutIn = GameObject.Find("CutIn").GetComponent<Image>();

        //全てのセトラスの監視処理をここで処理しておく
        foreach (var i in settlusStateList)
        {
            //死んだときにリストから削除
            i.OnDead
                .Subscribe(_ =>
                {
                    //カットインを出す
                    switch (i.caseOfDeath)
                    {
                        case SettlusStatePresenter.CaseOfDeath.Stucking:
                            cutIn.sprite = cutInSprite[0];
                            break;
                        case SettlusStatePresenter.CaseOfDeath.distress:
                            cutIn.sprite = cutInSprite[1];
                            break;
                    }
                    cutIn.GetComponent<Animator>().SetTrigger("IsDead");
                });
        }

        //最初のカウントダウンが終わったら一斉に落下を開始する
        m_countDown.IsCountDownOver.Where(isOver => isOver == true)
        .Subscribe(isOver =>
        {
            foreach (var i in settlusStateList)
            {
                i.StartFall();
            }
        });
    }

    public void PushList()
    {
     }
}
