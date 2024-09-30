using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class RaccoonMarioController : MarioBassController
{
    [SerializeField] protected GameObject smallMario;
    [SerializeField] protected AudioClip levelUp;

    [SerializeField] private RaccoonJumpState raccoonJumpState;
    [SerializeField] private RaccoonAttackState raccoonAttackState;

    [SerializeField] Transform muzzlePoint;

    private void Awake()
    {
        states[(int)State.Idle] = idleState;
        states[(int)State.Walk] = walkState;
        states[(int)State.Jump] = jumpState;
        states[(int)State.Run] = runState;
        states[(int)State.RaccoonJump] = raccoonJumpState;
        states[(int)State.RaccoonAttack] = raccoonAttackState; 

        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();

        gameOverCoroutine = null;
        curMarioType = MarioType.Raccoon;
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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("GameOver"))
        {
            Destroy(gameObject);
        }
    }

    [System.Serializable]
    private class RaccoonJumpState : BaseMarioState
    {

    }

    [System.Serializable]
    private class RaccoonAttackState : BaseMarioState
    {

    }
}
