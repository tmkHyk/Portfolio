using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UniRx;

namespace View
{
    public class BrokenGimmickView : MonoBehaviour, IGimmickView
    {
        //ギミックの子オブジェクトを格納
        public List<SpriteRenderer> m_list = new List<SpriteRenderer>();

        //衝突判定 trueの時衝突した
        public BoolReactiveProperty m_isHit = new BoolReactiveProperty(false);

        /// <summary>
        /// 初期化
        /// </summary>
        public void Awake()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                //SpriteRendererをもっている子オブジェクトをすべて取得
                var chi = transform.GetChild(i).GetComponentInChildren<SpriteRenderer>();
                //取得したオブジェクトをリストに格納
                if (chi != null)
                    m_list.Add(chi);
            }
        }

        /// <summary>
        /// フェードアウトアニメーション
        /// </summary>
        /// <param name="target">フェードするオブジェクト</param>
        /// <returns></returns>
        public IEnumerator PlayAnim(GameObject target)
        {
            //消去開始
            for (int i = 0; i < m_list.Count; i++)
            {
                //フェードアウト
                m_list[i].material.color -= new Color(0,0,0,0.1f);
            }
            //落下
            transform.position += new Vector3(0, -0.05f, 0);
            yield return null;
        }

        public BoolReactiveProperty GetHit()
        {
            return m_isHit;
        }
    }
}