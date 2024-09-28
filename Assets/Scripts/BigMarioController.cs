using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BigMarioController : MonoBehaviour
{
    public enum State { Idle, Walk, Jump, Size }


    [Header("State")]
    [SerializeField] private State curState = State.Idle;

    private BaseMarioState[] states = new BaseMarioState[(int)State.Size];
    [SerializeField] IdleState idleState;
    [SerializeField] WalkState walkState;
    [SerializeField] JumpState jumpState;


    [Header("Physics")]
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] SpriteRenderer render;
    [SerializeField] CapsuleCollider2D body;

    [Header("Animation")]
    [SerializeField] Animator animator;


    [Header("Property")]
    [SerializeField] private float movePower;
    [SerializeField] private float maxMoveSpeed;
    [SerializeField] private float maxFallSpeed;
    [SerializeField] private bool isGrounded;

    [Header("Audio")]
    [SerializeField] AudioClip gameOver;
    [SerializeField] AudioClip levelUp;

    [Header("Prefab")]
    [SerializeField] GameObject smallMario;

    private float x;                                            // X 축 입력을 위한 변수
    private WaitForSeconds delay = new WaitForSeconds(1f);      // 코루틴 캐싱
    private Coroutine gameOverCoroutine;                        // 코루틴 변수
    public UnityEvent onGameOver;                               // 게임오버를 알리는 이벤트
    public MarioType curMarioType;                              // 마리오의 현재 타입
    private bool powerDown = false;

    private UIcontroller UIcontroller;

    private void Awake()
    {
        states[(int)State.Idle] = idleState;
        states[(int)State.Walk] = walkState;
        states[(int)State.Jump] = jumpState;

        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();

        gameOverCoroutine = null;
        curMarioType = MarioType.Big;
        UIcontroller = GameObject.Find("GameOverText").GetComponent<UIcontroller>();
    }

    private void Start()
    {
        if (GameManager.Instance.marioCam == null)
        {
            GameManager.Instance.marioCam = GameObject.Find("MarioCam").GetComponent<CinemachineVirtualCamera>();
        }
        
        if(GameManager.Instance.marioCam != null)
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

    public void ChangeState(State nextState)
    {
        states[(int)curState].Exit();
        curState = nextState;
        states[(int)curState].Enter();
    }


    private void GroundCheck()
    {
        Vector3 rayStartPosition = transform.position + new Vector3(0, 0.2f, 0);

        Debug.DrawRay(transform.position, Vector3.down, Color.yellow, 0.2f);
        RaycastHit2D hit = Physics2D.Raycast(rayStartPosition, Vector2.down, 0.2f, LayerMask.GetMask("Ground"));

        if (hit.collider != null)
        {
            isGrounded = true;
            //Debug.Log("바닥에 있음");
        }
        else
        {
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (powerDown) return;                                // 충돌 무시

        Debug.Log($"{collision.gameObject.name} 충돌 확인");
        if (collision.collider.CompareTag("Goomba") && transform.position.y < collision.transform.position.y + 0.3f)
        // 굼바에게 부딪혔을때 상황
        {
            powerDown = true;
            Debug.Log("굼바 충돌 진입");
            Instantiate(smallMario, transform.position, Quaternion.identity);
            Destroy(gameObject);

        }

        //else if (collision.collider.CompareTag("Mushroom"))
        //{
        //    powerUp = true;
        //    Instantiate(bigMario, transform.position, Quaternion.identity);
        //    Debug.Log("BigMario 생성");
        //    Destroy(gameObject);
        //}
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

        onGameOver?.Invoke();                                           // 이벤트 사용

        rigid.isKinematic = false;                                      // 중력 false
        rigid.AddForce(Vector2.up * 20, ForceMode2D.Impulse);           // 살짝 올라갔다가 떨어지는 연출을 위한 AddForce

        yield return delay;                                             // 1초 딜레이
        yield return delay;                                             // 1초 딜레이

        Destroy(gameObject);
    }

    #region
    [System.Serializable]
    public class BaseMarioState
    {
        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void Exit() { }
    }

    [System.Serializable]
    private class IdleState : BaseMarioState
    {
        [SerializeField] BigMarioController mario;
        public override void Enter()
        {
            mario.animator.Play("BigMarioIdle");
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
    private class WalkState : BaseMarioState
    {
        [SerializeField] BigMarioController mario;
        public override void Enter()
        {
            mario.animator.Play("BigMarioWalk");
            Debug.Log("Walk 돌입");
        }
        public override void FixedUpdate()
        {
            mario.rigid.AddForce(Vector2.right * mario.x * mario.movePower, ForceMode2D.Impulse);

            if (mario.rigid.velocity.x > mario.maxMoveSpeed)
            {
                mario.rigid.velocity = new Vector2(mario.maxMoveSpeed, mario.rigid.velocity.y);
            }
            else if (mario.rigid.velocity.x < -mario.maxMoveSpeed)
            {
                mario.rigid.velocity = new Vector2(-mario.maxMoveSpeed, mario.rigid.velocity.y);
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
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && mario.isGrounded)
            {
                mario.ChangeState(State.Jump);
            }
            else if (mario.x == 0)
            {
                mario.ChangeState(State.Idle);
            }
        }
    }

    [System.Serializable]
    private class JumpState : BaseMarioState
    {
        [SerializeField] BigMarioController mario;
        [SerializeField] AudioClip jump;
        [SerializeField] float jumpPower;

        public override void Enter()
        {
            mario.rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            Debug.Log("점프 진입");

            mario.animator.Play("BigMarioJump");
            SoundManager.Instance.PlaySFX(jump);

        }

        public override void Update()
        {

            if (mario.isGrounded && mario.rigid.velocity.y <= 0)
            {
                mario.ChangeState(State.Idle);
            }
        }

        public override void FixedUpdate()
        {

            mario.rigid.AddForce(Vector2.right * mario.x * mario.movePower, ForceMode2D.Impulse);

            if (mario.rigid.velocity.x > mario.maxMoveSpeed)
            {
                mario.rigid.velocity = new Vector2(mario.maxMoveSpeed, mario.rigid.velocity.y);
            }
            else if (mario.rigid.velocity.x < -mario.maxMoveSpeed)
            {
                mario.rigid.velocity = new Vector2(-mario.maxMoveSpeed, mario.rigid.velocity.y);
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
    #endregion

}
