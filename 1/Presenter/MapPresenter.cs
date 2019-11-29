using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using View;

public class MapPresenter : MonoBehaviour
{
    //ギミックプレハブを格納する
    private GameObject[] m_gimmicks;
    //生成したエリアを格納する
    List<GameObject> areaList = new List<GameObject>();

    //Areaプレハブ
    [SerializeField]
    GameObject areaObj;

    private void Awake()
    {
        //Resourcesフォルダ内のギミックプレハブを格納
        m_gimmicks = Resources.LoadAll<GameObject>("Prefab/Gimmicks");

        //各エリアを生成
        SetStage(LoadAreaCSV("StageCSV"), PlayerPrefs.GetInt(StageSelectManager.STAGE_NUMBER_KEY));


        //各エリアのギミックを生成
        for (int i = 0; i < 10; i++)
        {
            //ギミックのの設定
            SetGimmick(LoadAreaCSV(areaList[i].name), areaList[i]);
        }
    }

    /// <summary>
    /// マップCSVの読み込み
    /// </summary>
    /// <returns></returns>
    List<string[]> LoadAreaCSV(string loadName)
    {
        var list = new List<string[]>();
        //スクリプトがアタッチされたオブジェクト名と同じCSVデータを取得
        var csvData = Resources.Load("CSV/" + loadName) as TextAsset;

        StringReader sr = new StringReader(csvData.text);
        while (sr.Peek() > -1)
        {
            string line = sr.ReadLine();
            list.Add(line.Split(','));
        }
        return list;
    }

    /// <summary>
    /// エリアの生成
    /// </summary>
    /// <param name="csvData"></param>
    /// <param name="stageNum"></param>
    void SetStage(List<string[]> csvData,int stageNum)
    {
        for (int i = 0; i < 10; i++)
        {
            //エリアオブジェクトを生成
            var area = Instantiate(areaObj, new Vector3(0, -10 * i + -10, 0), Quaternion.identity);
            //生成したオブジェクトの名前をCSVと同じ名前に
            area.name = "Area" + csvData[stageNum][i];
            //子オブジェクトに指定
            area.transform.parent = this.gameObject.transform;
            //リストに追加
            areaList.Add(area.gameObject);
        }
    }

    /// <summary>
    /// CSVからマップの設定
    /// </summary>
    /// <param name="csvData">CSVのStringデータ</param>
    void SetGimmick(List<string[]> csvData,GameObject parentObj)
    {
        for (int y = 0; y < 10; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                //rotationの設定 1文字目が-なら反転
                var rot = csvData[y][x].Contains("-") ? Quaternion.AngleAxis(180, Vector3.up) : Quaternion.identity;
                //positionの設定
                var pos = transform.position + new Vector3(x, -y);
                //実際の値のみ引き出す(-を引いた文字列)
                var mapString = csvData[y][x].Contains("-") ? csvData[y][x].Substring(1, 2) : csvData[y][x];

                //ギミックオブジェクトの生成
                foreach (var g in m_gimmicks)
                {
                    //リソースから対象のプレハブで生成
                    if (g.name.Substring(0, 2) == mapString)
                    {
                        var obj = Instantiate(g, new Vector3(pos.x-3.5f,pos.y+parentObj.transform.position.y,pos.z), rot);
                        obj.transform.parent = parentObj.transform;

                        //生成したプレハブがMoveFloorViewを持っていたら
                        if (obj.GetComponent<MoveGimmickView>())
                        {
                            //移動方向を指定
                            var plus = csvData[y][x].Contains("-") ? -1 : 1;
                            obj.GetComponent<MoveGimmickView>().SetVelocity(plus);
                        }
                        if (obj.GetComponent<WindHole>())
                        {
                            var plus = csvData[y][x].Contains("-") ? true : false;
                            obj.GetComponent<WindHole>().SetState(true, plus);
                        }
                    }
                }
            }
        }
    }
}
