using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Events;



public class BigMarioController : MarioBassController
{
    [SerializeField] protected GameObject fireMario;
    [SerializeField] protected GameObject smallMario;
    [SerializeField] protected GameObject raccoonMario;
    [SerializeField] protected AudioClip levelUp;
 

    private void Awake()
    {
        states[(int)State.Idle] = idleState;
        states[(int)State.Walk] = walkState;
        states[(int)State.Jump] = jumpState;
        states[(int)State.Run] = runState;

        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();

        gameOverCoroutine = null;
        curMarioType = MarioType.Big;
        UIcontroller = GameObject.Find("GameOverText").GetComponent<UIcontroller>();
        powerUp = false;
        powerDown = false;
    }

    private void Start()
    {
        if (GameManager.Instance.marioCam == null)
        {
            GameManager.Instance.marioCam = GameObject.Find("MarioCam").GetComponent<CinemachineVirtualCamera>();
        }

        if (GameManager.Instance.marioCam != null)
        {
            GameManager.Instance.marioCam.Follow = transform;
        }
        states[(int)curState].Enter();
        SoundManager.Instance.PlaySFX(levelUp);
        onGameOver.AddListener(UIcontroller.GameOverUI);

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (powerDown || powerUp) return;                                // 충돌 무시

        Debug.Log($"{collision.gameObject.name} 충돌 확인");
        if (collision.collider.CompareTag("Goomba") && transform.position.y < collision.transform.position.y + 0.3f)
        // 굼바에게 부딪혔을때 상황
        {
            powerDown = true;
            GameManager.Instance.playerDamaged = true;
            Debug.Log("굼바 충돌 진입");
            Instantiate(smallMario, transform.position, Quaternion.identity);
            Destroy(gameObject);

        }

        if (collision.collider.CompareTag("Flower"))
        {
            powerUp = true;
            Instantiate(fireMario, transform.position, Quaternion.identity);
            Debug.Log("FireMario 생성");
            Destroy(gameObject);
        }
        else if (collision.collider.CompareTag("Leaf"))
        {
            powerUp = true;
            Instantiate(raccoonMario, transform.position, Quaternion.identity);
            Debug.Log("RaccoonMario 생성");
            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool hit = false;
        if (collision.CompareTag("GameOver"))               // 게임오버 존에 닿았을때
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Coin"))        // 코인이랑 부딪혔을때
        {
            if (!hit)
            {
                SoundManager.Instance.PlaySFX(coin);
                GameManager.Instance.score += 100;
                GameManager.Instance.coin++;
                hit = true;
                Destroy(collision.gameObject);
            }
        }
    }
}

