using Unity.VisualScripting;
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


    [Header("Physics")]
    [SerializeField] protected Rigidbody2D rigid;
    [SerializeField] protected SpriteRenderer render;
    [SerializeField] protected CapsuleCollider2D body;

    [Header("MoveProperty")]
    [SerializeField] private float acceleration;    // 가속도
    [SerializeField] private float deceleration;    // 감속도
    [SerializeField] private float currentSpeed;    // 현재속도
    [SerializeField] protected float maxSpeed;        // 최대 속도
    [SerializeField] protected float maxFallSpeed;


    [Header("Animation")]
    [SerializeField] protected Animator animator;


    [Header("Property")]

    [SerializeField] protected bool isGrounded;
    [SerializeField] protected bool runON;
    [SerializeField] protected float targetSpeed;

    [Header("Audio")]
    [SerializeField] protected AudioClip gameOver;

    protected float x;                                            // X 축 입력을 위한 변수
    protected WaitForSeconds delay = new WaitForSeconds(1f);      // 코루틴 캐싱
    protected Coroutine gameOverCoroutine;                        // 코루틴 변수
    protected Coroutine playerDamagedCoroutine;                   // 코루틴 변수
    public UnityEvent onGameOver;                               // 게임오버를 알리는 이벤트
                                                                // 마리오의 현재 타입

    [SerializeField] protected UIcontroller UIcontroller;
    protected bool powerUp = false;
    protected bool powerDown = false;

    public LayerMask ground;
    public LayerMask box;

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

            states[(int)curState].Update();
        }
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.gameEnded)
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

        //if (rigid.velocity.y > maxFallSpeed)
        //{
        //    rigid.velocity = new Vector2(rigid.velocity.x, maxFallSpeed);
        //}


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

        if (hits != null && hits.Length > 0)
        {
            foreach (RaycastHit2D hit in hits)
            {
                //Debug.Log($"콜라이더 감지 {hit.collider.name}");

                if (hit.collider.gameObject.CompareTag("Ground"))    // 레이어 6번 : Ground
                {
                    //Debug.Log("콜라이더 감지 6번");
                    isGrounded = true;

                }
                else if (hit.collider.gameObject.CompareTag("Box"))     // 레이어 8번 : Box
                {
                    //Debug.Log("콜라이더 감지 8번");
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
        private bool falling;


        public override void Enter()
        {
            //maxJumpHeight = curJumpHeight;
            //mario.rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            startJumpHeight = mario.transform.position.y;
            falling = false;

            mario.rigid.velocity = new Vector2(mario.rigid.velocity.x, jumpPower);
            Debug.Log("점프 진입");

            mario.animator.Play("Jump");
            SoundManager.Instance.PlaySFX(jump);

        }

        public override void Update()
        {


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
            // 점프 높낮이 구현
            float currentHeight = mario.transform.position.y;

            if(Input.GetKey(KeyCode.Space) && currentHeight - startJumpHeight < maxJumpHeight && !falling)
            {
                mario.rigid.velocity = new Vector2(mario.rigid.velocity.x, jumpPower);
 
            }
            if (Input.GetKey(KeyCode.Space) && currentHeight - startJumpHeight >= maxJumpHeight)
            {
                falling = true;
            }



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
    protected class RunState : BaseMarioState
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
                mario.ChangeState(State.Jump);
            }
        }
    }
}