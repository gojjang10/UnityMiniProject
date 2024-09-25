using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaController : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] float movePower;
    [SerializeField] float maxMoveSpeed;

    private void FixedUpdate()
    {
        rigid.AddForce(Vector2.left * movePower, ForceMode2D.Impulse);

        if(rigid.velocity.x > maxMoveSpeed)
        {
            rigid.velocity = new Vector2(maxMoveSpeed, rigid.velocity.y);
        }
        else
        {
            rigid.velocity = new Vector2(-maxMoveSpeed, rigid.velocity.y);
        }

    }
}
