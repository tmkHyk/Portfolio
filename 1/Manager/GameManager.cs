using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using View;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    PlayerPresenter playerPresenter;
    [SerializeField]
    SystemPresenter systemPresenter;
    Transform player_Pos;

    [SerializeField]
    List<GameObject> settluses;
    IntReactiveProperty settlusCount;
    IntReactiveProperty surviveSettlusCount;
    Subject<Unit> aliveSettlusAllClear = new Subject<Unit>();

    [SerializeField]
    GameObject game_clear, game_over;
    [SerializeField]
    GameObject debugCanvas;

    [Header("セトラス全員生還SEList")]
    [SerializeField]
    private AudioClip[] perfectClearSE;
    public Dictionary<int, string> clearCommentDic = new Dictionary<int, string>();
    private TextLoader textLoader;
    private string commentText;


    private SystemSEManager systemSEManager;


    private void Awake()
    {
        player_Pos = GameObject.Find("Player").transform;

        game_clear = GameObject.Find("GameClear");
        game_over = GameObject.Find("GameOver");
        game_clear.SetActive(false);
        game_over.SetActive(false);

        debugCanvas = GameObject.Find("DebugCanvas");
        textLoader = new TextLoader("PerfectClearText");
        SetClearComment();

        systemSEManager = FindObjectOfType<SystemSEManager>();
    }

    private void SetClearComment()
    {
        for (int i = 0; i < perfectClearSE.Length; i++)
        {
            clearCommentDic.Add(i, textLoader.textWords[i, 0]);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        settluses = new List<GameObject>(GameObject.FindGameObjectsWithTag("Settlus"));//Awakeでやると０になる
        settlusCount = new IntReactiveProperty(settluses.Count);
        surviveSettlusCount = new IntReactiveProperty();

        GameClear();
        GameOver();
    }

    // Update is called once per frame
    void Update()
    {
        RemoveDeadSettlus();
    }

    void GameClear()
    {
        playerPresenter.playerClear.First().Subscribe(_ =>
        {
            Debug.Log("GameClear");
            systemPresenter.SetGameState(GameState.GameClear);
            game_clear.SetActive(true);
            CheckSurviveSettlusCount();
            game_clear.GetComponent<GameClear>().setResult.OnNext(settlusCount.Value);
            debugCanvas.SetActive(false);

            //shigu追加
            foreach (var gimmick in GameObject.FindGameObjectsWithTag("Gimmick"))
            {
                Destroy(gimmick);
            }
        });
    }

    void GameOver()
    {
        //プレイヤー死亡
        playerPresenter.GetHP().Where(hp => hp == 0)
        .Subscribe((System.Action<int>)(_ =>
        {
            playerPresenter.SetIsDead(true);
            playerPresenter.SetState(PlayerState.Dead);
            systemPresenter.SetGameState((GameState)GameState.GameOver);
        }));
        //settlus全滅
        settlusCount
        .Where(count => count <= 0 &&
                        systemPresenter.GetGameState_RP().Value != GameState.GameClear)
        .Subscribe((System.Action<int>)(_ =>
        {
            playerPresenter.PlaySE(PlayerGameSE.GameOver_SettlusDead);
            systemPresenter.SetGameState((GameState)GameState.GameOver);
        }));
        //GAMEOVER
        //①プレイヤー死亡
        systemPresenter.GetGameState_RP().Where(state => state == GameState.GameOver)
        .First()
        .Subscribe(_ =>
        {
            GameOverUI();
        });
    }

    private void GameOverUI()
    {
        game_over.SetActive(true);
        game_over.GetComponent<GameOver>().gameOverEvent.OnNext(Unit.Default);
        debugCanvas.SetActive(false);
    }

    void RemoveDeadSettlus()
    {
        for (int i = 0; i < settluses.Count; i++)
        {
            if (settluses[i] == null)
            {
                settluses.RemoveAt(i);
                settlusCount.Value = settluses.Count;
            }
        }
    }

    /// <summary>
    /// 生還したセトラスの数をチェック
    /// </summary>
    private void CheckSurviveSettlusCount()
    {
        for (int i = 0; i < settluses.Count; i++)
        {
            //クリアリスト作って格納
            //同じものがあったら入れない
            //生存数とクリア数が同じになったら
            //aliveSettlusAllClear着火
            if (settluses[i] != null)
            {
                if(settluses[i].transform.position.y <= -100)
                {
                    surviveSettlusCount.Value++;
                }
            }
        }
    }

    public void Title()
    {
        SceneManager.LoadScene("Title");
    }

    public void Retry()
    {
        SceneManager.LoadScene("Load");
    }

    public void PlaySystemSE(SystenSE se)
    {
        if(se == SystenSE.PerfectClear)
        {
            int rndClearSE = Random.Range(0, perfectClearSE.Length);
            systemSEManager.PlaySEWithClip(perfectClearSE[rndClearSE]);
            commentText = clearCommentDic[rndClearSE];
        }
        else
        {
            systemSEManager.PlaySystemSE(se);
        }
    }

    public string ClearComment()
    {
        return commentText;
    }
}
