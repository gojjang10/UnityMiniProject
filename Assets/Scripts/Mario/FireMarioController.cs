using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class FireMarioController : MarioBassController
{
    [SerializeField] protected GameObject smallMario;
    [SerializeField] protected GameObject raccoonMario;
    [SerializeField] protected AudioClip levelUp;

    [SerializeField] private FireState fireState;

    [SerializeField] Transform muzzlePoint;
    

    private void Awake()
    {
        states[(int)State.Idle] = idleState;
        states[(int)State.Walk] = walkState;
        states[(int)State.Jump] = jumpState;
        states[(int)State.Run] = runState;
        states[(int)State.Fire] = fireState;

        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();

        gameOverCoroutine = null;
        curMarioType = MarioType.Fire;
        UIcontroller = GameObject.Find("GameOverText").GetComponent<UIcontroller>();
        speedUI = GameObject.Find("GameUI").GetComponent<SpeedUI>();
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

    private void Update()
    {
        if (!GameManager.Instance.gameEnded)
        {
            GroundCheck();

            if (Input.GetKey(KeyCode.Z))
            {
                runON = true;
                maxSpeed = 10;
            }
            else if (Input.GetKeyUp(KeyCode.Z))
            {
                runON = false;
                maxSpeed = 5;
            }

            if (render.flipX)
            {
                muzzlePoint.localPosition = new Vector3(-Mathf.Abs(muzzlePoint.localPosition.x), muzzlePoint.localPosition.y, muzzlePoint.localPosition.z);
            }
            else
            {
                muzzlePoint.localPosition = new Vector3(Mathf.Abs(muzzlePoint.localPosition.x), muzzlePoint.localPosition.y, muzzlePoint.localPosition.z);
            }
            speedUI.UpdateSpeedUI(currentSpeed, maxSpeed);
            states[(int)curState].Update();
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
        else if (collision.collider.CompareTag("Leaf"))
        {
            powerUp = true;
            Instantiate(raccoonMario, transform.position, Quaternion.identity);
            Debug.Log("RaccoonMario 생성");
            Destroy(gameObject);
        }
        if (collision.collider.CompareTag("Box"))
        {
            falling = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool hit = false;   

        if (collision.CompareTag("GameOver"))
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

    [System.Serializable]
    private class FireState : BaseMarioState
    {
        [SerializeField] FireMarioController mario;

        public override void Enter()
        {
            mario.animator.Play("Fire");
            Debug.Log("Fire 발사");
        }

    }
}
