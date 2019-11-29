using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;

public class ClockPresenter : MonoBehaviour
{
    [SerializeField]
    ClockProperty m_timer;

    //時間テキスト
    private Text m_timerText;
    //月日テキスト
    private Text m_dateText;

    private void Awake()
    {
        m_timer = new ClockProperty();
        m_timerText = GameObject.Find("TimeText").GetComponent<Text>();
        m_dateText = GameObject.Find("DateText").GetComponent<Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //時間テキストの更新
        m_timer.nowTime.Subscribe(time => 
        {
            if(m_timerText != null)
            m_timerText.text = m_timer.nowTime.Value;
        });

        //月日テキストの更新
        m_timer.nowDate.Subscribe(date => 
        {
            if(m_timerText != null)
            m_dateText.text = m_timer.nowDate.Value;
        });

        //現在の日時を取得
        Observable.EveryUpdate().Subscribe(_ => 
        {
            DateTime now = DateTime.Now;
            m_timer.SetTime(now.Hour.ToString("00") + ":" + now.Minute.ToString("00"));
            m_timer.SetDate(now.Month.ToString("00") + "月" + now.Day.ToString("00") + "日(" + GetDay(now.DayOfWeek) + ")");
        });
    }

    /// <summary>
    /// 曜日を日本語で指定
    /// </summary>
    /// <param name="day">取得した曜日</param>
    /// <returns></returns>
    string GetDay(DayOfWeek day)
    {
        switch (day)
        {
            case DayOfWeek.Sunday:
                return "日";
            case DayOfWeek.Monday:
                return "月";
            case DayOfWeek.Tuesday:
                return "火";
            case DayOfWeek.Wednesday:
                return "水";
            case DayOfWeek.Thursday:
                return "木";
            case DayOfWeek.Friday:
                return "金";
            case DayOfWeek.Saturday:
                return "土";
        }
        return "";
    }
}
