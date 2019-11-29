using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeManager : MonoBehaviour
{
    Loading loadObj;

    // Start is called before the first frame update
    void Start()
    {
        loadObj = FindObjectOfType<Loading>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        loadObj.nextSceneNum.OnNext((int)Loading.SceneName.Game);
        
    }
}
