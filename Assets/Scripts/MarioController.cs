using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class MarioController : MonoBehaviour
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

    private float x;
    private WaitForSeconds delay = new WaitForSeconds(1f);
    private Coroutine gameOverCoroutine;
    public UnityEvent onGameOver;


    private void Awake()
    {
        states[(int)State.Idle] = idleState;
        states[(int)State.Walk] = walkState;
        states[(int)State.Jump] = jumpState;

        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();

        gameOverCoroutine = null;
    }

    private void Start()
    {
        states[(int)curState].Enter(this);
    }

    private void Update()
    {
        if (!GameManager.Instance.gameEnded)
        {
            GroundCheck();
            x = Input.GetAxis("Horizontal");

            states[(int)curState].Update(this);
        }
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.gameEnded)
        {
            states[(int)curState].FixedUpdate(this);
        }
    }

    public void ChangeState(State nextState)
    {
        states[(int)curState].Exit(this);
        curState = nextState;
        states[(int)curState].Enter(this);
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
        Debug.Log($"{collision.gameObject.name} 충돌 확인");
        if(collision.collider.CompareTag("Goomba") && transform.position.y < collision.transform.position.y + 0.3f)
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
        yield return delay;

        onGameOver?.Invoke();                                           // 이벤트 사용
        
        rigid.isKinematic = false;                                      // 중력 false
        rigid.AddForce(Vector2.up * 12, ForceMode2D.Impulse);           // 살짝 올라가는 연출

        yield return delay;
        yield return delay;

        Destroy(gameObject);
    }

    #region
    [System.Serializable]
    public class BaseMarioState
    {
        public virtual void Enter(MarioController mario) { }
        public virtual void Update(MarioController mario) { }
        public virtual void FixedUpdate(MarioController mario) { }
        public virtual void Exit(MarioController mario) { }
    }

    [System.Serializable]
    private class IdleState : BaseMarioState
    {
        public override void Enter(MarioController mario)
        {
            mario.animator.Play("Idle");
            Debug.Log("Idle 돌입");
        }

        public override void Update(MarioController mario)
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

        public override void Enter(MarioController mario)
        {
            mario.animator.Play("Walk");
            Debug.Log("Walk 돌입");
        }
        public override void FixedUpdate(MarioController mario)
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

        public override void Update(MarioController mario)
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
        [SerializeField] AudioClip jump;
        [SerializeField] float jumpPower;

        public override void Enter(MarioController mario)
        {
            mario.rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            Debug.Log("점프 진입");

            mario.animator.Play("Jump");
            SoundManager.Instance.PlaySFX(jump);



        }

        public override void Update(MarioController mario)
        {

            if (mario.isGrounded && mario.rigid.velocity.y <= 0)
            {
                mario.ChangeState(State.Idle);
            }
        }

        public override void FixedUpdate(MarioController mario)
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

