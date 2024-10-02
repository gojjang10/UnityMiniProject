using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearZone : MonoBehaviour
{
    [SerializeField] private GameObject gameClearText;
    [SerializeField] AudioClip bgm;

    private void Start()
    {
        gameClearText.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SoundManager.Instance.LoopBGM(false);
            SoundManager.Instance.PlayBGM(bgm);
            Time.timeScale = 0;
            gameClearText.SetActive(true);
            GameManager.Instance.GameClear();

        }
    }
}
