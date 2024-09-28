using System.Collections;
using System.Collections.Generic;
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


    [Header("Physics")]
    [SerializeField] protected Rigidbody2D rigid;
    [SerializeField] protected SpriteRenderer render;
    [SerializeField] protected CapsuleCollider2D body;

    [Header("Animation")]
    [SerializeField] protected Animator animator;


    [Header("Property")]
    [SerializeField] protected  float movePower;
    [SerializeField] protected  float maxMoveSpeed;
    [SerializeField] protected  float maxFallSpeed;
    [SerializeField] protected bool isGrounded;

    [Header("Audio")]
    [SerializeField] protected AudioClip gameOver;

    protected  float x;                                            // X 축 입력을 위한 변수
    protected  WaitForSeconds delay = new WaitForSeconds(1f);      // 코루틴 캐싱
    protected  Coroutine gameOverCoroutine;                        // 코루틴 변수
    protected Coroutine playerDamagedCoroutine;                   // 코루틴 변수
    public UnityEvent onGameOver;                               // 게임오버를 알리는 이벤트
                                                                // 마리오의 현재 타입

    [SerializeField] protected UIcontroller UIcontroller;
    protected bool powerUp = false;
    protected bool powerDown = false;


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
    protected class JumpState : BaseMarioState
    {
        [SerializeField] MarioBassController mario;
        [SerializeField] AudioClip jump;
        [SerializeField] float jumpPower;

        public override void Enter()
        {
            mario.rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            Debug.Log("점프 진입");

            mario.animator.Play("Jump");
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
}

