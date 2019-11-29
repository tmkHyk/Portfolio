using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public interface IGimmickView
{
    BoolReactiveProperty GetHit(); 
    IEnumerator PlayAnim(GameObject target);  //アニメーション再生}
}