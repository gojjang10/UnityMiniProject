using UnityEngine;
using UnityEngine.Events;

public class MarioBassController : MonoBehaviour
{
    public MarioType curMarioType;

    [Header("State")]
    [SerializeField] protected State curState = State.Idle;

    protected BaseMarioState[] states = new BaseMarioState[(int)State.Size];
    [SerializeField] protected IdleState idleState;
    [SerializeField] protected WalkState walkState;
    [SerializeField] protected JumpState jumpState;
    [SerializeField] protected RunState runState;



    [field: SerializeField] public Rigidbody2D rigid { get; protected set; }
    [field: SerializeField] public SpriteRenderer render { get; protected set; }

    [Header("MoveProperty")]
    [SerializeField] private float acceleration;    // 가속도
    [field: SerializeField] public float deceleration { get; protected set; }    // 감속도
    [field: SerializeField] public float currentSpeed { get; protected set; }    // 현재속도
    [field: SerializeField] public float maxSpeed { get; protected set; }       // 최대 속도
    [field: SerializeField] public float maxFallSpeed { get; protected set; }


    //[Header("Animation")]
    [field: SerializeField] public Animator animator { get; protected set; }

    //[Header("Property")]

    [field: SerializeField] public bool isGrounded { get; protected set; }
    [field: SerializeField] public bool runON { get; protected set; }
    [field: SerializeField] protected float targetSpeed;

    [Header("Audio")]
    [SerializeField] protected AudioClip gameOver;
    [SerializeField] protected AudioClip coin;

    [field: SerializeField] public float x { get; protected set; }                                           // X 축 입력을 위한 변수
    protected WaitForSeconds delay = new WaitForSeconds(1f);      // 코루틴 캐싱
    protected Coroutine gameOverCoroutine;                        // 코루틴 변수
    protected Coroutine playerDamagedCoroutine;                   // 코루틴 변수
    public UnityEvent onGameOver;                               // 게임오버를 알리는 이벤트
                                                                // 마리오의 현재 타입

    [SerializeField] protected UIcontroller UIcontroller;
    [SerializeField] protected SpeedUI speedUI;
    protected bool powerUp = false;
    protected bool powerDown = false;
    public int jumpCount = 1;
    public bool falling = false;

    public LayerMask ground;
    public LayerMask box;

    private void Update()
    {
        if (!GameManager.Instance.gameEnded && !GameManager.Instance.gameCleared)
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
            speedUI.UpdateSpeedUI(currentSpeed, maxSpeed);
            Debug.Log("스피드UI 활성화");

            states[(int)curState].Update();

            
        }
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.gameEnded && !GameManager.Instance.gameCleared)
        {
            VelocityMove();
            states[(int)curState].FixedUpdate();
        }
    }


    public void VelocityMove()
    // velocity를 사용한 속도, 가속도를 이용한 로직
    {
        x = Input.GetAxisRaw("Horizontal");

        if (x != 0)
        {
            currentSpeed += x * acceleration * Time.fixedDeltaTime;
            // 물리엔진의 정확도를 위해 fixedDeltaTime 사용.
            currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);
            // Mathf.Clamp : 최소 / 최대값을 설정하여 범위 이외의 값을 넘지 못도록 만들기

            render.flipX = x < 0;
            // 캐릭터 반전
        }

        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.fixedDeltaTime);
            // Mathf.MoveTowards : Mathf.Lerp와 비슷한 기능으로 current에서 target으로 서서히 값을 변경
        }

        rigid.velocity = new Vector2(currentSpeed, rigid.velocity.y);
        // 현재 속도를 설정한 currentSpeed로 설정


    }


    public void ChangeState(State nextState)
    {
        states[(int)curState].Exit();
        curState = nextState;
        states[(int)curState].Enter();
    }
    public void GroundCheck()
    {
        Vector3 rayStartPosition = transform.position + new Vector3(0, 0.2f, 0);

        Debug.DrawRay(transform.position, Vector3.down, Color.yellow, 0.2f);
        //RaycastHit2D hit = Physics2D.Raycast(rayStartPosition, Vector2.down, 0.2f, LayerMask.GetMask("Ground"));
        RaycastHit2D[] hits = Physics2D.RaycastAll(rayStartPosition, Vector2.down, 0.2f);

        if (hits != null && hits.Length >= 1)
        {
            foreach (RaycastHit2D hit in hits)
            {
                //Debug.Log($"콜라이더 감지 {hit.collider.name}");

                if (hit.collider.gameObject.CompareTag("Ground"))    // 레이어 6번 : Ground
                {
                    Debug.Log("콜라이더 감지 6번");
                    isGrounded = true;

                }
                else if (hit.collider.gameObject.CompareTag("Box"))     // 레이어 8번 : Box
                {
                    Debug.Log("콜라이더 감지 8번");
                    isGrounded = true;

                }
                else
                {
                    //Debug.Log("땅에 없음");
                    isGrounded = false;
                }
            }
        }

        // hit != null || hit.length >= 1
        // 반복문 진행
        // 반복문을 순회하면서 원하는 레이어 혹은 태그인지 확인


    }

    [System.Serializable]
    public class BaseMarioState
    {
        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void Exit() { }
    }

    [System.Serializable]
    protected class IdleState : BaseMarioState
    {
        [SerializeField] MarioBassController mario;
        public override void Enter()
        {
            mario.animator.Play("Idle");
            Debug.Log("Idle 돌입");
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && mario.isGrounded)
            {
                mario.ChangeState(State.Jump);
            }
            else if (mario.x != 0)
            {
                mario.ChangeState(State.Walk);
            }
        }
    }

    [System.Serializable]
    protected class WalkState : BaseMarioState
    {
        [SerializeField] MarioBassController mario;
        public override void Enter()
        {
            mario.animator.Play("Walk");
            Debug.Log("Walk 돌입");
            mario.maxSpeed = 5;
        }
        public override void FixedUpdate()
        {
            // AddForce를 사용한 움직임 구현
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
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && mario.isGrounded)
            {
                mario.ChangeState(State.Jump);
            }
            else if (mario.currentSpeed == 0)
            {
                mario.ChangeState(State.Idle);
            }

            if (mario.runON && Mathf.Abs(mario.currentSpeed) == Mathf.Abs(mario.maxSpeed))
            {
                mario.ChangeState(State.Run);
            }
        }
    }

    [System.Serializable]
    protected class JumpState : BaseMarioState
    {
        [SerializeField] MarioBassController mario;
        [SerializeField] AudioClip jump;
        [SerializeField] float jumpPower;
        [SerializeField] float maxJumpHeight;
        private float startJumpHeight;



        public override void Enter()
        {

            //maxJumpHeight = curJumpHeight;
            //mario.rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            startJumpHeight = mario.transform.position.y;
            mario.falling = false;

            if (mario.jumpCount == 1)
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

            if (mario.isGrounded && mario.rigid.velocity.y <= 0.01f)            // 땅에 있을때 && 확실하게 velocity로 값 확인
            {
                mario.jumpCount = 1;
                Debug.Log($"업데이트 점프카운트{mario.jumpCount}");
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
    protected class RunState : BaseMarioState
    {
        [SerializeField] MarioBassController mario;

        public override void Enter()
        {
            mario.animator.Play("Run");
        }

        public override void Update()
        {
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
                mario.ChangeState(State.Jump);
            }
        }
    }
}