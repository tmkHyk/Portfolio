using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UniRx;

namespace View
{
    public class MoveGimmickView : MonoBehaviour, IGimmickView
    {
        //CSVの値の符号
        float m_velocity;
        //初期位置
        Vector3 m_startPos;
                
        [SerializeField,Tooltip("移動の向き")]
        //上下のときtrue
        bool isMove_Updown;
        [SerializeField,Tooltip("移動速度")]
        float m_speed;
        [SerializeField,Tooltip("移動距離")]
        float m_maxPos;

        //ギミックの子オブジェクトを格納
        List<SpriteRenderer> m_list = new List<SpriteRenderer>();

        //衝突判定 trueの時衝突した
        public BoolReactiveProperty m_isHit = new BoolReactiveProperty(false);

        private void Awake()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                //SpriteRendererをもっている子オブジェクトをすべて取得
                var chi = transform.GetChild(i).GetComponentInChildren<SpriteRenderer>();
                //取得したオブジェクトをリストに格納
                if (chi != null)
                    m_list.Add(chi);
            }
            //初期位置を格納
            m_startPos = transform.localPosition;
        }

        /// <summary>
        /// フェードアウトアニメーション
        /// </summary>
        /// <param name="target">フェードするギミック</param>
        /// <returns></returns>
        public IEnumerator PlayAnim(GameObject target)
        {
            //消去開始
            for (int i = 0; i < m_list.Count; i++)
            {
                //フェードアウト
                if (m_list[i] != null)
                    m_list[i].GetComponent<SpriteRenderer>().material.color -= new Color(0, 0, 0, 0.1f);
            }
            //落下
            transform.position += new Vector3(0, -0.05f, 0);
            yield return null;
        }

        public void Update()
        {
            //移動処理
            StartCoroutine(Move());
        }

        /// <summary>
        /// 移動処理
        /// </summary>
        public IEnumerator Move()
        {
            //衝突していないとき
            if (!m_isHit.Value)
            {
                //現在位置を取得
                var pos = transform.position;
                //移動距離と速度
                var plusPos = Mathf.PingPong(Time.time * m_speed, m_maxPos);
                //CSVの値が負のとき1減算
                var plusRight = m_velocity == 1 ? 0 : -1;
                //移動
                transform.position = isMove_Updown
                    ? new Vector3(pos.x, m_startPos.y - 1 + plusPos * m_velocity, pos.z)
                    : new Vector3((plusRight + m_startPos.x - plusPos) * m_velocity, pos.y, pos.z);
            
                yield return null;
            }
        }

        /// <summary>
        /// CSVの符号を指定
        /// </summary>
        /// <param name="right">CSVの値</param>
        public void SetVelocity(float velocity)
        {
            m_velocity = velocity;
        }

        public void SetHit(bool isHit)
        {
            m_isHit.Value = isHit;
        }

        /// <summary>
        /// 衝突したか
        /// </summary>
        /// <param name="isHit">trueのとき衝突した</param>
        public BoolReactiveProperty GetHit()
        {
            return m_isHit;
        }
    }
}