using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaController : Monster
{
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] Collider2D col;
    [SerializeField] float movePower;
    [SerializeField] float maxMoveSpeed;
    [SerializeField] bool live;

    [SerializeField] Animator animator;

    [SerializeField] AudioClip sfx;

    private WaitForSeconds delay = new WaitForSeconds(0.5f);
    // 캐싱


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
        {
            SoundManager.Instance.PlaySFX(sfx);
            live = false;

            StartCoroutine(AnimPlay());
        }
        else
        {

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

}
