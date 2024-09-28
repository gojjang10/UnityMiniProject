using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    [SerializeField] AudioClip gameOver;
    [SerializeField] TextMeshProUGUI gameOverText;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            SoundManager.Instance.LoopBGM(false);
            SoundManager.Instance.PlayBGM(gameOver);
            
            gameOverText.enabled = true;
            GameManager.Instance.GameOver();
                
        }
    }
}
