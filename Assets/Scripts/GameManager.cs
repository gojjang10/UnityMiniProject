using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool gameEnded = false;
    public bool gameCleared = false;
    public bool playerDamaged = false;
    public CinemachineVirtualCamera marioCam;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (gameEnded && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
        else if(gameCleared && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    public void GameOver()
    {
        if(!gameEnded)
        {
            gameEnded = true;
            //Time.timeScale = 0; // 게임 일시정지

            Debug.Log("게임 오버. 재시작하려면 'R'키를 입력하시오.");
        }
    }

    public void GameClear()
    {
        if(!gameCleared)
        {
            gameCleared = true;
            Debug.Log("게임 클리어! 재시작하려면 'R'키를 입력하시오.");
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
        gameCleared = false ;
        gameEnded = false ;
    }

}
