using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIcontroller : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI gameOverText;
    private void Start()
    {
        gameOverText.enabled = false;
    }

    public void GameOverUI()
    {
        gameOverText.enabled = true;
    }

}
