using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMarioController : MarioBassController
{
    [SerializeField] protected GameObject smallMario;
    [SerializeField] protected GameObject fireMario;
    [SerializeField] protected AudioClip levelUp;
    
    
    

    private void Awake()
    {
        states[(int)State.Idle] = idleState;
        states[(int)State.Walk] = walkState;
        states[(int)State.Jump] = jumpState;

        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();

        gameOverCoroutine = null;
        curMarioType = MarioType.Fire;
        UIcontroller = GameObject.Find("GameOverText").GetComponent<UIcontroller>();
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

    private void Update()
    {
        if (!GameManager.Instance.gameEnded)
        {
            GroundCheck();
            x = Input.GetAxis("Horizontal");

            states[(int)curState].Update();
        }
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.gameEnded)
        {
            states[(int)curState].FixedUpdate();
        }
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

 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("GameOver"))
        {
            Destroy(gameObject);
        }
    }
}
