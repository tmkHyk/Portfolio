using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;
using Unity.Notifications.Android;

public class AlarmPresenter : MonoBehaviour
{
    //ヒエラルキーのアラームテキストを格納
    private Text m_alarmText;
    //ヒエラルキーのInputFieldのテキストを各々格納
    private Text m_hourText, m_minuteText;
    //ヒエラルキーのInputFieldHourのPlaceholderを各々格納
    private Text m_hourPlaceholder, m_minutePlaceholder;
    //ヒエラルキーのeditPanelを格納
    private GameObject m_editPanel;
    
    //アラーム時刻に指定した時間を各々格納
    private int m_hour, m_minute;
    //editPanelがアクティブか trueの時アクティブ
    BoolReactiveProperty m_isActive = new BoolReactiveProperty(false);

    //通知用のチャンネルID
    private string m_chandelId = "Alarm";

    private void Awake()
    {
        //通知用のチャンネルを生成
        var channel = new AndroidNotificationChannel
        {
            Id = m_chandelId,
            Name = "name",
            Importance = Importance.High,
            Description = "description",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    // Start is called before the first frame update
    void Start()
    {
        //ヒエラルキーから取得
        m_alarmText = GameObject.Find("AlarmText").GetComponent<Text>();
        m_hourText = GameObject.Find("InputFieldHour/Text").GetComponent<Text>();
        m_minuteText = GameObject.Find("InputFieldMinute/Text").GetComponent<Text>();
        m_hourPlaceholder = GameObject.Find("InputFieldHour/Placeholder").GetComponent<Text>();
        m_minutePlaceholder = GameObject.Find("InputFieldMinute/Placeholder").GetComponent<Text>();
        m_editPanel = GameObject.Find("EditPanel");

        //アラーム時刻を格納
        m_hour = PlayerPrefs.GetInt("HOUR", 0);
        m_minute = PlayerPrefs.GetInt("MINUTE", 0);

        //EditPanelのアクティブ化非アクティブ化
        m_isActive
            .Subscribe(active =>
        {
            m_editPanel.SetActive(active);
            SetAlarm();
        });
    }

    /// <summary>
    /// アラームテキストの設定
    /// </summary>
    void SetAlarm()
    {
        m_alarmText.text =
            m_hour.ToString("00") + ":" + m_minute.ToString("00");
    }

    /// <summary>
    /// InputFieldHourのPlaceholderを現在の時刻に設定
    /// </summary>
    public void Edit()
    {
        var now = DateTime.Now;
        m_hourPlaceholder.text = now.Hour.ToString("00");
        m_minutePlaceholder.text = now.Minute.ToString("00");
        m_isActive.Value = true;
    }

    /// <summary>
    /// アラーム時刻の設定
    /// ボタンで使用
    /// </summary>
    public void Enter()
    {
        var hour = m_hourText.text != "" ? int.Parse(m_hourText.text) : m_hour;
        m_hour = 0 <= hour && hour <= 23 ? hour : m_hour;

        var minute = m_minuteText.text != "" ? int.Parse(m_minuteText.text) : m_minute;
        m_minute = 0 <= minute && minute <= 59 ? minute : m_minute;

        //PlayerPrefsに保存
        PlayerPrefs.SetInt("HOUR", m_hour);
        PlayerPrefs.SetInt("MINUTE", m_minute);
        PlayerPrefs.Save();

        //EditPanelを非アクティブ化
        m_isActive.Value = false;
    }

    /// <summary>
    /// 通知
    /// </summary>
    /// <param name="pause"></param>
    void OnApplicationPause(bool pause)
    {
        if (pause)
            Push();
    }

    /// <summary>
    /// 通知までの時間を計算
    /// </summary>
    /// <returns></returns>
    int GetTimer()
    {
        var now = DateTime.Now;

        var hour = now.Hour <= m_hour
            ? m_hour - now.Hour : m_hour + (24 - now.Hour);
        var minute = now.Minute <= m_minute
            ? m_minute - now.Minute - 1 : m_minute + (59 - now.Minute) - 1;
        var secound = 60 - now.Second;

        return hour * 3600 + minute * 60 + secound;
    }

    /// <summary>
    /// 通知内容の設定
    /// </summary>
    public void Push()
    {
        var time = GetTimer();

        var n = new AndroidNotification
        {
            Title = "アラーム", //通知のタイトル
            Text = m_hour + ":" + m_minute,　//通知の本文
            FireTime = DateTime.Now.AddSeconds(time), //通知までの時間
        };

        AndroidNotificationCenter.SendNotification(n, m_chandelId);
    }
}
