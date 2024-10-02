using Cinemachine;
using System.Collections;
using UnityEngine;

public class RaccoonMarioController : MarioBassController
{


    [SerializeField] protected GameObject smallMario;
    [SerializeField] protected GameObject fireMario;
    [SerializeField] protected AudioClip levelUp;

    [SerializeField] private RaccoonJumpState raccoonJumpState;
    [SerializeField] private RaccoonRunState raccoonRunState;
    [SerializeField] private RaccoonDashJumpState raccoonDashJumpState;
    [SerializeField] private RaccoonAttackState raccoonAttackState;

    [SerializeField] Transform muzzlePoint;

    private Coroutine flyOn;

    [SerializeField] private bool raccoonFalling;
    [SerializeField] private bool canFly;

    private WaitForSeconds fallingDelay = new WaitForSeconds(2.5f);

    private void Awake()
    {
        states[(int)State.Idle] = idleState;
        states[(int)State.Walk] = walkState;
        states[(int)State.Jump] = raccoonJumpState;
        states[(int)State.Run] = raccoonRunState;
        states[(int)State.RaccoonJump] = raccoonDashJumpState;
        states[(int)State.RaccoonAttack] = raccoonAttackState;

        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();

        gameOverCoroutine = null;
        curMarioType = MarioType.Raccoon;
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
            //x = Input.GetAxis("Horizontal");
            if (Input.GetKey(KeyCode.Z))
            {
                runON = true;
                maxSpeed = 10;
                targetSpeed = maxSpeed;

            }
            else
            {
                runON = false;
                targetSpeed = 5;
            }

            maxSpeed = Mathf.Lerp(maxSpeed, targetSpeed, deceleration * Time.deltaTime);
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
        if (collision.collider.CompareTag("Flower"))
        {
            powerUp = true;
            Instantiate(fireMario, transform.position, Quaternion.identity);
            Debug.Log("FireMario 생성");
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
            Destroy(collision.gameObject);
            if (!hit)
            {
                hit = true;
                SoundManager.Instance.PlaySFX(coin);
                GameManager.Instance.score += 100;
                GameManager.Instance.coin++;
  
            }
        }
    }

    public IEnumerator FlyingCoroutine()
    {
        yield return fallingDelay;
        falling = true;
        canFly = false;
    }

    [System.Serializable]
    private class RaccoonJumpState : BaseMarioState
    {
        [SerializeField] MarioBassController mario;
        [SerializeField] AudioClip jump;
        [SerializeField] float jumpPower;
        [SerializeField] float maxJumpHeight;
        private float startJumpHeight;
        public int jumpCount = 1;


        public override void Enter()
        {

            //maxJumpHeight = curJumpHeight;
            //mario.rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            startJumpHeight = mario.transform.position.y;
            mario.falling = false;

            if (jumpCount == 1)
            {
                mario.rigid.velocity = new Vector2(mario.rigid.velocity.x, jumpPower);
                mario.jumpCount = 0;
                Debug.Log($"점프카운트{mario.jumpCount}");
            }


            Debug.Log("점프 진입");
            mario.animator.Play("Jump");
            SoundManager.Instance.PlaySFX(jump);


        }

        public override void Update()
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                Debug.Log("스페이스바 떼짐");
                mario.falling = true;
            }


            if (mario.falling)                            // 너구리마리오가 떨어지고 있을때
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    mario.animator.Play("RaccoonJumpFall");
                    mario.rigid.velocity = new Vector2(mario.rigid.velocity.x, 0);              // 스페이스바 연타로 떨어지는 속도 상쇄
                }
            }


            if (mario.isGrounded && mario.rigid.velocity.y <= 0.01f)            // 땅에 있을때 && 확실하게 velocity로 값 확인
            {
                jumpCount = 1;
                if (mario.runON)
                {
                    mario.ChangeState(State.Run);
                }
                else if (mario.currentSpeed != 0)
                {
                    mario.ChangeState(State.Walk);
                }
                else
                {
                    mario.ChangeState(State.Idle);
                }
            }




            //if (mario.runON && mario.currentSpeed == mario.maxSpeed)
            //{
            //    mario.ChangeState(State.Run);
            //}

        }

        public override void FixedUpdate()
        {
            // 점프 높낮이 구현
            float currentHeight = mario.transform.position.y;

            if (mario.jumpCount == 0)
            {
                Debug.Log("픽스드업데이트로 현재 점프중");

                if (Input.GetKey(KeyCode.Space) && currentHeight - startJumpHeight < maxJumpHeight && !mario.falling)
                {
                    Debug.Log("위로 힘 가해주기");
                    mario.rigid.velocity = new Vector2(mario.rigid.velocity.x, jumpPower);
                }

                if (Input.GetKey(KeyCode.Space) && currentHeight - startJumpHeight >= maxJumpHeight)
                {
                    mario.falling = true;
                }
            }

            // 캐릭터 이미지 전환
            if (mario.x < 0)
            {
                mario.render.flipX = true;
            }
            else if (mario.x > 0)
            {
                mario.render.flipX = false;
            }

            if (mario.rigid.velocity.y < -mario.maxFallSpeed)
            {
                mario.rigid.velocity = new Vector2(mario.rigid.velocity.x, -mario.maxFallSpeed);
            }
        }
    }



    [System.Serializable]
    private class RaccoonRunState : BaseMarioState
    {
        [SerializeField] MarioBassController mario;

        public override void Enter()
        {
            mario.animator.Play("Run");
        }

        public override void Update()
        {
            //if(Input.GetKeyUp(KeyCode.Z))
            //{
            //    mario.ChangeState(State.Walk);
            //}
            if (mario.currentSpeed == 0)
            {
                mario.ChangeState(State.Idle);
            }

            if (!mario.runON && mario.currentSpeed < mario.maxFallSpeed)
            {
                mario.ChangeState(State.Walk);
            }

            if (Input.GetKeyDown(KeyCode.Space) && mario.isGrounded)
            {
                mario.ChangeState(State.RaccoonJump);
            }
        }
    }


    [System.Serializable]
    private class RaccoonDashJumpState : BaseMarioState
    {
        [SerializeField] MarioBassController mario;
        [SerializeField] RaccoonMarioController raccoonMario;
        [SerializeField] AudioClip jump;
        [SerializeField] float jumpPower;
        private float startJumpHeight;




        public override void Enter()
        {
            raccoonMario.falling = false;   // 떨어지고 있는지 아닌지
            raccoonMario.canFly = true;     // 날 수 있는 상태

            raccoonMario.rigid.velocity = new Vector2(mario.rigid.velocity.x, jumpPower);
            Debug.Log("점프 진입");
            
            if (raccoonMario.flyOn == null)
            {
                Debug.Log("코루틴 시작");
                raccoonMario.flyOn = raccoonMario.StartCoroutine(raccoonMario.FlyingCoroutine());
            }

            mario.animator.Play("RaccoonJump");
            SoundManager.Instance.PlaySFX(jump);

        }

        public override void Update()
        {
            if (raccoonMario.canFly)                // 너구리마리오가 날고 있을때
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    raccoonMario.rigid.velocity = new Vector2(raccoonMario.rigid.velocity.x, jumpPower);              // 스페이스바 누를때마다 힘 가해주기
                }
                if(raccoonMario.isGrounded && raccoonMario.rigid.velocity.y <= 0.01f )
                {
                    StopFlyCoroutine();
                    Debug.Log("땅에 닿아서 코루틴 해제");
                }
            }

            if (raccoonMario.falling)                            // 너구리마리오가 떨어지고 있을때
            {
                if(raccoonMario.flyOn != null)
                {
                    Debug.Log("최고점에서 코루틴 해제");
                    StopFlyCoroutine();

                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    mario.animator.Play("RaccoonJumpFall");
                    raccoonMario.rigid.velocity = new Vector2(raccoonMario.rigid.velocity.x, 0);              // 스페이스바 연타로 떨어지는 속도 상쇄
                }
            }


            if (mario.isGrounded && mario.rigid.velocity.y <= 0.01f)            // 땅에 있을때 && 확실하게 velocity로 값 확인
            {
 
                    if (mario.runON)
                    {
                        mario.ChangeState(State.Run);
                    }
                    else if (mario.currentSpeed != 0)
                    {
                        mario.ChangeState(State.Walk);
                    }
                    else
                    {
                        mario.ChangeState(State.Idle);
                    }
                

            }

        }

        public override void FixedUpdate()
        {

            // 캐릭터 이미지 전환
            if (mario.x < 0)
            {
                mario.render.flipX = true;
            }
            else if (mario.x > 0)
            {
                mario.render.flipX = false;
            }

            if (mario.rigid.velocity.y < -mario.maxFallSpeed)
            {
                mario.rigid.velocity = new Vector2(mario.rigid.velocity.x, -mario.maxFallSpeed);
            }
        }

        private void StopFlyCoroutine()
        {
            if(raccoonMario.flyOn != null)
            {
                raccoonMario.StopCoroutine(raccoonMario.flyOn);
                raccoonMario.flyOn = null;
            }
        }

    }



    [System.Serializable]
    private class RaccoonAttackState : BaseMarioState
    {
        [SerializeField] RaccoonAttackState mario;
    }
}
