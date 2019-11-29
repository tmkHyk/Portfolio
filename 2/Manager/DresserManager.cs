using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class DresserManager : MonoBehaviour
{
    //現在のルーム画像の配列番号
    private static IntReactiveProperty m_roomSprite = new IntReactiveProperty();
    public static IntReactiveProperty roomSprite { get { return m_roomSprite; } }
    //ルームイメージ
    Image roomImage;

    private void Start()
    {
        //ヒエラルキーのルームイメージを格納
        roomImage = GameObject.Find("Room").GetComponent<Image>();
        //Resourcesフォルダからルーム画像を取得
        var roomSprite = Resources.LoadAll<Sprite>("Sprite");

        //ルーム画像の配列番号が変更されたら更新
        m_roomSprite.Subscribe(_ => 
        {
            //ルームイメージを指定された画像に変更
            roomImage.sprite = roomSprite[m_roomSprite.Value];
        });
    }

    /// <summary>
    /// ルームイメージの指定
    /// </summary>
    /// <param name="index"></param>
    public void SetRoom(int index)
    {
        m_roomSprite.Value = index;
    }
}
