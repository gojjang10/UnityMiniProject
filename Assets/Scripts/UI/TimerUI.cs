using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerUI;
    [SerializeField] TextMeshProUGUI gameOverText;
    [SerializeField] GameScene gameScene;
    [SerializeField] AudioClip gameOver;

    public float timerValue;
    public float resultValue;
    private bool done;

    private void Start()
    {
        timerValue = 300f;
    }

    private void Update()
    {
        if(timerValue >= 0)
        {
            if(!GameManager.Instance.gameEnded && !GameManager.Instance.gameCleared)
            {
                timerValue -= 1 * Time.deltaTime;

                resultValue = (int)timerValue;

                timerUI.text = resultValue.ToString();

            }

        }
        else
        {
            if(!done)
            {
                done = true;
                gameOverText.enabled = true;
                SoundManager.Instance.LoopBGM(false);
                SoundManager.Instance.PlayBGM(gameOver);
                Time.timeScale = 0;
                GameManager.Instance.GameOver();
            }

        }
    }
}
