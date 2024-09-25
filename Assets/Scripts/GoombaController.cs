using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaController : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigid;
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
        if(live)
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
            rigid.velocity = Vector2.zero;
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
    }

    private IEnumerator AnimPlay()
    {
        animator.Play("GoombaStepped");
        yield return delay;
        Destroy(gameObject);
    }

}
