using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : Item
{
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] Collider2D col;

    //[SerializeField] Animator animator;

    //[SerializeField] AudioClip sfx;

    private void Start()
    {
        Vector3 beforePos = new Vector3 (transform.position.x, transform.position.y - 1, transform.position.z);
        Vector3 curPos = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(beforePos, curPos, 1f);
    }

    private void FixedUpdate()
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
