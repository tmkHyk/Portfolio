using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class HP : MonoBehaviour
{
    [SerializeField]
    PlayerPresenter player;

    [SerializeField]
    Transform[] hearts = new Transform[3];

    private void Awake()
    {
        player = GameObject.Find("Presenter").GetComponent<PlayerPresenter>();
        hearts = transform.GetComponentsInChildren<Transform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        player.GetHP().Subscribe(hp =>
        {
            if (player.GetHP().Value < 3 && player.GetHP().Value >= 0)
                hearts[player.GetHP().Value + 1].gameObject.SetActive(false);
        });
    }
}
