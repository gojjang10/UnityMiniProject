using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;



public class MarioController : MarioBassController
{
    [SerializeField] protected GameObject bigMario;
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
        curMarioType = MarioType.Small;
        powerUp = false;
        powerDown = false;

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
        onGameOver.AddListener(UIcontroller.GameOverUI);

        if(GameManager.Instance.playerDamaged)
        {
            if (playerDamagedCoroutine == null)
            // 코루틴 두 번 실행되지 못하게 예외처리
            {
                playerDamagedCoroutine = StartCoroutine(OnDamaged());
            }
        }
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (powerUp) return;                                // 충돌 무시

        Debug.Log($"{collision.gameObject.name} 충돌 확인");
        if(collision.collider.CompareTag("Goomba") && transform.position.y < collision.transform.position.y + 0.3f)
            // 굼바에게 부딪혔을때 상황
        {
            Debug.Log("굼바 충돌 진입");
            SoundManager.Instance.LoopBGM(false);
            SoundManager.Instance.PlayBGM(gameOver);
            GameManager.Instance.gameEnded = true;  // 이동불가 상태로 만들기
            
            if(gameOverCoroutine == null)
                // 코루틴 두 번 실행되지 못하게 예외처리
            {
                gameOverCoroutine = StartCoroutine(GameOverAnim());
            }
        }

        else if(collision.collider.CompareTag("Mushroom"))
        {
            powerUp = true;
            Instantiate(bigMario, transform.position, Quaternion.identity);
            Debug.Log("BigMario 생성");
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

    private IEnumerator GameOverAnim()
    {
        rigid.velocity = Vector2.zero;
  
        gameObject.GetComponent<CapsuleCollider2D>().enabled = false;   // 충돌체 비활성화
        body.enabled = false;                                           // 충돌체 비활성화
        rigid.isKinematic = true;                                       // 중력 true
        animator.Play("GameOver");

        GameManager.Instance.GameOver();
        yield return delay;                                             // 1초 딜레이

        //onGameOver?.Invoke();                                           // 이벤트 사용
        onGameOver?.Invoke();
        
        rigid.isKinematic = false;                                      // 중력 false
        rigid.AddForce(Vector2.up * 20, ForceMode2D.Impulse);           // 살짝 올라갔다가 떨어지는 연출을 위한 AddForce

        yield return delay;                                             // 1초 딜레이
        yield return delay;                                             // 1초 딜레이

        Destroy(gameObject);
    }

    private IEnumerator OnDamaged()
        // 데미지를 입었을때 무적판정을 위한 코루틴
    {
        ChangeLayer(gameObject, 9);
        render.color = new Color(1, 1, 1, 0.4f);
        yield return delay;
        yield return delay;
        ChangeLayer(gameObject, 0);
        render.color = new Color(1, 1, 1, 1); 
        GameManager.Instance.playerDamaged = false;
    }

    private void ChangeLayer(GameObject obj, int layer)
        // 자식 오브젝트의 레이어를 바꿔주기 위한 재귀함수
    {
        gameObject.layer = layer;
        obj.layer = layer;
        foreach(Transform child in obj.transform)
        {
            ChangeLayer(child.gameObject, layer);
        }
    }

  
}

