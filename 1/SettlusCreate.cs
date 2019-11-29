using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SettlusCreate : MonoBehaviour
{
    //画像データ
    private Sprite[] hairSprite;
    private Sprite[] faceSprite;
    private Sprite[] headSprite;
    public Sprite[] bodySprite;

    //子オブジェクト
    private SpriteRenderer[] settlusParts = new SpriteRenderer[4];

    //各パーツ
    enum PARTS
    {
        HAIR = 0,
        FACE,
        HEAD,
        BODY
    }

    //性別と年齢
    public enum GENDER
    {
        //男
        BOY = 0,
        MALE,
        //女
        GIRL,
        FEMALE
    }

    public enum BODY
    {
        SLIM = 0,
        NORMAL,
        FAT
    }

    public struct Length
    {
        public int min;
        public int max;
    }

    [SerializeField]
    private GENDER gender;
    [SerializeField]
    private BODY body;

    void Start()
    {
        //Rsourcesフォルダからの読み込み
        hairSprite = Resources.LoadAll<Sprite>("Settlus/Sett_Hair");
        faceSprite = Resources.LoadAll<Sprite>("Settlus/Sett_Face");
        headSprite = Resources.LoadAll<Sprite>("Settlus/Sett_Head");
        bodySprite = Resources.LoadAll<Sprite>("Settlus/Sett_Body");

        for (int i = 0; i < transform.childCount - 1; i++)
        {
            //子オブジェクト読み込み
            settlusParts[i] = transform.GetChild(i).GetComponent<SpriteRenderer>();

            //各パーツごとの画像指定
            switch (i)
            {
                case (int)PARTS.HAIR:
                    //髪の画像指定
                    SetSprite(i, hairSprite, SetHair(gender).min, SetHair(gender).max);
                    break;
                case (int)PARTS.FACE:
                    SetSprite(i, faceSprite, 0, 8);
                    break;
                case (int)PARTS.HEAD:
                    //Bodyと同じ値の画像を指定
                    body = (BODY)SetSprite(i, headSprite, 0, 2);
                    break;
                case (int)PARTS.BODY:
                    //体の画像指定
                    SetSprite(i, bodySprite, SetBody(gender, body).min, SetBody(gender, body).max);
                    break;
            }
        }
    }

    /// <summary>
    /// 画像指定
    /// </summary>
    /// <param name="parts">どのパーツか(PARTS)</param>
    /// <param name="sprites">どのパーツか(配列)</param>
    /// <param name="number">配列の要素数</param>
    int SetSprite(int parts, Sprite[] sprites, int min,int max)
    {
        //ランダムで画像指定
        var rand = Random.Range(min, max + 1);
        settlusParts[parts].sprite = sprites[rand];
        return rand;
    }

    Length SetHair(GENDER gender)
    {
        switch (gender)
        {
            case GENDER.BOY:
                return SetLength(0, 1);
            case GENDER.MALE:
                return SetLength(2, 4);
            case GENDER.GIRL:
                return SetLength(5, 6);
            case GENDER.FEMALE:
                return SetLength(7, 9);
        }
        return SetLength(0, 0);
    }

    Length SetBody(GENDER gender,BODY body)
    {
        if (gender == GENDER.BOY || gender == GENDER.MALE)
        {
            switch (body)
            {
                case BODY.SLIM:
                    return SetLength(0, 1);
                case BODY.NORMAL:
                    return SetLength(2, 3);
                case BODY.FAT:
                    return SetLength(4, 5);
            }
        }
        if (gender == GENDER.GIRL || gender == GENDER.FEMALE)
        {
            switch (body)
            {
                case BODY.SLIM:
                    return SetLength(6, 7);
                case BODY.NORMAL:
                    return SetLength(8, 9);
                case BODY.FAT:
                    return SetLength(10, 11);
            }
        }
        return SetLength(0, 0);
    }

    Length SetLength(int min, int max)
    {
        Length length;
        length.min = min;
        length.max = max;
        return length;
    }

    int GetBody(int bodyIndex)
    {
        var slim = new int[] { 0, 1, 6, 7 };
        var normal = new int[] { 2, 3, 8, 9 };
        var fat = new int[] { 4, 5, 10, 11 };
        for(int i=0;i<4;i++)
        {
            if (slim[i] == bodyIndex)
                return (int)BODY.SLIM;
            if (normal[i] == bodyIndex)
                return (int)BODY.NORMAL;
            if (fat[i] == bodyIndex)
                return (int)BODY.FAT;
        }
        return 0;
    }

    /// <summary>
    /// デバッグ ボタン用
    /// </summary>
    public void Reload()
    {
        Start();
    }

    public void SetGender(GENDER gender)
    {
        this.gender = gender;
    }

    public GENDER GetGender()
    {
        return gender;
    }
}
