using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] sceneObjs;

    private void Awake()
    {
        SetScene(sceneObjs[0]);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    /// <summary>
    /// ClockPanelをアクティブ化
    /// ボタンで使用
    /// </summary>
    public void SetClock()
    {
        SetScene(sceneObjs[0]);
    }

    /// <summary>
    /// AlarmPanelをアクティブ化
    /// ボタンで使用
    /// </summary>
    public void SetAlarm()
    {
        SetScene(sceneObjs[1]);
    }

    /// <summary>
    /// sceneObjをアクティブ化、それ以外は非アクティブ化
    /// </summary>
    /// <param name="sceneObj">アクティブにしたいPanelオブジェクト</param>
    void SetScene(GameObject sceneObj)
    {
        foreach (var obj in sceneObjs)
        {
            if (obj == sceneObj)
                obj.SetActive(true);
            else
                obj.SetActive(false);
        }
    }
}
