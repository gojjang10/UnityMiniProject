using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerUI;
    [SerializeField] TextMeshProUGUI gameOverText;
    [SerializeField] GameScene gameScene;

    public float timerValue;
    public float resultValue;

    private void Start()
    {
        timerValue = 300f;
    }

    private void Update()
    {
        if(timerValue >= 0)
        {
            timerValue -= 1 * Time.deltaTime;

            //Debug.Log($"타이머 값: {timerValue}");

            resultValue = (int)timerValue;

            timerUI.text = resultValue.ToString();
            //timerUI.text = $"{Mathf.CeilToInt(timerValue)}";
        }
        else
        {
            gameOverText.enabled = true;
            GameManager.Instance.GameOver();
        }
    }
}
