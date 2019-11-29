using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseCanvas;
    Text menuBtnText;
    Image menuImage;

    public GameObject optionPanel;

    BoolReactiveProperty isPausing = new BoolReactiveProperty(false);
    public BoolReactiveProperty IsPausing
    {
        get { return isPausing; }
    }

    private void Awake()
    {
        //pauseCanvas = GameObject.Find("PauseCanvas");
        pauseCanvas.SetActive(false);

        menuImage = GetComponent<Image>();
        menuBtnText = GetComponentInChildren<Text>();
    }

    public void Pausing()
    {
        if (isPausing.Value == false)
        {
            isPausing.Value = true;
            pauseCanvas.SetActive(true);
            menuImage.enabled = false;
            menuBtnText.enabled = false;
            PauseTime();
        }
        else Continue();
    }

    public void Retry()
    {
        ResumeTime();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Continue()
    {
        ResumeTime();
        isPausing.Value = false;
        pauseCanvas.SetActive(false);
        menuImage.enabled = true;
        menuBtnText.enabled = true;
    }

    public void Title()
    {
        ResumeTime();
        SceneManager.LoadScene("Title");
    }

    public void OpenOption()
    {
        optionPanel.SetActive(true);
    }

    public void CloseOption()
    {
        optionPanel.SetActive(false);
    }

    public void PauseTime()
    {
        Time.timeScale = 0;
    }

    public void ResumeTime()
    {
        Time.timeScale = 1;
    }
}
