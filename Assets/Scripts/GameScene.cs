using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    [SerializeField] GameObject titleScreen;
    [SerializeField] GameObject anykey;
    [SerializeField] GameObject gameUI;
    [SerializeField] GameObject video;
    [SerializeField] AudioClip bgm;

    private bool gameStart = false;
    private bool videoStart = false;
    WaitForSecondsRealtime delay = new WaitForSecondsRealtime(26.3f);



    private void Start()
    {
        Time.timeScale = 0f;

        SoundManager.Instance.LoopBGM(true);
        SoundManager.Instance.StopCurBGM();
        video.SetActive(true);
        titleScreen.gameObject.SetActive(true);
        anykey.SetActive(true);
        gameUI.SetActive(false);

        StartCoroutine(AutoStart());
    }

    private void Update()
    {
        if (Input.anyKeyDown && !videoStart)
        {
            video.SetActive(false);
            videoStart = true;
            StopAllCoroutines();
        }
        else if (!gameStart && Input.anyKeyDown && videoStart)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        gameStart = true;
        titleScreen.gameObject.SetActive(false);
        anykey.SetActive(false);
        gameUI.SetActive(true);
        GameManager.Instance.coin = 0;
        GameManager.Instance.score = 0;
        Time.timeScale = 1f;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM(bgm);
        }
        else
        {
            Debug.Log("SoundManager 인스턴스가 null입니다.");
        }
    }

    private IEnumerator AutoStart()
    {
        yield return delay;

        if(!videoStart)
        {
            video.SetActive(false);
            videoStart = true;
        }
    }
}
