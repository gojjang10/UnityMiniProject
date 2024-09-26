using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    [SerializeField] GameObject titleScreen;
    [SerializeField] GameObject anykey;
    [SerializeField] AudioClip bgm;

    private bool gameStart = false;

    private void Start()
    {
        Time.timeScale = 0f;

        SoundManager.Instance.LoopBGM(true);
        SoundManager.Instance.StopCurBGM();
        titleScreen.gameObject.SetActive(true);
        anykey.SetActive(true);
    }

    private void Update()
    {
        if(!gameStart && Input.anyKeyDown)
        {
            StartGame();
        }

    }

    private void StartGame()
    {
        gameStart = true;
        titleScreen.gameObject.SetActive(false);
        anykey.SetActive(false);
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
}
