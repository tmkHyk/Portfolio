using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class BrokenGimmick : MonoBehaviour
{
    //ギミックの子オブジェクトを格納
    public List<SpriteRenderer> m_list = new List<SpriteRenderer>();
    public SpriteRenderer[] childObj;

    // Start is called before the first frame update
    void Start()
    {
        childObj = transform.GetComponentsInChildren<SpriteRenderer>();

        this.OnTriggerEnter2DAsObservable()
            .Subscribe(other =>
            {
                Debug.Log("衝突された");
                StartCoroutine(PlayAnim());
            });

        this.OnCollisionEnter2DAsObservable()
            .Subscribe(other =>
            {
                Debug.Log("衝突された");
                StartCoroutine(PlayAnim());
            });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// フェードアウトアニメーション
    /// </summary>
    /// <returns></returns>
    public IEnumerator PlayAnim()
    {
        //消去開始
        for (int i = 0; i < m_list.Count; i++)
        {
            //フェードアウト
            childObj[i].material.color -= new Color(0, 0, 0, 0.1f);
        }
        //落下
        transform.position += new Vector3(0, -0.05f, 0);
        yield return null;

        Destroy(this.gameObject, 1.0f);
        yield break;
    }
}
