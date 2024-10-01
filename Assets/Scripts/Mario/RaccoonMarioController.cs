using Cinemachine;
using System.Collections;
using UnityEngine;

public class RaccoonMarioController : MarioBassController
{


    [SerializeField] protected GameObject smallMario;
    [SerializeField] protected AudioClip levelUp;

    [SerializeField] private RaccoonRunState raccoonRunState;
    [SerializeField] private RaccoonJumpState raccoonJumpState;
    [SerializeField] private RaccoonAttackState raccoonAttackState;

    [SerializeField] Transform muzzlePoint;

    private bool canFly;
    private bool flyCoroutine;

    private void Awake()
    {
        states[(int)State.Idle] = idleState;
        states[(int)State.Walk] = walkState;
        states[(int)State.Jump] = jumpState;
        states[(int)State.Run] = raccoonRunState;
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
            //x = Input.GetAxis("Horizontal");
            if (Input.GetKey(KeyCode.Z))
            {
                runON = true;
                maxSpeed = 12;
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

    public void StartFlyingCoroutine()
    {
        StartCoroutine(FlyingCoroutine());
    }

    public IEnumerator FlyingCoroutine()
    {
        flyCoroutine = true;
        yield return new WaitForSeconds(3f);
        canFly = false;
        flyCoroutine = false;
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
    private class RaccoonJumpState : BaseMarioState
    {
        [SerializeField] MarioBassController mario;
        [SerializeField] RaccoonMarioController raccoonMario;
        [SerializeField] AudioClip jump;
        [SerializeField] float jumpPower;
        [SerializeField] float maxJumpHeight;
        private float startJumpHeight;
        private bool falling;



        public override void Enter()
        {
            //maxJumpHeight = curJumpHeight;
            //mario.rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            startJumpHeight = mario.transform.position.y;
            falling = false;
            raccoonMario.canFly = true;

            mario.rigid.velocity = new Vector2(mario.rigid.velocity.x, jumpPower);
            Debug.Log("점프 진입");

            mario.animator.Play("RaccoonJump");
            SoundManager.Instance.PlaySFX(jump);

        }

        public override void Update()
        {
            float currentHeight = mario.transform.position.y;

            if (raccoonMario.canFly)                // 너구리마리오가 날고 있을때

            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    mario.rigid.velocity = new Vector2(mario.rigid.velocity.x, jumpPower);              // 스페이스바 누를때마다 힘 가해주기
                }
            }

            if (falling)                            // 너구리마리오가 떨어지고 있을때
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    mario.animator.Play("RaccoonJumpFall");
                    mario.rigid.velocity = new Vector2(mario.rigid.velocity.x, 0);              // 스페이스바 연타로 떨어지는 속도 상쇄
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




            //if (mario.runON && mario.currentSpeed == mario.maxSpeed)
            //{
            //    mario.ChangeState(State.Run);
            //}

        }

        public override void FixedUpdate()
        {
            float currentHeight = mario.transform.position.y;

            if (raccoonMario.canFly)
            {
                if (Input.GetKey(KeyCode.Space) && currentHeight - startJumpHeight < maxJumpHeight && !falling)
                // 스페이스바를 꾹 누르기 && 현재 높이 - 시작 높이 < 최대높이 && 떨어지고 있지 않을때
                {
                    raccoonMario.StartFlyingCoroutine();
                }
            }
            else
            {
                falling = true;
            }

 

            //if (Input.GetKey(KeyCode.Space) && currentHeight - startJumpHeight < maxJumpHeight && !falling)
            //{
            //    mario.rigid.velocity = new Vector2(mario.rigid.velocity.x, jumpPower);
            //    if (Input.GetKeyUp(KeyCode.Space))
            //    {
            //        falling = true;
            //    }

            //}
            //if (Input.GetKey(KeyCode.Space) && currentHeight - startJumpHeight >= maxJumpHeight)
            //{
            //    falling = true;
            //}



            //mario.rigid.AddForce(Vector2.right * mario.x * mario.movePower, ForceMode2D.Impulse);

            //if (mario.rigid.velocity.x > mario.maxMoveSpeed)
            //{
            //    mario.rigid.velocity = new Vector2(mario.maxMoveSpeed, mario.rigid.velocity.y);
            //}
            //else if (mario.rigid.velocity.x < -mario.maxMoveSpeed)
            //{
            //    mario.rigid.velocity = new Vector2(-mario.maxMoveSpeed, mario.rigid.velocity.y);
            //}

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
    private class RaccoonAttackState : BaseMarioState
    {
        [SerializeField] RaccoonAttackState mario;
    }
}
