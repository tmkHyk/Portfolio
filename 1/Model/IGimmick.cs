using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public interface IGimmick
{
    GameObject SetTarget(GameObject target);  //衝突したギミックを設定
    GameObject GetTarget();  //衝突したギミックを返す
}