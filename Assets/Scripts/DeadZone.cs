using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    [SerializeField] private GameObject gameOverText;
    [SerializeField] AudioClip gameOver;

    private void Start()
    {
        gameOverText.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            SoundManager.Instance.LoopBGM(false);
            SoundManager.Instance.PlayBGM(gameOver);
            
            gameOverText.SetActive(true);
            GameManager.Instance.GameOver();
                
        }
    }
}
