using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Model
{
    public class Gimmick : IGimmick
    {
        //衝突したギミックを格納する変数
        GameObject m_target;
        //ReactiveProperty<GameObject> m_target;

       // bool m_isHit;
        BoolReactiveProperty m_isHit = new BoolReactiveProperty();

        /// <summary>
        /// 衝突したギミックを指定
        /// </summary>
        /// <param name="target">対象のギミック</param>
        /// <returns></returns>
        public GameObject SetTarget(GameObject target)
        {
            return m_target = target;
        }

        /// <summary>
        /// 衝突したギミックを返す
        /// </summary>
        /// <returns></returns>
        public GameObject GetTarget()
        {
            return m_target;
        }

        public void SetHit(bool isHit)
        {
            m_isHit.Value = isHit;
        }

        public bool GetHit()
        {
            return m_isHit.Value;
        }
    }
}