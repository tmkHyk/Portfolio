using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
/// <summary>
/// 管理するのは移動と早く移動する処理
/// 死んだときにイベントを通知する処理
/// </summary>
public class SettlusStatePresenter : MonoBehaviour
{
    /// <summary>
    /// Settulsの状態
    /// </summary>
    public enum SettlusState
    {
        StandBy,
        Moving,
        Dead,
    }

    public enum CaseOfDeath
    {
        Stucking,
        distress,
    }

    [SerializeField, Range(0, 10), Tooltip("通常速度")]
    private float fallSpeedDefault = 2;
    [SerializeField, Range(0, 10), Tooltip("画面外速度")]
    private float fallSpeedFast = 5;

    public float startDistance;
    private Rigidbody2D rigidBody2D;
    private Collider2D collider2D;
    private Animator animator;

    private GameObject player;
    /// <summary>
    /// Settlusの現在状態
    /// </summary>
    public SettlusState currentState;

    /// <summary>
    /// イベントを発火（出す方）の変数
    /// publicにしないのは出すのが外部から出来てしまうため。
    /// </summary>
    private Subject<Unit> onDead = new Subject<Unit>();
    /// <summary>
    /// イベントを監視する側が見れるやつ
    /// </summary>
    public IOptimizedObservable<Unit> OnDead => onDead;

    private Subject<Unit> onWind = new Subject<Unit>();
    public IOptimizedObservable<Unit> OnWind => onWind;

    public float currentDistance;

    public CaseOfDeath caseOfDeath;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        rigidBody2D = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
        //初期距離をここで持っておく
        startDistance = Mathf.Abs(transform.position.y - player.transform.position.y);
        currentState = SettlusState.StandBy;

        //浮遊アニメーションの開始時間をランダムでまばらに
        animator = GetComponent<Animator>();
        animator.enabled = false;
        float rand = Random.Range(0, 3);

        //移動処理 （FixedUpdate　物理挙動を使用するので）
        Observable.EveryFixedUpdate()
            //↓gameObjectが削除されたら実行を止めるやつ（ないとDestroy後も動くのでエラーが出る）
            .TakeUntilDestroy(gameObject)
            .Where(_ => currentState == SettlusState.Moving)
            .Subscribe(_ =>
            {
                //アニメーション開始時間をまばらに
                rand -= Time.deltaTime;
                if (rand <= 0)
                    animator.enabled = true;

                //マイナスだったらプレイヤーよりも下に行ってる
                currentDistance = transform.position.y - player.transform.position.y;
                if (currentDistance >= startDistance + 15.0f)
                {
                    rigidBody2D.velocity = new Vector2();
                    currentState = SettlusState.Dead;
                    caseOfDeath = CaseOfDeath.distress;
                    //死んだイベントが出される
                    onDead.OnNext(Unit.Default);
                }
                if (currentDistance < 0)
                {
                    rigidBody2D.velocity = new Vector3(0, -fallSpeedDefault, 0);
                    return;
                }
                //初期の距離よりも離れていればfast
                if (currentDistance > startDistance)
                {
                    rigidBody2D.velocity = new Vector3(0, -fallSpeedFast, 0);
                }
                else
                {
                    rigidBody2D.velocity = new Vector3(0, -fallSpeedDefault, 0);
                }
            });

        //衝突イベント
        this.OnTriggerEnter2DAsObservable()
            .TakeWhile(_ => currentState != SettlusState.Dead)
            .Subscribe(other =>
            {
                //BodyとWindオブジェクト以外ははじく
                if (other.tag != "Body" && !other.name.Contains("Wind"))
                    return;

                if (other.name.Contains("Wind"))
                {
                    onWind.OnNext(Unit.Default);
                    return;
                }

                //Areaでparameterを設定
                var target = other.transform.parent.gameObject;
                target.transform.parent.GetComponent<GimmickPresenter>().SetParameter(target, true);

                //ここで速度を0にしないと0にできないため
                rigidBody2D.velocity = new Vector2();
                currentState = SettlusState.Dead;
                caseOfDeath = CaseOfDeath.Stucking;
                //死んだイベントが出される
                onDead.OnNext(Unit.Default);
            });

        onDead.Subscribe(_ =>
        {
            //死亡アニメーション
            StopFall();
            animator.SetTrigger("IsDead");
            Destroy(gameObject, 0.5f);
        });

        onWind.Subscribe(_ => { });
    }

    /// <summary>
    /// 落下開始
    /// </summary>
    public void StartFall()
    {
        currentState = SettlusState.Moving;
    }

    public void SetDistance(int i)
    {
        startDistance -= 2;
    }

    public void StopFall()
    {
        rigidBody2D.velocity = Vector2.zero;
    }

    public CaseOfDeath GetCaseOfDeath()
    {
        return caseOfDeath;
    }
}
