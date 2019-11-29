using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ClockProperty : MonoBehaviour
{
    //現在の時間を格納
    private StringReactiveProperty m_time = new StringReactiveProperty();
    public StringReactiveProperty nowTime { get { return m_time; } }

    //現在の月日を格納
    private StringReactiveProperty m_date = new StringReactiveProperty();
    public StringReactiveProperty nowDate { get { return m_date; } }

    /// <summary>
    /// 現在の時間を指定
    /// </summary>
    /// <param name="time"></param>
    public void SetTime(string time)
    {
        m_time.Value = time;
    }

    /// <summary>
    /// 現在の月日を指定
    /// </summary>
    /// <param name="date"></param>
    public void SetDate(string date)
    {
        m_date.Value = date;
    }
}
