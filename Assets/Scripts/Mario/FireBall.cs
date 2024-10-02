using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float bounceForce;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Vector2 velocity;
    

    private float lastGround;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    private void FixedUpdate()
    {
        Vector2 currentVelocity = rb.velocity;

        rb.velocity = new Vector2(rb.velocity.x, currentVelocity.y);
    }


    public void SetSpeed(Vector2 speed)
    {
        rb.velocity = speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("파이어볼이 지면과 닿음");
            Vector2 curVelocity = rb.velocity;

            lastGround = collision.contacts[0].point.y;     // 첫번째 충돌 지점 초기화

            if (collision.contacts[0].point.y > lastGround)         // 이전 충돌 지점보다 더 높다면
            {
                Debug.Log("if 작동");
                rb.velocity = new Vector2(curVelocity.x, bounceForce + (collision.contacts[0].point.y - lastGround));       // 그만큼 더 많이 튀어오른다
            }
            else
            {
                Debug.Log("else if 작동");
                rb.velocity = new Vector2(curVelocity.x, bounceForce);      // 아닐 시에는 기본적인 힘으로 진행
            }


        }
    }
}
