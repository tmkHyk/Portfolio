using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class HomeManager : MonoBehaviour
{
    /// <summary>
    /// ルーム画像関連
    /// </summary>
    //ルーム画像の位置パターン{left,right}を格納する配列
    private float[,] m_roomPosition = new float[4, 2]
            {
                {0,2400 },
                {-800,1600 },
                {-1600,800 },
                {-2400,0 },
            };
    //現在表示されているroomスプライト
    private Image roomImage;
    [SerializeField, Range(1, 10), Tooltip("移動速度")]
    private float m_speed;
    //表示するルームの位置
    private IntReactiveProperty m_index = new IntReactiveProperty(1);

    /// <summary>
    /// Dresser関連
    /// </summary>
    //Dresserオブジェクトを格納
    GameObject dresser;
    //Dresserがアクティブか trueのときアクティブ
    private BoolReactiveProperty m_isActiveDresser = new BoolReactiveProperty();
    public BoolReactiveProperty isActiveDresser { get { return m_isActiveDresser; } }

    // Start is called before the first frame update
    void Start()
    {
        //ヒエラルキーのルームイメージを格納
        roomImage = GameObject.Find("Room").GetComponent<Image>();
        //Resourcesフォルダからルーム画像を取得
        var roomSprites = Resources.LoadAll<Sprite>("Sprite");

        //ヒエラルキーからDresserオブジェクトを取得
        dresser = GameObject.Find("DresserPanel");
        //最初は非アクティブ
        dresser.SetActive(false);

        //ルームの位置パターンが変更されたら
        m_index.Subscribe(index =>
        {
            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    if(roomImage != null)
                        //ルームの移動
                        MoveRoomImage(m_index.Value, m_speed);
                });
        });

        //Dresserのアクティブ化非アクティブ化を指定
        m_isActiveDresser.Subscribe(_ => 
        {
            dresser.SetActive(m_isActiveDresser.Value);
        });
    }

    /// <summary>
    /// ルームの移動処理
    /// </summary>
    /// <param name="index"></param>
    /// <param name="speed"></param>
    void MoveRoomImage(int index,float speed)
    {
        //top:max.y bottom:min.y right:max.x left:min.x
        var nowRectPos = roomImage.rectTransform;
        if (nowRectPos.offsetMin.x != m_roomPosition[index, 0])
        {
            nowRectPos.offsetMin = Vector2.MoveTowards(nowRectPos.offsetMin, new Vector2(m_roomPosition[index, 0], 150f), speed);
            nowRectPos.offsetMax = Vector2.MoveTowards(nowRectPos.offsetMax, new Vector2(m_roomPosition[index, 1], -100f), speed);
        }
    }

    /// <summary>
    /// ルームの移動方向を指定
    /// ボタンで使用
    /// </summary>
    /// <param name="velocity"></param>
    public void MoveRoom(int velocity)
    {
        m_index.Value = Mathf.Clamp(m_index.Value + velocity, 0, 3);
    }

    /// <summary>
    /// Dresserの非アクティブ化
    /// ボタンで使用
    /// </summary>
    public void SetDresser()
    {
        isActiveDresser.Value = false;
    }
}
