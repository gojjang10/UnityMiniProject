using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map1_1 : MonoBehaviour
{
    [SerializeField] AudioClip bgm;

    private void Start()
    {
        if(SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM(bgm);
        }
        else
        {
            Debug.Log("SoundManager 인스턴스가 null입니다.");
        }
    }
}
