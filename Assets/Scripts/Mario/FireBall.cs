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
        velocity = rb.velocity;
    }

    public void SetSpeed(Vector2 speed)
    {
        rb.velocity = speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 6)
        {
            Debug.Log("지면과 닿음");
            Vector2 curVelocity = rb.velocity;

            
            if (collision.contacts[0].point.y > lastGround)
            {
                Debug.Log("if 작동");
                rb.velocity = new Vector2(curVelocity.x, bounceForce + (collision.contacts[0].point.y - lastGround));
            }
            else
            {
                Debug.Log("else if 작동");
                rb.velocity = new Vector2(curVelocity.x, bounceForce);
            }

            lastGround = collision.contacts[0].point.y;
        }
    }
}
