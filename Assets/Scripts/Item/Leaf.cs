using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : Item
{

    public float fallSpeed = 1f;
    public float swaySpeed = 10f;
    public float swayAmount = 0.5f;

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private float swayDir = 1;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    private void FixedUpdate()      // 살랑살랑 떨어지는 로직
    {
        float sway = Mathf.Sin(Time.time * swaySpeed) * swayAmount;     // 삼각함수를 사용하여 진자운동 구현
        rb.velocity = new Vector2(sway, -fallSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
