using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaController : Monster
{
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] Collider2D col;

    [SerializeField] Animator animator;

    [SerializeField] AudioClip hitSFX;
    [SerializeField] AudioClip steppedSfx;
    private void Start()
    {
        live = true;
    }
    private void FixedUpdate()
    {
        if (live && !GameManager.Instance.gameEnded)
        {
            rigid.AddForce(Vector2.left * movePower, ForceMode2D.Impulse);

            if (rigid.velocity.x > maxMoveSpeed)
            {
                rigid.velocity = new Vector2(maxMoveSpeed, rigid.velocity.y);
            }
            else
            {
                rigid.velocity = new Vector2(-maxMoveSpeed, rigid.velocity.y);
            }
        }
        else
        {
            return;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"{collision.gameObject.name} 충돌 확인");
        if(collision.collider.CompareTag("Player") && collision.transform.position.y > transform.position.y + 0.1f)
            // 마리오가 밟았을때
        {
            SoundManager.Instance.PlaySFX(steppedSfx);
            live = false;

            StartCoroutine(AnimPlay());
        }
        
        if(collision.collider.CompareTag("FireBall"))
            // 파이어볼에 맞았을때
        {
            SoundManager.Instance.PlaySFX(hitSFX);
            live = false;
            StartCoroutine(HitFireBall());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("MainCamera") && live)
            // 카메라의 콜라이더 범위 밖으로 사라지면 삭제
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator AnimPlay()
    {
        gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        rigid.velocity = Vector2.zero;
        rigid.gravityScale = 0;

        animator.Play("GoombaStepped");

        yield return delay;
        Destroy(gameObject);
    }

    private IEnumerator HitFireBall()
    {
        Debug.Log("굼바 처치");
        rigid.velocity = Vector2.zero;
        col.enabled = false;
        SpriteRenderer render = GetComponent<SpriteRenderer>();
        render.flipY = true;
        transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        yield return delay;
        yield return delay;
        yield return delay;
        yield return delay;
        yield return delay;
        yield return delay;

        Destroy (gameObject);
        
    }
}
